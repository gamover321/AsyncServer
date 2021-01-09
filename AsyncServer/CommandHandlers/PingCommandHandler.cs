using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer.CommandHandlers
{
    public class PingCommandHandler : BaseCommandHandler
    {
        protected override async Task ProcessRequest(ClientConnection connection, CancellationToken cancellationToken)
        {
            var ping = await Connection.ReadStringAsync(cancellationToken);           

            Console.WriteLine($"[{Guid}] -> {ping} ({DataLength})");         
            await connection.WriteStringAsync("pong", cancellationToken);
        }
    }
}
