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

        public Count(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] output_op, string[] replicas_op)
            : base("COUNT", inputSources, outputSources, routing, logLevel, output_op, replicas_op)
        {
            this.tupleCounter = 0;
        }

        public override void doOperation(Tuple input)
        {
            this.tupleCounter++;
            this.result = Tuple.fromString(this.tupleCounter.ToString());
			for (int i = 0; i < replicas_url.Length; i++) {
				if (String.Compare(op_url, replicas_url[i]) != 0) {
					Count count_op = (Count)Activator.GetObject(typeof(Count), replicas_url[i]);

					RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(count_op.updateCount);
					IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
				}
			}
		}

		public void updateCount() {
			tupleCounter++;
		}
    }
}
