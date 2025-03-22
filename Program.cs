using MercAPI;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

            MercAPI.TcpSocket tcpSocket = new MercAPI.TcpSocket(ip, port);

            if (tcpSocket.Open())
            {
                var mess = @"{""command"": ""GetDriverInfo""}";

                if (tcpSocket.Send(mess))
                {
                    Console.WriteLine(tcpSocket.Host);
                    Console.WriteLine(tcpSocket.Port);
                    Console.WriteLine(tcpSocket.Message);
                    Console.WriteLine(tcpSocket.Answer);

                    //OpenSession openSession = new OpenSession();
                    OpenSession.Request request = new OpenSession.Request();
                    request.SessionKey = null;
                    request.Command = "OpenSession";
                    request.PortName = "\\\\.\\COM3";
                    request.BoudRate = 115200;
                    request.Debug = true;
                    request.LogPath = "C:\\Log.log";

                    JsonSerializerOptions jsonopt = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    //{
                    //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                    //};

                    string jsonstr = JsonSerializer.Serialize(request, jsonopt);

                    Console.WriteLine(jsonstr);

                    string strjson = @"{""result"":0,""sessionKey"":""143001-25882"",""protocolVer"":""3.11"",""programDate"":""2024-12-25"",""ffdTotalVer"":""1.2""}";

                    OpenSession.Answer answer = new OpenSession.Answer();

                    answer = JsonSerializer.Deserialize<OpenSession.Answer>(strjson);

                    Console.WriteLine(answer.Result.ToString());
                    Console.WriteLine(answer.SessionKey);
                    Console.WriteLine(answer.ProtocolVer);
                    Console.WriteLine(answer.ProgramDate);
                    Console.WriteLine(answer.FfdTotalVer);
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
