using CommonTypes;
using CommonTypes.operators;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Operator
{
	class Program {
		static void Main(string[] args) {
			//Debugger.Launch();

			if (args.Length < 6) {
				Console.WriteLine("Invalid Argument Count");
				return;
			}

			String type = args[0];
			int port = Int32.Parse(args[1]);
			Boolean fullLog = args[2].Equals("full") ? true : false;
			String routing = args[3];
			String[] input = args[4].Split(',');
			String[] output = args[5].Split(',');
			String[] output_op = args[6].Split(',');
			String[] replicas_op = args[7].Split(',');

			String[] parameters = null;

			if (args.Length > 8) {
				parameters = args[8].Split(',');

				if (parameters[0] == null || parameters[0].Equals("")) {
					parameters = null;
				}
			}

			if (input[0] == null || input[0].Equals("")) {
				input = null;
			}

			if (output[0] == null || output[0].Equals("")) {
				output = null;
			}

			TcpChannel channel = new TcpChannel(port);
			ChannelServices.RegisterChannel(channel, true);

			RemoteOperator remoteOperator = createOperatorType(type, fullLog, routing, input, output, output_op, replicas_op, parameters);

			RemotingServices.Marshal(remoteOperator, "op", remoteOperator.GetType());

			Console.WriteLine("Started Operator " + type + " on port " + port);
			Console.ReadLine();
		}

		private static RemoteOperator createOperatorType(String operatorType, Boolean isFullLog, String routing, String[] inputSources, String[] outputSources, String[] output_op, String[] replicas_op, String[] operatorParams)
        {
            RemoteOperator remoteOperator;

            switch (operatorType)
            {
                case "COUNT":
                    remoteOperator = new Count(inputSources, outputSources, routing, isFullLog, output_op, replicas_op);
                    break;
                case "CUSTOM":
                    remoteOperator = new Custom(inputSources, outputSources, routing, isFullLog, output_op, replicas_op, operatorParams);
                    break;
                case "DUP":
                    remoteOperator = new Dup(inputSources, outputSources, routing, isFullLog, output_op, replicas_op);
                    break;
                case "FILTER":
                    remoteOperator = new Filter(inputSources, outputSources, routing, isFullLog, output_op, replicas_op, operatorParams);
                    break;
                case "UNIQ":
                    remoteOperator = new Uniq(inputSources, outputSources, routing, isFullLog, output_op, replicas_op, operatorParams);
                    break;
                default:
                    throw new Exception("Unknown Operator Type");
            }

            return remoteOperator;
        }
    }
}