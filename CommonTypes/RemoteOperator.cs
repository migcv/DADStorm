using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CommonTypes {

	public abstract class RemoteOperator : MarshalByRefObject {
		public string type;				// Type of the Operator
		public Tuple result = null;     // Tuple to send
		public string op_id;			// Operator ID
		public List<Tuple> inputList = new List<Tuple>();

		public string[] inputSources;   // URLs of the previous Operators
		public string[] outputSources;  // URLs of the next Operators
        public List<String> workingOutputSources;

		public string op_url;			// Operators own URL
		public string[] replicas_url;	// Operators replicas URL
		public string[] output_op;		// Output Opeartor (last operator) URL

		public string routing;			// Routing method: PRIMARY, HASH, RANDOM

		public string semantic;			// at-most-once | at-least-once | exactly-once

		public bool isFullLog;			// Controls if Operator have to send logs of the results to the PuppetMaster

		public string[] parameters;		// Parameters for the operation

		public string state = Constants.STATE_WAITING;

		public bool doWork = false;      // Controls when PuppetMaster says that the Operator can work (Default: false)
		public bool isFreezed = false;   // Controls when PuppetMaster says to the Operator to freeze(true) or to unfreeze(false)
		public int interval = 0;

		private bool logCleaned = false;
		private List<string> logList;

        public void AsyncCallBackOperator(IAsyncResult ar) {
            try {
                RemoteAsyncDelegateTuple del = (RemoteAsyncDelegateTuple)((AsyncResult)ar).AsyncDelegate;
                del.EndInvoke(ar);
            }catch {
                buildWorkingOps();
                if (workingOutputSources != null)
                {
                   
                TrySend:
                    Console.WriteLine("FALLBACK");
                    if (this.workingOutputSources.Count > 0)
                    {
                        string nextOp = workingOutputSources[0];
                        try
                        {
                            RemoteOperator outputOperator = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), nextOp);
                            RemoteAsyncDelegateTuple RemoteDel = new RemoteAsyncDelegateTuple(outputOperator.receiveInput);

                            Console.WriteLine("SENDING CALLBACK - Result: " + result);
                            IAsyncResult RemAr = RemoteDel.BeginInvoke(this.result, null, null);
                            RemoteDel.EndInvoke(RemAr);
                            writeLog(this.op_id, this.type, this.op_url, "RESULT <" + result.ToString() + ">");

                        }
                        catch (Exception e1)
                        {
                            this.workingOutputSources.RemoveAt(0);
                            goto TrySend;
                        }
                    }
                }
            }
            return;
		}

		public RemoteOperator(string type, string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] output_op, string[] replicas_op, string[] parameters = null) {
			this.type = type;
			this.inputSources = inputSources;
			this.outputSources = outputSources;
			this.routing = routing;
			this.isFullLog = logLevel;
			this.parameters = parameters;
			this.output_op = output_op;
			this.replicas_url = replicas_op;
        }

		public void process() {
			while (true) {
				while (isFreezed || !doWork || inputList.Count == 0) ;  // Waits until PuppetMaster unfreezes Operator
				if (inputList[0] != null) {
					Tuple input = inputList[0];
					doOperation(input);
					while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
					sendResult();
					inputList.RemoveAt(0);
					sleep();
					reset();
				}
			}
		}

		public void receiveInput(Tuple input) {
			inputList.Add(input);
			Console.WriteLine("RECEIVED <" + input.ToString() + ">;\r\n");
			// Writing on log throgh the output operator
			if (String.Compare(this.op_url, this.output_op[0]) == 0) {
				writeLog(this.op_id, this.type, this.op_url, "RECEIVED <" + input.ToString() + ">");
			}
			else {
				RemoteOperator output_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.output_op[0]);
				RemoteAsyncDelegateLog RemoteDelOutput = new RemoteAsyncDelegateLog(output_op.writeLog);
				IAsyncResult RemArOutput = RemoteDelOutput.BeginInvoke(this.op_id, this.type, this.op_url, "RECEIVED <" + input.ToString() + ">", null, null);
            }
		}

		public abstract void doOperation(Tuple input);

		public void sendResult() {
			if (this.result == null) {
				return;
			}
			// Writing on log throgh the output operator
			if (String.Compare(this.op_url, this.output_op[0]) == 0) {
				writeLog(this.op_id, this.type, this.op_url, "RESULT <" + result.ToString() + ">");
			}
			else {
				RemoteOperator output_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.output_op[0]);
				RemoteAsyncDelegateLog RemoteDelOutput = new RemoteAsyncDelegateLog(output_op.writeLog);
				IAsyncResult RemArOutput = RemoteDelOutput.BeginInvoke(this.op_id, this.type, this.op_url, "RESULT <" + result.ToString() + ">", null, null);
			}
			Console.WriteLine("RESULT <" + result.ToString() + ">;\r\n");

			if (this.outputSources == null) {
				return;
			}

			IDictionary options = new Hashtable();
			options["name"] = this.type + "output" + (new Random()).Next(0, 10000); ;

			TcpChannel channel = new TcpChannel(options, null, null);
			ChannelServices.RegisterChannel(channel, true);

            String firstOp = this.getRoutingOperator();

            RemoteOperator outputOperator = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), firstOp);
            RemoteAsyncDelegateTuple RemoteDel = new RemoteAsyncDelegateTuple(outputOperator.receiveInput);
                
            if (!semantic.Equals("at-most-once")){ // semantic at-least-once, exactly-once
                IAsyncResult RemAr = RemoteDel.BeginInvoke(this.result, AsyncCallBackOperator, this.result);
            }
            else{ // semantic at-most-once
                IAsyncResult RemAr = RemoteDel.BeginInvoke(this.result, null, null);
            }
            //RemoteDel.EndInvoke(RemAr);
           

            if (isFullLog) { // Is full log, must send result to PuppetMaster
				RemotePM pm = (RemotePM)Activator.GetObject(typeof(RemotePM), "tcp://localhost:10001/RemotePM");
				string[] resultArray = { op_id, type, result.ToString() };
				pm.receiveResult(resultArray);
			}

			ChannelServices.UnregisterChannel(channel);
		}

		public void writeLog(string op_id, string op_type, string op_url, string str) {
			logList.Add("<" + DateTime.Now + "><" + op_id + " " + op_url + ">: <" + op_type + "> " + str +";");
		}

		public string Name {
			get { return type; }
		}

		public void setInterval(int miliseconds) { interval = miliseconds; }

		public void startWorking(string op_id, string op_url, string semantic) {
			this.op_id = op_id;
			this.semantic = semantic;
			this.op_url = op_url;
			state = Constants.STATE_RUNNING;
			doWork = true;
			start();
			// Writing on log throgh the output operator
			if (String.Compare(this.op_url, this.output_op[0]) == 0) {
				writeLog(this.op_id, this.type, this.op_url, "STARTED");
			}
			else {
				RemoteOperator output_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.output_op[0]);
				RemoteAsyncDelegateLog RemoteDelOutput = new RemoteAsyncDelegateLog(output_op.writeLog);
				IAsyncResult RemArOutput = RemoteDelOutput.BeginInvoke(this.op_id, this.type, this.op_url, "STARTED", null, null);
			}
			Console.WriteLine("STARTED;\r\n");
		}

		public void crashOperator() {
			System.Environment.Exit(1);
		}

		public void freeze() {
			isFreezed = true;
			state = Constants.STATE_FREEZED;
			// Writing on log throgh the output operator
			if (String.Compare(this.op_url, this.output_op[0]) == 0) {
				writeLog(this.op_id, this.type, this.op_url, "FREZZED");
			}
			else {
				RemoteOperator output_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.output_op[0]);
				RemoteAsyncDelegateLog RemoteDelOutput = new RemoteAsyncDelegateLog(output_op.writeLog);
				IAsyncResult RemArOutput = RemoteDelOutput.BeginInvoke(this.op_id, this.type, this.op_url, "FREZZED", null, null);
			}
			Console.WriteLine("FREZZED;\r\n");
		}

		public void unfreeze() {
			isFreezed = false;
			if (doWork) {
				state = Constants.STATE_RUNNING;
			}
			else {
				state = Constants.STATE_WAITING;
			}
			// Writing on log throgh the output operator
			if (String.Compare(this.op_url, this.output_op[0]) == 0) {
				writeLog(this.op_id, this.type, this.op_url, "UNFREZZED");
			}
			else {
				RemoteOperator output_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.output_op[0]);
				RemoteAsyncDelegateLog RemoteDelOutput = new RemoteAsyncDelegateLog(output_op.writeLog);
				IAsyncResult RemArOutput = RemoteDelOutput.BeginInvoke(this.op_id, this.type, this.op_url, "UNFREZZED", null, null);
			}
			Console.WriteLine("UNFREZZED;\r\n");
		}

		public string operatorState() { return state; }

		public override object InitializeLifetimeService() {
			return null;
		}
		/*
		 *	Private Methods
		 */

		private void logWritter() {
			while (true) {
				while (logList.Count == 0) ;  // Waits until have something to write on log
				using (StreamWriter file = new StreamWriter(@"../../../output/log.txt", true)) {
					file.WriteLine(""+logList[0]);
				}
				logList.RemoveAt(0);
			}
		}

		private void start() {
			Thread oThreadProcess = new Thread(new ThreadStart(this.process));
			oThreadProcess.Start();
			while (!oThreadProcess.IsAlive);

			if(String.Compare(output_op[0], op_url) == 0) {
				logList = new List<string>();
				File.WriteAllText(@"../../../output/log.txt", "");
				Thread oThreadLog = new Thread(new ThreadStart(this.logWritter));
				oThreadLog.Start();
				while (!oThreadLog.IsAlive) ;
			}

			if (!inputSources[0].StartsWith("tcp://")) { // Input from file
				List<Tuple> tupleList = this.readFromFile(inputSources[0]);
				doWork = true;
				foreach (Tuple tuple in tupleList) {
					receiveInput(tuple);
				}
			}
			else { // Input from Operator
				doWork = true;
			}
		}

		private List<Tuple> readFromFile(String fileName) {
			List<Tuple> tupleList = new List<Tuple>();

			String[] lines = System.IO.File.ReadAllLines("../../../input/" + fileName);

			foreach (String line in lines) {
				String tupleStr;

				int commentIndex = line.IndexOf('%');

				if (commentIndex == 0) {
					tupleStr = "";
				}
				else if (commentIndex > 0) {
					tupleStr = line.Substring(0, commentIndex);
				}
				else {
					tupleStr = line;
				}

				if (tupleStr.Length > 0) {
					tupleList.Add(Tuple.fromString(tupleStr));
				}
			}
			return tupleList;
		}

		private String getRoutingOperator()
        {
            if(this.routing.Equals("primary"))
            {
                return this.outputSources[0];
            }
            else if(this.routing.Equals("random"))
            {
                Random rnd = new Random();

                return this.outputSources[rnd.Next(0, outputSources.Length)];
            }
            else if(this.routing.StartsWith("hashing"))
            {
                Regex regex = new Regex(@"\((.*?)\)");
                var match = regex.Match(this.routing);
                String idStr = match.Groups[1].ToString();

                Tuple tuple = this.inputList[0];

                if(tuple != null)
                {
                    String field = tuple.getField(Int32.Parse(idStr));

                    if(field != null)
                    {
                        int operatorId = Math.Abs(field.GetHashCode()) % this.outputSources.Length;

                        return this.outputSources[operatorId];
                    }
                }
            }
			return this.outputSources[0]; // For now is PRIMARY
		}

		private void reset() {
			//this.input = null;
			this.result = null;
		}

		private void sleep() {
			state = Constants.STATE_WAITING;
			Thread.Sleep(interval);
			state = Constants.STATE_RUNNING;
		}

        private void buildWorkingOps()
        {
            if (this.outputSources == null) return;

            this.workingOutputSources = new List<string>();

            foreach(String op in this.outputSources)
            {
                this.workingOutputSources.Add(op);
            }
        }

		/*
		 *	CODE THATS NO LONGER USED 
		 */
		public void start_OLD() {
			// Change Flags
			//doWork = true;
			// Check for input files
			foreach (String fileName in inputSources) {
				if (!fileName.StartsWith("tcp://")) {
					List<Tuple> tupleList = this.readFromFile(fileName);

					foreach (Tuple t in tupleList) {
						//process(t);
					}
				}
			}
		}
		public void process_OLD(Tuple input) {
			while (isFreezed || !doWork) ;  // Waits until PuppetMaster unfreezes Operator
			receiveInput(input);
			while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
			//doOperation();
			while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
			sendResult();
			sleep();
			reset();
		}

	}
}
