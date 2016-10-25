using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            RemotePCS obj = (RemotePCS)Activator.GetObject(typeof(RemotePCS), "tcp://localhost:10000/RemotePCS");
            Console.Write(obj.createOperator("test", 1000, null, null, "123", 456));
            Console.ReadLine();
        }
    }
}
