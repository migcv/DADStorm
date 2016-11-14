using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class Tuple
    {
        private String[] tupleContent;

        public Tuple(String[] tupleContent)
        {
            this.tupleContent = tupleContent;
        }

        public static Tuple fromString(String tupleStr)
        {
            String[] tupleContent = tupleStr.Split(Constants.TUPLE_SEPARATOR).Select(a => a.Trim()).ToArray();

            return new Tuple(tupleContent);
        }

        public String getField(int index)
        {
            return (String)this.tupleContent.GetValue(index);
        }

        public override string ToString()
        {
            return String.Join(Constants.TUPLE_SEPARATOR + " ", this.tupleContent);
        }

        public int length()
        {
            return tupleContent.Length;
        }
    }
}
