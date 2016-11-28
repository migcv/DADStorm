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

	public delegate void RemoteAsyncDelegateStart(string id, string url);

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

		public static void OurRemoteAsyncCallBack(IAsyncResult ar) {
			// Alternative 2: Use the callback to get the return value
			RemoteAsyncDelegateTuple del = (RemoteAsyncDelegateTuple)((AsyncResult)ar).AsyncDelegate;
			return;
		}
	}
}
