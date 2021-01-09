using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("press any key to start");
            Console.ReadKey();

            var cancellationToken = new CancellationTokenSource().Token;
            var server = new Server();
            server.Start(cancellationToken);
            Thread.Sleep(1000);
            Console.WriteLine("started");

            var guid = Guid.NewGuid();
            using (var client = new Client("127.0.0.1", 8443, guid, cancellationToken))
            {
                client.Ping().GetAwaiter().GetResult();
            }
            using (var client = new Client("127.0.0.1", 8443, guid, cancellationToken))
            {
                client.GetDate().GetAwaiter().GetResult();
            }
                       

            //while (true)
            //{
            //    var guid = Guid.NewGuid();
            //    var client = new Client("127.0.0.1", 8443, guid, cancellationToken);
            //    client.Ping().GetAwaiter().GetResult();
            //
            //    Thread.Sleep(3000);
            //}

            Console.ReadKey();
        }     
    }

    

    

 

    public class ConnectionSycnronizationContext : SynchronizationContext
    {
        public string Id { get; set; }
    }
}
