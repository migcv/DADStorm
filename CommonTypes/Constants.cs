using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public static class Constants
    {
        public const String OPERATOR_EXE = @"..\..\..\Operator\bin\Debug\Operator.exe";
        //public const String OPERATOR_EXE = @"Operator.exe";
        public const Char TUPLE_SEPARATOR = ',';

		// Operator states
		public const string STATE_RUNNING = "RUNNING";
		public const string STATE_WAITING = "WAITING";
		public const string STATE_FREEZED = "FREEZED";
	}
}
