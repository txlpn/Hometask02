using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static int equationCount = 0; 

        static void Main(string[] args)
        {
            Task serverTask = StartServer();
            serverTask.Wait();
        }

        static async Task StartServer()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipPoint);
            listener.Listen(10); 

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true) 
            {
                Socket handler = await listener.AcceptAsync(); 
                Task.Run(() => HandleClient(handler)).Wait(); 
            }
        }

        static void HandleClient(Socket clientSocket)
        {
            int clientId = Interlocked.Increment(ref equationCount); 
            Console.WriteLine($"Клиент подключен. ID клиента: {clientId}");

            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string clientMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            string[] coefficients = clientMessage.Split(' ');
            double a = double.Parse(coefficients[0]);
            double b = double.Parse(coefficients[1]);
            double c = double.Parse(coefficients[2]);

            double discriminant = b * b - 4 * a * c;
            string response;
            if (discriminant < 0)
            {
                response = "Корней нет.";
            }
            else
            {
                double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                response = $"x1 = {x1}, x2 = {x2}";
            }

            clientSocket.Send(Encoding.UTF8.GetBytes(response)); 
            Console.WriteLine($"Решено уравнений: {equationCount}");
            clientSocket.Close(); 
        }
    }
}
