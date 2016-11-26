using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;

namespace CommonTypes {

	public abstract class RemoteOperator : MarshalByRefObject {
		public string type;             // Name of the Operator
		public Tuple input;             // Tuple to operate
		public Tuple result;                // Tuple to send

		public string[] inputSources;   // URLs of the previous Operators
		public string[] outputSources;     // URLs of the next Operators

		public string routing;           // Routing method: PRIMARY, HASH, RANDOM

		public bool isFullLog;           // Controls if Operator have to send logs of the results to the PuppetMaster

		public string[] parameters;      // Parameters for the operation

		public string state = Constants.STATE_WAITING;

		public bool doWork = false;      // Controls when PuppetMaster says that the Operator can work (Default: false)
		public bool isFreezed = false;   // Controls when PuppetMaster says to the Operator to freeze(true) or to unfreeze(false)
		public int interval = 0;

		public RemoteOperator() { }

		public RemoteOperator(string type, string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] parameters = null) {
			this.type = type;
			this.inputSources = inputSources;
			this.outputSources = outputSources;
			this.routing = routing;
			this.isFullLog = logLevel;
			this.parameters = parameters;
			//start();
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

		public void start() {
			// Change Flags
			doWork = true;
			// Check for input files
			foreach (String fileName in inputSources) {
				if (!fileName.StartsWith("tcp://")) {
					List<Tuple> tupleList = this.readFromFile(fileName);

					foreach (Tuple t in tupleList) {
						process(t);
					}
				}
			}
		}

		public void process(Tuple input) {
			while (isFreezed || !doWork);  // Waits until PuppetMaster unfreezes Operator
			reset();
			while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
			receiveInput(input);
			while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
			doOperation();
			while (isFreezed) ;  // Waits until PuppetMaster unfreezes Operator
			sendResult();
			sleep();
		}

		public void receiveInput(Tuple input) {
			this.input = input;
			Console.WriteLine("Received " + input.ToString());
		}

		public abstract void doOperation();

		public void sendResult() {
			if (this.result == null || this.outputSources == null) {
				return;
			}

			Console.WriteLine("Sending " + this.result.ToString());
			IDictionary options = new Hashtable();
			options["name"] = this.type + "output" + (new Random()).Next(0, 10000); ;

			TcpChannel channel = new TcpChannel(options, null, null);
			ChannelServices.RegisterChannel(channel, true);

			RemoteOperator outputOperator = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), this.getRoutingOperator());
			//outputOperator.process(this.result);

			RemoteAsyncDelegateProcess RemoteDel = new RemoteAsyncDelegateProcess(outputOperator.process);
			// Call delegate to remote method
			IAsyncResult RemAr = RemoteDel.BeginInvoke(this.result, null, null);
			// Wait for the end of the call and then explictly call EndInvoke
			//RemAr.AsyncWaitHandle.WaitOne();

			ChannelServices.UnregisterChannel(channel);
		}

		public string Name {
			get { return type; }
		}

		public void setInterval(int miliseconds) { interval = miliseconds; }

		public void startWorking() {
			state = Constants.STATE_RUNNING;
			start();
		}

		public void crashOperator() {
			System.Environment.Exit(1);
		}

		public void freeze() {
			isFreezed = true;
			state = Constants.STATE_FREEZED;
		}

		public void unFreeze() {
			isFreezed = false;
			if (doWork) {
				state = Constants.STATE_RUNNING;
			}
			else {
				state = Constants.STATE_WAITING;
			}
		}

		public string operatorState() { return state; }

		public override object InitializeLifetimeService() {
			return null;
		}
		/*
		 *	Private Methods
		 */
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

                Tuple tuple = this.input;

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
			this.input = null;
			this.result = null;
		}

		private void sleep() {
			state = Constants.STATE_WAITING;
			Thread.Sleep(interval);
			state = Constants.STATE_RUNNING;
		}
	}
}
