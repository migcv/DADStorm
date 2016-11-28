using System;
using System.Collections.Generic;

namespace CommonTypes.operators
{
    public class Uniq : RemoteOperator
    {
        private List<Tuple> tupleList;

        public Uniq(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] output_op, string[] replicas_op, string[] parameters)
            : base("UNIQ", inputSources, outputSources, routing, logLevel, output_op, replicas_op, parameters)
        {
            this.tupleList = new List<Tuple>();
        }

        public override void doOperation(Tuple input)
        {
            if (parameters.Length == 1)
            {
                int fieldNumber;

                if (Int32.TryParse(parameters[0], out fieldNumber))
                {
                    int uniqueCounter = 0;

                    this.tupleList.Add(input);

                    foreach (Tuple t in this.tupleList)
                    {
                        if (t.getField(fieldNumber).Equals(input.getField(fieldNumber)))
                        {
                            uniqueCounter++;
                        }

                        if (uniqueCounter > 1)
                        {
                            return;
                        }
                    }

                    result = input;
                }
                else throw new Exception("Invalid Argument Type");
            }
            else throw new Exception("Invalid Argument Count");
        }
    }
}
