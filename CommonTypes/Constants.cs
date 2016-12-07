using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
	public delegate void RemoteAsyncDelegateTuple(Tuple input);

	public delegate void RemoteAsyncDelegate();

	public delegate void RemoteAsyncDelegateInt(int integer);

	public delegate void RemoteAsyncDelegateString(string str);

	public delegate void RemoteAsyncDelegateStart(string id, string url, string semantic);

	public delegate void RemoteAsyncDelegateLog(string id, string type, string url, string log);

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
