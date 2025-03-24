using MercAPI;
using System.Text.Encodings.Web;
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
            JsonSerializerOptions jsonOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };


            string? sessionKey = null;

            Session session = new();
            session.Request.SessionKey = null;
            session.Request.Command = "OpenSession";
            session.Request.PortName = "\\\\.\\COM3";
            session.Request.BoudRate = 115200;
            session.Request.Model = "185F";
            session.Request.Debug = true;
            session.Request.LogPath = "C:\\Log.log";

            DriverInfo driverinfo = new();

            MercAPI.TcpSocket tcpSocket = new MercAPI.TcpSocket(ip, port);

            if (tcpSocket.Open())
            {
                //if (tcpSocket.Send(Json.Serialize(driverinfo.Request)))
                if (tcpSocket.Send(JsonSerializer.Serialize(session.Request, jsonOptions)))
                {
                    Console.WriteLine(tcpSocket.Message);
                    Console.WriteLine(tcpSocket.Answer);

                    //string ss = tcpSocket.Answer.Substring(tcpSocket.Answer.IndexOf('{'));
                    //driverinfo.Answer = JsonSerializer.Deserialize<DriverInfo.DriverAnswer>(ss, jsonOptions);
                    string ss = tcpSocket.Answer.Substring(tcpSocket.Answer.IndexOf('{'));
                    session.Answer = JsonSerializer.Deserialize<Session.SessionAnswer>(ss, jsonOptions);

                    sessionKey = session.Answer.SessionKey;

                    Console.WriteLine($"result: {session.Answer.Result}");
                    Console.WriteLine($"sessionKey: {sessionKey}");
                    Console.WriteLine($"protocolVer: {session.Answer.ProtocolVer}");
                    //Console.WriteLine($"result: {driverinfo.Answer.Result}");
                    //Console.WriteLine($"driverVer: {driverinfo.Answer.DriverVer}");
                    //Console.WriteLine($"protocolVer: {driverinfo.Answer.ProtocolVer}");
                    //Console.WriteLine($"driverBaseVer: {driverinfo.Answer.DriverBaseVer}");

                    if (session.Answer.Result == 0)
                    {
                        string sc = $"{{\"sessionKey\":\"{sessionKey}\",\"command\":\"PrintText\", \"text\":\"*** TEST TEST TEST ***\", \"forcePrint\":true}}";
                        Console.WriteLine(sc);
                        
                        for (int i = 0; i < 5; i++) tcpSocket.Send(sc);
                        
                        Session closeSession = new Session();
                        closeSession.Request.SessionKey = sessionKey;
                        closeSession.Request.Command = "CloseSession";
                        tcpSocket.Send(JsonSerializer.Serialize(closeSession.Request, jsonOptions));

                        Console.WriteLine(tcpSocket.Message);
                        Console.WriteLine(tcpSocket.Answer);
                    }
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
