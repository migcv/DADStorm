using System.Collections.Generic;

namespace CommonTypes.operators
{
    public class Dup : RemoteOperator
    {
        public Dup(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] output_op, string[] replicas_op)
            : base("DUP", inputSources, outputSources, routing, logLevel, output_op, replicas_op)
        {
        }

        public override void doOperation(Tuple input)
        {
            result = input;
        }
    }
}
