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

					for (int i = 0; i < replicas_url.Length; i++) {
						if (String.Compare(op_url, replicas_url[i]) != 0) {
							Uniq count_op = (Uniq)Activator.GetObject(typeof(Uniq), replicas_url[i]);

							RemoteAsyncDelegateTuple RemoteDel = new RemoteAsyncDelegateTuple(count_op.updateUniq);
							IAsyncResult RemAr = RemoteDel.BeginInvoke(result, null, null);
						}
					}
				}
                else throw new Exception("Invalid Argument Type");
            }
            else throw new Exception("Invalid Argument Count");
        }

		public void updateUniq(Tuple new_input) {
			tupleList.Add(new_input);
		}
	}
}
