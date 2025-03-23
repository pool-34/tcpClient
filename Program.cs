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

            //Session2 session2 = new Session2();
            //session2.Request2.SessionKey = null;
            //session2.Request2.Command = "ERRERRER";

            //Console.WriteLine($"Session2.Request2.Command = {session2.Request2.Command}");

            //Session3 openSession = new();
            //Session3.Request request = new()
            //{
            //    SessionKey = null,
            //    Command = "OpenSession",
            //    PortName = "\\\\.\\COM3",
            //    BoudRate = 115200,
            //    Debug = true,
            //    LogPath = "C:\\Log.log"
            //};

            //openSession.Serialize(request);

            Session session = new();
            session.Request.SessionKey = null;
            session.Request.Command = "OpenSession";
            session.Request.PortName = "\\\\.\\COM3";
            session.Request.BoudRate = 115200;
            session.Request.Model = "185F";
            session.Request.Debug = true;
            session.Request.LogPath = "C:\\Log.log";

            session.Serialize();

            Console.WriteLine(session.Message);



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

                    //Console.WriteLine(openSession.Message);

                    //string strjson = @"{""result"":0,""sessionKey"":""143001-25882"",""protocolVer"":""3.11"",""programDate"":""2024-12-25"",""ffdTotalVer"":""1.2""}";

                    //Session3.Answer answer = new();

                    //openSession.Deserialize(answer, strjson);

                    //JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                    //answer = JsonSerializer.Deserialize<Session.Answer>(strjson, options);

                    //Console.WriteLine(answer.Result.ToString());
                    //Console.WriteLine(answer.SessionKey);
                    //Console.WriteLine(answer.ProtocolVer);
                    //Console.WriteLine(answer.ProgramDate);
                    //Console.WriteLine(answer.FfdTotalVer);
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
