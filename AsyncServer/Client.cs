using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    public class Client : IDisposable
    {
        private TcpClient TcpClient { get; set; }
        private Guid Guid { get; set; }

        private CancellationToken CancellationToken { get; set; }

        private ClientConnection Connection { get; set; }

        private string _messageCaption = "hello|";

        public Client(string ip, int port, Guid guid, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            TcpClient = new TcpClient();
            TcpClient.Connect(ip, port);

            Connection = new ClientConnection(TcpClient);

            Guid = guid;
        }

        public async Task Ping()
        {
            var commandData = "ping";
            var commandLength = commandData.Length;

            await WriteHeader();
            await WriteCommandCode(CommandCodeEn.Ping);
            await WriteGuid();            
            await WriteDataLength(commandLength);
            await WriteString(commandData);

            var caption = await Connection.ReadStringAsync(_messageCaption.Length, CancellationToken);
            var pong = await Connection.ReadStringAsync("pong".Length, CancellationToken);

            Console.WriteLine($"<- {caption}{pong}");
        }

        public async Task GetDate()
        {
            await WriteHeader();
            await WriteCommandCode(CommandCodeEn.GetDate);
            await WriteGuid();
            await WriteDataLength(0);
          
            var caption = await Connection.ReadStringAsync(_messageCaption.Length, CancellationToken);
            var unixDate = await Connection.ReadInt64Async(CancellationToken);

            var date = DateTimeOffset.FromUnixTimeSeconds(unixDate);

            Console.WriteLine($"<- {caption}{date}");
        }

        public void Dispose()
        {            
            TcpClient?.Close();
        }

        private async Task WriteHeader()
        {
            var bytes= Encoding.UTF8.GetBytes(_messageCaption);
            await Connection.WriteBytesAsync(bytes, CancellationToken);
        }

        private async Task WriteCommandCode(CommandCodeEn commandCode)
        {           
            await Connection.WriteByteAsync((byte)commandCode, CancellationToken);
        }

        private async Task WriteGuid()
        {
            var bytes = Encoding.UTF8.GetBytes(Guid.ToString());
            await Connection.WriteBytesAsync(bytes, CancellationToken);
        }

        private async Task WriteDataLength(int dataLength)
        {
            await Connection.WriteInt32Async(dataLength, CancellationToken);
        }

        private async Task WriteString(string data)
        {
            await Connection.WriteStringAsync(data, CancellationToken);
        }

        private byte[] Append(byte[] buffer, byte[] data)
        {
            var newBuffer = new byte[buffer.Length + data.Length];
            Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
            Array.Copy(data, 0, newBuffer, buffer.Length, data.Length);

            return newBuffer;
        }
    }
}
