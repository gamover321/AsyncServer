using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    public class ClientConnection
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private int _offset = 0;

        public bool CanRead => _stream.CanRead;
        public bool DataAvailable => _stream.DataAvailable;

        public int TotalReaded => _offset;

        public ClientConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void Close()
        {
            _client.Close();
        }

        public async Task<byte> ReadByteAsync(CancellationToken token)
        {
            return (await ReadBytesAsync(1, token))[0];
        }

        public async Task WriteByteAsync(byte data, CancellationToken token)
        {
            var bytes = new byte[1] { data };
            await WriteBytesAsync(bytes, token);
        }

        public async Task<int> ReadInt32Async(CancellationToken token)
        {     
            var result = await ReadBytesAsync(4, token);        
            return BitConverter.ToInt32(result);
        }
        public async Task WriteInt32Async(int number, CancellationToken token)
        {
            var bytes = GetBytes(number);
            await WriteBytesAsync(bytes, token);
        }

        public async Task<long> ReadInt64Async(CancellationToken token)
        {
            var result = await ReadBytesAsync(8, token);
            return BitConverter.ToInt64(result);
        }
        public async Task WriteInt64Async(long number, CancellationToken token)
        {
            var bytes = GetBytes(number);
            await WriteBytesAsync(bytes, token);
        }

        public async Task<string> ReadStringAsync(int stringLength, CancellationToken token)
        {
            return GetString(await ReadBytesAsync(stringLength, token));
        }

        public async Task WriteStringAsync(string str, CancellationToken token)
        {
            var bytes = GetBytes(str);
            await WriteBytesAsync(bytes, token);
        }

        public async Task<byte[]> ReadBytesAsync(int count, CancellationToken token)
        {
            var result = await ReadBytesAsync(_stream, count, token);
            _offset += count;
            return result;
        }

        public async Task WriteBytesAsync(byte[] bytes, CancellationToken token)
        {
            await WriteBytesAsync(_stream, bytes, token);           
        }

        private async Task<byte[]> ReadBytesAsync(NetworkStream stream, int count, CancellationToken token)
        {
            var totalReaded = 0;
           
            var result = new byte[count];

            while (totalReaded < count)
            {
                var data = new byte[count];
                var readed = await stream.ReadAsync(data, 0, count-totalReaded, token);
               
                Array.Copy(data, 0, result, totalReaded, readed);
                totalReaded += readed;
            }

            return result;
        }

        public async Task WriteBytesAsync(NetworkStream stream, byte[] bytes, CancellationToken token)
        {
            await stream.WriteAsync(bytes, 0, bytes.Length, token);
        }

        private string GetString(byte[] bytes)
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            return message;
        }

        private byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private byte[] GetBytes(int number)
        {
            return BitConverter.GetBytes(number);
        }

        private byte[] GetBytes(long number)
        {
            return BitConverter.GetBytes(number);
        }

    }
}
