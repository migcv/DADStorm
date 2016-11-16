using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.operators
{
    public class Count : RemoteOperator
    {
        private int tupleCounter;

        public Count(string[] inputSources, string[] outputSources, string routing, bool logLevel)
            : base("COUNT", inputSources, outputSources, routing, logLevel)
        {
            this.tupleCounter = 0;
        }

        public override void doOperation(Tuple input)
        {
            this.tupleCounter++;
            this.result = Tuple.fromString(this.tupleCounter.ToString());
        }
    }
}
