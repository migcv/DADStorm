using CommonTypes;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Constants.PCS_LISTENING_PORT);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotePCS), "RemotePCS", WellKnownObjectMode.Singleton);

            Console.WriteLine("Process Creation Service Started");
            Console.ReadLine();
        }
    }
}
