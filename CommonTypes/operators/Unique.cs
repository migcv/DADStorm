using System;
using System.Collections.Generic;

namespace CommonTypes.operators
{
    public class Uniq : RemoteOperator
    {
        private List<Tuple> tupleList;

        public Uniq() { }

        public Uniq(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] parameters)
            : base("UNIQ", inputSources, outputSources, routing, logLevel, parameters)
        {
            this.tupleList = new List<Tuple>();
        }

        public override void doOperation()
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
