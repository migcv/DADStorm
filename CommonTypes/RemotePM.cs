using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace CommonTypes
{
	public class RemotePM : MarshalByRefObject {

		private RemotePCS pcs;

		private List<string[]> resultList = new List<string[]>();

		public RemotePM() {
			pcs = (RemotePCS)Activator.GetObject(typeof(RemotePCS), "tcp://localhost:10000/RemotePCS");
		}
		public void createOperator(object[] operator_info) {  // Operator Info {ID, URLs, Type, InputSources, OutputSources, RoutingOption, logLevel, params}
			string op_port;
			int n_op = ((List<string>)operator_info[1]).Count;
			for (int i = 0; i < n_op; i++) { // Register Operator in all URLs assinged
				op_port = ((List<string>)operator_info[1])[i].Split(':')[2].Split('/')[0];

				List<string> unsafeInput = (List<string>)operator_info[3];
				List<string> unsafeOutput = (List<string>)operator_info[4];
				List<string> unsafeParams = (List<string>)operator_info[9];

				String[] input = unsafeInput == null ? null : unsafeInput.ToArray();
				String[] output = unsafeOutput == null ? null : unsafeOutput.ToArray();
				String[] parameters = (unsafeParams == null || unsafeParams.Count < 1) ? null : unsafeParams.ToArray();

				// createOperator(operatorID, operatorType, operatorPort, isFullLog, routing, inputSources, outputSources, output_op, replicas_url, operatorParams)
				pcs.createOperator((string)operator_info[2], Int32.Parse(op_port), (bool)operator_info[6], (string)operator_info[5], input, output, (List<string>)operator_info[7], (List<string>)operator_info[1], parameters);
			}
		}

		public void startOperator(string op_url, string op_id) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);

			RemoteAsyncDelegateStart RemoteDel = new RemoteAsyncDelegateStart(rm_op.startWorking);
			IAsyncResult RemAr = RemoteDel.BeginInvoke(op_id, op_url, null, null);
		}

		public void crashOperator(string op_url) {
			try {
				RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);

				RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(rm_op.crashOperator);
				IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
			}
			catch { }
		}

		public void intervalOperator(string op_url, int miliseconds) {
            try
            {
                RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
			    rm_op.setInterval(miliseconds);
            }
            catch(Exception e) {}
			
		}

		public void freezeOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);

			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(rm_op.freeze);
			IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
		}

		public void unfreezeOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);

			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(rm_op.unfreeze);
			IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
		}

		public string statusOperator(string op_url) {
			RemoteOperator rm_op = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), op_url);
            try
            {
                return rm_op.operatorState();
            }
            catch(Exception e)
            {
                return "UNKNOWN";
            }
		
		}

		public void receiveResult(string[] result) {
			resultList.Add(result);
		}

		public string[] getResult() {
			if(resultList.Count > 0) {
				string[] res = resultList[0];
				resultList.RemoveAt(0);
				return res;
			}
			return null;
		}
	}
}
