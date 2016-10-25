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

        public Boolean createOperator(String operatorType, int operatorPort, String[] inputSources, String[] outputSources, params Object[] operatorParams)
        {
            String parameters = String.Join(" ", operatorParams);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(Constants.OPERATOR_EXE, parameters);
            Process process = new Process();
            process.StartInfo = processStartInfo;

            try
            {
                if(process.Start())
                {
                    this.runningOperators.Add(operatorPort);

                    Console.WriteLine("Creating Operator " + operatorType + " on port " + operatorPort + " with following parameters " + parameters);

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
