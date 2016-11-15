using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
	public delegate void RemoteAsyncDelegateProcess(Tuple input);

	public delegate void RemoteAsyncDelegate();


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
			RemoteAsyncDelegateProcess del = (RemoteAsyncDelegateProcess)((AsyncResult)ar).AsyncDelegate;
			return;
		}
	}
}
