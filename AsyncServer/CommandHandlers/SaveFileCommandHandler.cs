using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer.CommandHandlers
{
    public class SaveFileCommandHandler : BaseCommandHandler
    {
        protected override async Task ProcessRequest(ClientConnection connection, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{Guid}] -> GetFile ({DataLength}b)");

            var fileName = await Connection.ReadStringAsync(cancellationToken);
            var fileBytes = await Connection.ReadBytesAsync(DataLength-fileName.Length-1, cancellationToken);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            var filePath = Path.Combine(path, fileName);
            await File.WriteAllBytesAsync(filePath, fileBytes);
            
            await connection.WriteStringAsync(fileName, cancellationToken);
        }
    }
}
