using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer.CommandHandlers
{
    public interface ICommandHandler
    {
        Task ProcessRequestAsync(TcpClient client, CancellationToken cancellationToken);
    }
}
