using MercAPI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace tcpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 50009;
            //string? sessionKey = null;

            //Session session = new();
            //session.Request.SessionKey = null;
            //session.Request.Command = "OpenSession";
            //session.Request.PortName = "\\\\.\\COM3";
            //session.Request.BoudRate = 115200;
            //session.Request.Model = "185F";
            //session.Request.Debug = true;
            //session.Request.LogPath = "C:\\Log.log";

            DriverInfo driverinfo = new();

            MercAPI.TcpSocket tcpSocket = new MercAPI.TcpSocket(ip, port);

            if (tcpSocket.Open())
            {
                if (tcpSocket.Send(driverinfo.Request.Serialize()))
                {
                    Console.WriteLine(tcpSocket.Message);
                    Console.WriteLine(tcpSocket.Answer);

                    driverinfo.Answer.Deserialize(tcpSocket.Answer);

                    Console.WriteLine($"result: {driverinfo.Answer.Result}");
                    Console.WriteLine($"driverVer: {driverinfo.Answer.DriverVer}");
                    Console.WriteLine($"protocolVer: {driverinfo.Answer.ProtocolVer}");
                    Console.WriteLine($"driverBaseVer: {driverinfo.Answer.DriverBaseVer}");
                }
                else
                {
                    Console.WriteLine("Ошибка!");
                    Console.WriteLine(tcpSocket.EMessage);
                }

                if (tcpSocket.Connected) tcpSocket.Close();
            }
        }

    }
}
