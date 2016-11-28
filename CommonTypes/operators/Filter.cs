using CommonTypes.exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.operators
{
    public class Filter : RemoteOperator
    {
        public Filter(string[] inputSources, string[] outputSources, string routing, bool logLevel, string[] output_op, string[] replicas_op, string[] parameters)
            : base("FILTER", inputSources, outputSources, routing, logLevel, output_op, replicas_op, parameters)
        {
        }

        public override void doOperation(Tuple input)
        {
            int fieldNumber = Int32.Parse(this.parameters[0]) - 1;
            string condition = this.parameters[1];
            string value = this.parameters[2];

            if (fieldNumber < 0 || input.length() < fieldNumber)
            {
                throw new FieldNumberInvalidException(fieldNumber);
            }
            switch (condition)
            {
                case "=":
                    if (input.getField(fieldNumber).Equals("\""+value+"\""))
                    {
                        result = input;
                    }
                    break;
                case "<":
                    if (Int32.Parse(input.getField(fieldNumber)) < Int32.Parse(value))
                    {
                        result = input;
                    }
                    break;
                case ">":
                    if (Int32.Parse(input.getField(fieldNumber)) > Int32.Parse(value))
                    {
                        result = input;
                    }
                    break;
            }
        }
    }
}
