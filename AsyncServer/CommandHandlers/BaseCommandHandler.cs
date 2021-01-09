using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer.CommandHandlers
{
    public abstract class BaseCommandHandler : ICommandHandler
    {
        public Guid Guid { get; private set; }
        public int DataLength { get; private set; }

        public ClientConnection Connection { get; private set; }

        public async Task ProcessRequestAsync(TcpClient client, CancellationToken cancellationToken)
        {
            Connection = new ClientConnection(client);

            var stringGuid = await Connection.ReadStringAsync(Guid.NewGuid().ToString().Length, cancellationToken);
            Guid = new Guid(stringGuid);
            DataLength = await Connection.ReadInt32Async(cancellationToken);

            await Connection.WriteStringAsync("hello|", cancellationToken);

            await ProcessRequest(Connection, cancellationToken);            
        }

        protected abstract Task ProcessRequest(ClientConnection connection, CancellationToken cancellationToken);
    }
}
