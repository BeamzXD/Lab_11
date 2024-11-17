using System;
using System.Net.Sockets;
using System.Text;

namespace StockDBApp
{
    public class StockClient
    {
        private readonly string _server;
        private readonly int _port;

        public StockClient(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public void RequestStockPrice(string ticker)
        {
            using TcpClient client = new TcpClient(_server, _port);
            NetworkStream stream = client.GetStream();

            byte[] requestData = Encoding.UTF8.GetBytes(ticker);
            stream.Write(requestData, 0, requestData.Length);

            byte[] buffer = new byte[256];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Ответ от сервера: {response}");
        }
    }
}
