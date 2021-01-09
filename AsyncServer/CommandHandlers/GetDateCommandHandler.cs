using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer.CommandHandlers
{
    public class GetDateCommandHandler : BaseCommandHandler
    {
        protected override async Task ProcessRequest(ClientConnection connection, CancellationToken cancellationToken)
        {
            var date = DateTimeOffset.Now;
            var unixDate = date.ToUnixTimeSeconds();

            Console.WriteLine($"[{Guid}] -> GetDate ({DataLength})");

            
            await connection.WriteInt64Async(unixDate, cancellationToken);
        }
    }
}
