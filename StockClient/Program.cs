namespace StockDBApp;
public class Program
{
    public static void Main()
    {
        StockClient client = new StockClient("127.0.0.1", 8888);

        Console.Write("Введите тикер акции: ");
        string ticker = Console.ReadLine();

        client.RequestStockPrice(ticker.ToUpper());
    }
}
