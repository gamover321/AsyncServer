using AsyncServer.CommandHandlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    public class Server
    {
        private TimeSpan ReadTimeOut => TimeSpan.FromSeconds(5000);
        private const string MessageCaption = "hello|";

        private ConcurrentBag<TcpClient> ConnectionPool {get; set;} = new ConcurrentBag<TcpClient>();

        public async Task Start(CancellationToken cancellationToken)
        {
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8443);
            listener.Start();

            var t = new Thread(() => {
                while (true)
                {
                    Console.WriteLine($"---------------------------------------------------------");
                    Console.WriteLine($"--- Current connections: {ConnectionPool.Count}------------------------");
                    Console.WriteLine($"---------------------------------------------------------");                    
                    Thread.Sleep(5000);
                }
            });
            t.Start();


            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                ThreadPool.QueueUserWorkItem(async (item) => await ProcessClientAsync(client), null);
                //new Thread((item) => ProcessClientAsync(client)).Start();
            }          
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            try
            {
                ConnectionPool.Add(client);

                var cts = new CancellationTokenSource();
                cts.CancelAfter((int)ReadTimeOut.TotalMilliseconds);
                var token = cts.Token;

                var connection = new ClientConnection(client);

                var caption = await connection.ReadStringAsync(MessageCaption.Length, token);
                if (caption != MessageCaption)
                {
                    Console.WriteLine("Wrong request");
                    return;
                }

               
                var commandCode = (CommandCodeEn) await connection.ReadByteAsync(token);                
               
                if (commandCode == CommandCodeEn.Ping)
                {
                    var handler = new PingCommandHandler();
                    await handler.ProcessRequestAsync(client, token);                  
                }
                else if (commandCode == CommandCodeEn.GetDate)
                {
                    var handler = new GetDateCommandHandler();
                    await handler.ProcessRequestAsync(client, token);
                }
                else
                {
                    throw new Exception($"Invalid command code: {commandCode}");
                }

                var taken = ConnectionPool.TryTake(out client);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }         
            
        }  


    }
}