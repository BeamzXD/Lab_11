using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockDBApp.Data;
using StockDBApp.Models;

namespace StockDBApp
{
    public class StockServer
    {
        private readonly int _port;

        public StockServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine($"Сервер запущен на порту {_port}...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[256];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string ticker = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            Console.WriteLine($"Получен запрос на тикер: {ticker}");

            string response = await GetLatestPriceAsync(ticker);
            byte[] responseData = Encoding.UTF8.GetBytes(response);

            await stream.WriteAsync(responseData, 0, responseData.Length);
            client.Close();
        }

        private async Task<string> GetLatestPriceAsync(string ticker)
        {
            using var context = new StockContext();
            var latestPrice = await context.Prices
                .Where(p => p.Ticker == ticker)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();

            if (latestPrice != null)
            {
                return $"Последняя цена для {ticker}: {latestPrice.AveragePrice} (дата: {latestPrice.Date})";
            }
            else
            {
                return $"Тикер {ticker} не найден в базе данных.";
            }
        }
    }
}
