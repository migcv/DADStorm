using CommonTypes;
using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            RemotePCS obj = (RemotePCS)Activator.GetObject(typeof(RemotePCS), "tcp://localhost:10000/RemotePCS");
            obj.createOperator("COUNT", 10010, true, "primary", null, new String[] { "tcp://localhost:10011/op" }, null);
            obj.createOperator("DUP", 10011, true, "primary", null, null, null);

            RemoteOperator count = (RemoteOperator)Activator.GetObject(typeof(RemoteOperator), "tcp://localhost:10010/op");

            CommonTypes.Tuple t1 = CommonTypes.Tuple.fromString("1,2,3");
            CommonTypes.Tuple t2 = CommonTypes.Tuple.fromString("1,2,2");
            CommonTypes.Tuple t3 = CommonTypes.Tuple.fromString("1,3,3");
            CommonTypes.Tuple t4 = CommonTypes.Tuple.fromString("1,4,1");

            System.Threading.Thread.Sleep(4000);

            count.process(t1);

            Console.ReadLine();
        }
    }
}
