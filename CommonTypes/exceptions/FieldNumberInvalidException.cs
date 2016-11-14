using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.exceptions
{
    class FieldNumberInvalidException : ApplicationException
    {
        private int value;
        public FieldNumberInvalidException(int fieldNumber)
        {
            value = fieldNumber;
        }

        public string getMessage()
        {
            return "FieldNumberInvalidException: Field Number " + value + " is invalid.";
        }
    }
}
