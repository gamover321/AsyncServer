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
            var client = new Client("127.0.0.1", 8443, guid, cancellationToken);            
            client.Ping().GetAwaiter().GetResult();            
            client.GetDate().GetAwaiter().GetResult();

            var filePath = @"C:\git\aida.PNG";
            client.UploadFile(filePath).GetAwaiter().GetResult();

            Console.ReadKey();
        }     
    }
}
