using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonTypes
{
    public class RemotePCS : MarshalByRefObject
    {
        private List<int> runningOperators;

        public RemotePCS()
        {
            this.runningOperators = new List<int>();
        }

        public Boolean createOperator(String operatorType, int operatorPort, Boolean isFullLog, String routing, String[] inputSources, String[] outputSources, List<string> output_operator, List<string> replicas_url_op, String[] operatorParams)
        {
            String syntaxWithParameters = "{0} {1} {2} {3} \"{4}\" \"{5}\" \"{6}\" \"{7}\" \"{8}\""; //operatorType, operatorPort, isFullLog, routing, inputSources, outputSources, output_op, replicas_url, operatorParams
			String syntaxWithoutParameters = "{0} {1} {2} {3} \"{4}\" \"{5}\" \"{6}\" \"{7}\""; //operatorType, operatorPort, isFullLog, routing, inputSources, outputSources, output_op, replicas_url
			String syntaxFormatted;

            String logType = isFullLog ? "full" : "light";

            String input;
            String output;

			String replicas_url = String.Join(",", replicas_url_op);
			String output_op = String.Join(",", output_operator);

            if (inputSources == null)
            {
                input = "";
            }
            else
            {
                input = String.Join(",", inputSources);
            }

            if (outputSources == null)
            {
                output = "";
            }
            else
            {
                output = String.Join(",", outputSources);
            }

            if (operatorParams == null)
            {
                syntaxFormatted = String.Format(syntaxWithoutParameters, operatorType, operatorPort, logType, routing, input, output, output_op, replicas_url);
            }
            else
            {
                String args = String.Join(",", operatorParams);
                syntaxFormatted = String.Format(syntaxWithParameters, operatorType, operatorPort, logType, routing, input, output, output_op, replicas_url, args);
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(Constants.OPERATOR_EXE, syntaxFormatted);
            Process process = new Process();
            process.StartInfo = processStartInfo;

            try
            {
                if(process.Start())
                {
                    this.runningOperators.Add(operatorPort);

                    Console.WriteLine("Creating Operator " + operatorType + " on port " + operatorPort + " with following parameters " + syntaxFormatted);

                    return true;
                }

                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
