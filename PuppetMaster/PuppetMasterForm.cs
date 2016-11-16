using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Windows.Forms;

namespace PuppetMaster {
	public partial class PuppetMasterForm : Form {
		private readonly int PM_PORT = 10001;
		private readonly bool LOCALHOST = true; // Flag that says if the project is to be run in a single machine or in many machines
		private int port_localhost = 10002;	// 1st port thats free to use, only if the project is running in one machine

		private Dictionary<string, List<string>> operatorsURL; // List with each operators replica url

		private List<string> commandsToDo; // Contains the commands to do after the operators are created

		public PuppetMasterForm() {
			InitializeComponent();
			// UI Stuff - Disables Buttons and TextBoxes until a config file is read
			logTextBox.ReadOnly = true;
			commandTextBox.Enabled = false;
			startAllButton.Enabled = false;
			startOneButton.Enabled = false;
			commandButton.Enabled = false;

			// Create Remote PuppetMaster
			TcpChannel channel = new TcpChannel(PM_PORT);
			ChannelServices.RegisterChannel(channel, true);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotePM), "RemotePM", WellKnownObjectMode.Singleton);
			logTextBox.AppendText("<PuppetMaster>: Insert a config file;\r\n");
		}

		private void loadConfigFileButton_Click(object sender, EventArgs e) {
			DialogResult result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK) {
				logTextBox.AppendText("<PuppetMaster>: Config file <" + openFileDialog.FileName + "> selected;\r\n");
				resetVariables();
				readConfigFile(openFileDialog.FileName);
				// UI Stuff - Enables Buttons and TextBoxes
				commandTextBox.Enabled = true;
				//startAllButton.Enabled = true;
				//startOneButton.Enabled = true;
				commandButton.Enabled = true;
				executeCommands();
			}
		}

		private void commandButton_Click(object sender, EventArgs e) {
			string command = commandTextBox.Text;
			commandTextBox.Text = "";
			doCommand(command);
		}

		private void createOperator(object[] operator_info) { // Operator Info {ID, URLs, Type, InputSources, OutputSources, RoutingOption, logLevel, params}
			RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
			pm.createOperator(operator_info);
			logTextBox.AppendText("<PuppetMaster>: Remote Operator <" + operator_info[0] + "> is up!\r\n");
		}

		private void startOperator(string op_id) {
			if (operatorExists(op_id)) {
				logTextBox.AppendText("<PuppetMaster>: <" + op_id + ">");
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				for (int i = 0; i < operatorsURL[op_id].Count; i++) {
					pm.startOperator(operatorsURL[op_id][i]);
					logTextBox.AppendText(" <" + operatorsURL[op_id][i] + ">");
				}
				logTextBox.AppendText(" started;\r\n");
			}
		}

		private void crashReplica(string op_id, int replica_id) {
			if (operatorExists(op_id) && replicaExists(op_id, replica_id)) {
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				pm.crashOperator(operatorsURL[op_id][replica_id]);
				logTextBox.AppendText("<PuppetMaster>: <" + op_id + "> <" + operatorsURL[op_id][replica_id] + "> crashed;\r\n");
				operatorsURL[op_id].RemoveAt(replica_id);
				if(operatorsURL[op_id].Count == 0) { // If doesnt have any more replicas remove Operator from Dictionary
					operatorsURL.Remove(op_id);
				}
			}
		}

		private void intervalOperator(string op_id, int miliseconds) {
			if (operatorExists(op_id)) {
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				for (int i = 0; i < operatorsURL[op_id].Count; i++) {
					pm.intervalOperator(operatorsURL[op_id][i], miliseconds);
				}
				logTextBox.AppendText("<PuppetMaster>: <" + op_id + "> interval <" + miliseconds + ">;\r\n");
			}
		}

		private void freezeReplica(string op_id, int replica_id) {
			if (operatorExists(op_id) && replicaExists(op_id, replica_id)) {
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				pm.freezeOperator(operatorsURL[op_id][replica_id]);
				logTextBox.AppendText("<PuppetMaster>: <" + op_id + "> <" + operatorsURL[op_id][replica_id] + "> is freezed ;\r\n");
			}
		}

		private void unfreezeReplica(string op_id, int replica_id) {
			if (operatorExists(op_id) && replicaExists(op_id, replica_id)) {
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				pm.unfreezeOperator(operatorsURL[op_id][replica_id]);
				logTextBox.AppendText("<PuppetMaster>: <" + op_id + "> <" + operatorsURL[op_id][replica_id] + "> is unfreezed ;\r\n");
			}
		}

		private void statusOperators(string op_id) {
			if (operatorExists(op_id)) {
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				for (int i = 0; i < operatorsURL[op_id].Count; i++) {
					string state = pm.statusOperator(operatorsURL[op_id][i]);
					logTextBox.AppendText("<PuppetMaster>: <" + op_id + "> <" + operatorsURL[op_id][i] + "> status is " + state + ";\r\n");
				}
			}
		}

		private void wait(int miliseconds) {
			Thread.Sleep(miliseconds);
		}

		private void readConfigFile(string path) {
			commandsToDo = new List<string>(); // Initialize a new list of commands

			List<object[]> operartorList = new List<object[]>();
			string op_id = null, n_rep = null, routing = null, op_type = null;
			List<string> input = null, urlList = null, op_params = null;

			string loggingLevel = null, semantics = null;

			logTextBox.AppendText("<PuppetMaster>: Reading config file;\r\n");

			// Open the stream and read it back.
			using (StreamReader sr = new StreamReader(path)) {
				while (sr.Peek() >= 0) {
					string line = sr.ReadLine();

					if (!line.StartsWith("%") && !line.Equals("")) { // Line isn't a comment nor a <Enter>
						//logTextBox.AppendText(line + "\r\n");
						if(line.StartsWith("LoggingLevel")) {  // Logging Level
							loggingLevel = line.Contains("light") ? "light" : "full";
							logTextBox.AppendText("<PuppetMaster>: Logging level <" + loggingLevel + ">\r\n");
						}
						else if(line.StartsWith("Semantics")) { // Semantics
							semantics = line.Contains("at-most-once") ? "at-most-once" : line.Contains("at-least-once") ? "at-least-once" : "exactly-once";
							logTextBox.AppendText("<PuppetMaster>: Semantic <" + semantics + ">;\r\n");
						}
						else if(line.StartsWith("Interval") || line.StartsWith("Status") || line.StartsWith("Start") || 
						line.StartsWith("Crash") || line.StartsWith("Freeze") || line.StartsWith("Unfreeze") || 
						line.StartsWith("Wait")) { // PuppetMaster Command
							commandsToDo.Add(line);
						}
						else { // Operator definition
							input = new List<string>();
							string[] lineSplited = line.Split(' ');
							for (int i = 0; i < lineSplited.Length; i++) {
								if (lineSplited[i].Equals("input") && lineSplited[i + 1].Equals("ops")) { // Operator ID & Input
									op_id = lineSplited[i - 1];
									input.Add(lineSplited[i + 2]);
								}
								else if (lineSplited[i].Equals("routing")) { // Number of Replicas & Routing Option
									n_rep = lineSplited[i - 1];
									routing = lineSplited[i + 1];
								}
								else if (lineSplited[i].Equals("address")) {
									if (LOCALHOST) { // Project is running in one single machine
										urlList = new List<string>();
										for (i = i + 1; !lineSplited[i].Equals("operator"); i++, port_localhost++) { // Operators URLs, in localhost is always "tcp://localhost:<port_number>/op"
											urlList.Add("tcp://localhost:"+ port_localhost + "/op");
										}
										op_type = lineSplited[i + 2];
										i += 3;
										op_params = new List<string>();
										if (lineSplited.Length > i) { // Operator have params
											string[] params_aux = lineSplited[i].Split(',');
											for (int j = 0; j < params_aux.Length; j++) {
												op_params.Add(params_aux[j]);
											}
										}

									}
									else { // Project is running in many machines
										//TODO
									}
								}
							}
							logTextBox.AppendText("<PuppetMaster>: Created <" + n_rep + "> <" + op_type + "> Operator <"+ op_id + "> input <" + input[0] + "> routing <" + routing + "> URLs ");
							for(int i = 0; i < urlList.Count; i++) {
								logTextBox.AppendText("<" + urlList[i] + "> ");
							}
							logTextBox.AppendText("params ");
							for (int i = 0; i < op_params.Count; i++) {
								logTextBox.AppendText("<" + op_params[i] + "> ");
							}
							logTextBox.AppendText("\r\n");
							// Operator Info {ID, URLs, Type, InputSources, OutputSources, RoutingOption, logLevel, params}
							object[] operator_info = { op_id, urlList, op_type, input, null, routing, loggingLevel, op_params };
							operartorList.Add(operator_info);
						}
					}
				}
			}
			// Insert inputSources & outputSources in operators
			logTextBox.AppendText("<PuppetMaster>: Operators scheme ");
			for (int i = 0; i < operartorList.Count; i++) {
				for(int j = 0; j < operartorList.Count; j++) {
					if(((List<string>) operartorList[i][3])[0].Equals(operartorList[j][0])) { // OPi input source is OPj 
						operartorList[i][3] = operartorList[j][1]; // Gives OPj URLs to OPi in input sorces
						operartorList[j][4] = operartorList[i][1]; // Gives OPi URLs to OPj in output sorces
						logTextBox.AppendText("<" + operartorList[j][0] + "-->" + operartorList[i][0] + "> ");
					}
				}
			}
			logTextBox.AppendText("\r\n");
			// Create the Remote Operator & insert Operator ID and URL in Dicitonary
			for (int i = 0; i < operartorList.Count; i++) {
				operartorList[i][6] = ((string) operartorList[i][6]).Equals("full") ? true : false;
				createOperator(operartorList[i]);
				operatorsURL.Add((string) operartorList[i][0], (List<string>) operartorList[i][1]);
			}
		}

		private void executeCommands() {
			if(commandsToDo.Count != 0) { // If commands were included in the config file
				logTextBox.AppendText("<PuppetMaster>: Running Config file's commands;\r\n");
				for (int i = 0; i < commandsToDo.Count; i++) {
					doCommand(commandsToDo[i]);
				}
			}
			else {
				logTextBox.AppendText("<PuppetMaster>: Config file didn't include commands;\r\n");
				logTextBox.AppendText("<PuppetMaster>: Insert commands;\r\n");
			}
		}

		private void doCommand(string command) {
			string[] commandSplited = command.Split(' ');
			if (command.StartsWith("Start")) {
				startOperator(commandSplited[1]);
			}
			else if (command.StartsWith("Crash")) {
				crashReplica(commandSplited[1], Int32.Parse(commandSplited[2]));
			}
			else if (command.StartsWith("Status")) {
				foreach(string i in operatorsURL.Keys) {
					statusOperators(i);
				}	
			}
			else if (command.StartsWith("Interval")) {
				intervalOperator(commandSplited[1], Int32.Parse(commandSplited[2]));
			}
			else if (command.StartsWith("Freeze")) {
				freezeReplica(commandSplited[1], Int32.Parse(commandSplited[2]));
			}
			else if (command.StartsWith("Unfreeze")) {
				unfreezeReplica(commandSplited[1], Int32.Parse(commandSplited[2]));
			}
			else if (command.StartsWith("Wait")) {
				wait(Int32.Parse(commandSplited[1]));
				logTextBox.AppendText("<PuppetMaster>: Waiting...;\r\n");
			}
			else {
				if (!String.IsNullOrEmpty(command))
					logTextBox.AppendText("<PuppetMaster>: Command unknown (Start | Crash | Status | Interval | Freeze | Unfreeze | Wait);\r\n");
			}
		}

		private bool operatorExists(string op_id) {
			if(!operatorsURL.ContainsKey(op_id)) {
				logTextBox.AppendText("<PuppetMaster>: Operator <" + op_id + "> doesn't exists;\r\n");
				return false;
			}
			return true;
		}

		private bool replicaExists(string op_id, int replica_id) {
			if (operatorsURL[op_id].Count <= replica_id) {
				logTextBox.AppendText("<PuppetMaster>: Replica <" + replica_id + "> of <" + op_id + "> doesn't exists;\r\n");
				return false;
			}
			return true;
		}

		private void resetVariables() {
			operatorsURL = new Dictionary<string, List<string>>();
			commandsToDo = new List<string>();
			logTextBox.Text = "";
		}

		private void crashAllButton_Click(object sender, EventArgs e) {
			List<string> urls = new List<string>();
			foreach (List<string> op_urls in operatorsURL.Values) {
				for (int j = 0; j < op_urls.Count; j++) {
					try {
						RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_urls[j]);
						rm_op.crashOperator();
					}
					catch { }
				}
			}
		}
	}
}
