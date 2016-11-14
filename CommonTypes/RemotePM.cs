using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace CommonTypes
{
    public class RemotePM : MarshalByRefObject {
		private RemotePCS pcs;
		public RemotePM() {
			//TcpChannel channel = new TcpChannel();
			//ChannelServices.RegisterChannel(channel, true);

			pcs = (RemotePCS)Activator.GetObject(typeof(RemotePCS), "tcp://localhost:10000/RemotePCS");
		}
		public void createOperator(object[] operator_info) {  // Operator Info {ID, URLs, Type, InputSources, OutputSources, RoutingOption, logLevel, params}
			string op_port;
			for (int i = 0; i < ((List<string>) operator_info[1]).Count; i++) { // Register Operator in all URLs assinged
				op_port = ((List<string>)operator_info[1])[i].Split(':')[2].Split('/')[0];

                // createOperator(operatorType, operatorPort, isFullLog, routing, inputSources, outputSources, operatorParams)

                List<string> unsafeInput = (List<string>)operator_info[3];
                List<string> unsafeOutput = (List<string>)operator_info[4];
                List<string> unsafeParams = (List<string>)operator_info[7];

                String[] input = unsafeInput == null ? null : unsafeInput.ToArray();
                String[] output = unsafeOutput == null ? null : unsafeOutput.ToArray();
                String[] parameters = (unsafeParams == null || unsafeParams.Count < 1) ? null : unsafeParams.ToArray();
                
                pcs.createOperator((string) operator_info[2], Int32.Parse(op_port), (bool)operator_info[6], (string) operator_info[5], input, output, parameters);
			}
		}

		public void startOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
			//rm_op.startWorking();
		}

		public void crashOperator(string op_url) {
			//TODO
		}

		public void intervalOperator(string op_url, int miliseconds) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
			//rm_op.setInterval(miliseconds);
		}

		public void freezeOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
			//rm_op.freeze();
		}

		public void unfreezeOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
			//rm_op.unFreeze();
		}

		public List<string> statusOperator() {
			return null;
		}
	}
}
