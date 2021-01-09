using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    public class Client
    {        
        private Guid Guid { get; set; }

        private int Port { get; set; }
        private string Ip { get; set; }      

        private CancellationToken CancellationToken { get; set; }    

        private string _messageCaption = "hello|";

        public Client(string ip, int port, Guid guid, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            Ip = ip;
            Port = port;
           

            Guid = guid;
        }

        public async Task Ping()
        {
            using var client = new TcpClient();
            client.Connect(Ip, Port);
            var connection = new ClientConnection(client);

            var commandData = "ping";
            var commandLength = commandData.Length+1;

            await WriteHeader(connection);
            await WriteCommandCode(connection, CommandCodeEn.Ping);
            await WriteGuid(connection);
            await WriteDataLength(connection, commandLength);
            await WriteString(connection, commandData);

            var caption = await connection.ReadStringAsync(CancellationToken);
            var pong = await connection.ReadStringAsync(CancellationToken);

            Console.WriteLine($"<- {caption}{pong}");
        }

        public async Task<DateTimeOffset> GetDate()
        {
            using var client = new TcpClient();
            client.Connect(Ip, Port);
            var connection = new ClientConnection(client);

            await WriteHeader(connection);
            await WriteCommandCode(connection, CommandCodeEn.GetDate);
            await WriteGuid(connection);
            await WriteDataLength(connection, 0);
          
            var caption = await connection.ReadStringAsync(CancellationToken);
            var unixDate = await connection.ReadInt64Async(CancellationToken);

            var date = DateTimeOffset.FromUnixTimeSeconds(unixDate);

            Console.WriteLine($"<- {caption}{date}");

            return date;
        }

        public async Task UploadFile(string filePath)
        {
            using var client = new TcpClient();
            client.Connect(Ip, Port);
            var connection = new ClientConnection(client);

            var fileName = filePath.Split('\\').Last();
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            await WriteHeader(connection);
            await WriteCommandCode(connection, CommandCodeEn.PutFile);
            await WriteGuid(connection);
            await WriteDataLength(connection, fileBytes.Length+fileName.Length+1);
            await connection.WriteStringAsync(fileName, CancellationToken);
            await connection.WriteBytesAsync(fileBytes, CancellationToken);

            var caption = await connection.ReadStringAsync(CancellationToken);
            var savedFileName = await connection.ReadStringAsync(CancellationToken);
            Console.WriteLine($"<- {caption}{savedFileName}");
        }

        private async Task WriteHeader(ClientConnection connection)
        {            
            await connection.WriteStringAsync(_messageCaption, CancellationToken);
        }

        private async Task WriteCommandCode(ClientConnection connection, CommandCodeEn commandCode)
        {           
            await connection.WriteByteAsync((byte)commandCode, CancellationToken);
        }

        private async Task WriteGuid(ClientConnection connection)
        {           
            await connection.WriteStringAsync(Guid.ToString(), CancellationToken);
        }

        private async Task WriteDataLength(ClientConnection connection, int dataLength)
        {
            await connection.WriteInt32Async(dataLength, CancellationToken);
        }

        private async Task WriteString(ClientConnection connection, string data)
        {
            await connection.WriteStringAsync(data, CancellationToken);
        }

    }
}
