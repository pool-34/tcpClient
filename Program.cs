using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 50009;

            MercAPI.TcpSocket tcpSocket = new MercAPI.TcpSocket(ip, port);

            //tcpSocket.Message = @"{""command"": ""GetDriverInfo""}";
            
            tcpSocket.Send(@"{""command"": ""GetDriverInfo""}");


            Console.WriteLine(tcpSocket.Host);
            Console.WriteLine(tcpSocket.Port);
            Console.WriteLine(tcpSocket.Message);
            Console.WriteLine(tcpSocket.Answer);
        }

    }
}
