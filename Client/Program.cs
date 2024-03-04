using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync("127.0.0.1", 8888);
                Console.WriteLine($"Подключение к {socket.RemoteEndPoint} установлено");

                while (true) 
                {
                    Console.WriteLine("Введите коэффициенты уравнения (a b c):");
                    string coefficients = Console.ReadLine();

                    byte[] messageBytes = Encoding.UTF8.GetBytes(coefficients);
                    socket.Send(messageBytes);

                    byte[] buffer = new byte[1024];
                    int receivedBytes = socket.Receive(buffer);
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                    Console.WriteLine("Результат от сервера: " + receivedMessage);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"Не удалось установить подключение с {socket?.RemoteEndPoint}");
            }
            finally
            {
                socket?.Close();
            }
        }
    }
}
