
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static MercAPI.Session;

namespace MercAPI
{
    public class TcpSocket(string host, int port)
    {
        private int _port = port;
        private bool _connected = false;
        private string _host = host;
        private Socket? _socket;
        private string _message = "";
        private string _emessage = "";
        private string _answer = "";

        public string Host { get => _host; set => _host = value; }
        public int Port { get => _port; set => _port = value; }
        public string Message { get => _message; set => _message = value; }
        public string Answer { get => _answer; }
        public string EMessage { get => _emessage; }
        public bool Connected { get => _connected; }
        public bool Open()
        {
            if (_port > 0)
            {
                if (!string.IsNullOrEmpty(_host))
                {
                    IPEndPoint _endpoint = new(IPAddress.Parse(_host), _port);
                    _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        _socket.Connect(_endpoint);
                        _connected = true;
                        return true;
                    }
                    catch (SocketException ex)
                    {
                        _emessage = ex.Message;
                        return false;
                    }
                }
                else
                {
                    _emessage = "Empty Host";
                    return false;
                }
            }
            else
            {
                _emessage = "Invalid Port";
                return false;
            }
        }
        public bool Send(string message = "")
        {
            _message = message;
            if (_connected)
            {
                if (!string.IsNullOrEmpty(_message))
                {
                    byte[] bsize = BitConverter.GetBytes(_message.Length);
                    byte[] bmsg = Encoding.UTF8.GetBytes(_message);
                    Array.Reverse(bsize);
                    byte[] bdata = bsize.Concat(bmsg).ToArray();
                    try
                    {
                        _socket.Send(bdata);
                        var _buffer = new byte[1024];
                        var _str = new StringBuilder();
                        do
                        {
                            int _size = _socket.Receive(_buffer);
                            _str.Append(Encoding.UTF8.GetString(_buffer, 0, _size));
                        }
                        while (_socket.Available > 0);
                        _answer = _str.ToString();
                    }
                    catch (SocketException ex)
                    {
                        _emessage = ex.Message;
                        return false;
                    }
                    return true;
                }
                else
                {
                    _emessage = "Empty Message";
                    return false;
                }
            }
            else
            {
                _emessage = "NOT Connected!";
                return false;
            }
        }
        public bool Close()
        {
            if (_connected)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _connected = false;
                    return true;
                }
                catch (SocketException ex)
                {
                    _emessage = ex.Message;
                    return false;
                }
            }
            else
            {
                _emessage = "NOT Connected!";
                return false;
            }
        }

    }

    public class DriverInfo
    {
        DriverRequest _request;
        static DriverAnswer _answer;

        public DriverRequest Request { get => _request; }
        public DriverAnswer Answer { get => _answer; }

        public DriverInfo()
        {
            _request = new DriverRequest(); 
            _answer = new DriverAnswer();
        }
        public class DriverRequest
        {
            string _command = "GetDriverInfo";
            public string Command { get => _command; set => _command = value; }

            public string Serialize()
            {
                JsonSerializerOptions options = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };
                return JsonSerializer.Serialize(this, options);
            }
        }

        public class DriverAnswer
        {
            public int Result { get; set; }
            public string? Description {  get; set; }
            public string? DriverVer { get; set; }
            public string? ProtocolVer {  get; set; }
            public string? DriverBaseVer {  get; set; }
            public string? DriverSalesVer {  get; set; }

            public void Deserialize(string jsonstring)
            {
                string ss = jsonstring.Substring(jsonstring.IndexOf('{'));
                JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                _answer = JsonSerializer.Deserialize<DriverAnswer>(ss, options);
            }

        }
    }

    public class Session
    {
        SessionRequest _request;
        SessionAnswer  _answer;
        string? _message;

        public SessionRequest Request { get => _request; set => _request = value; }
        public SessionAnswer Answer { get => _answer; set => _answer = value; }
        public string Message { get => _message; set => _message = value; }
        public Session() 
        {
            _request = new SessionRequest();
            _answer  = new SessionAnswer();
        }

        public void Deserialize(string jsonstring)
        {
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            _answer = JsonSerializer.Deserialize<SessionAnswer>(jsonstring, options);
        }

        public class SessionRequest
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public string? SessionKey { get; set; }
            public string Command { get; set; }
            public string PortName { get; set; }
            public int BoudRate { get; set; }
            public string Model { get; set; }
            public string SerialNumber { get; set; }
            public bool Debug { get; set; }
            public string LogPath { get; set; }
        }
        
        public class SessionAnswer
        {
            public int Result { get; set; }
            public string Description { get; set; }
            public string ProtocolVer { get; set; }
            public string FfdTotalVer { get; set; }
            public string ProgramDate { get; set; }
        }

    }
}

  