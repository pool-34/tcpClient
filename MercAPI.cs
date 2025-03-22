
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MercAPI
{
    public class TcpSocket
    {
        private bool   _debug;
        private int    _port;
        private string _host;
        private string _message;
        private string _emessage;
        private string _answer;
        public TcpSocket(string host, int port, bool debug = false)
        { 
            _debug    = debug;
            _host     = host; 
            _port     = port;
            _answer   = "";
            _message  = "";
            _emessage = "";
        }
        public string Host      { get =>  _host;    set => _host    = value; }
        public int    Port      { get =>  _port;    set => _port    = value; }
        public bool   Debag     { get =>  _debug;   set => _debug   = value; }
        public string Message   { get =>  _message; set => _message = value; }
        public string Answer    { get =>  _answer; }
        public string eMessage  { get =>  _emessage; }
        public bool Send(string message = "")
        {
            _message = message;
            if (_port > 0)
            {
                if (!string.IsNullOrEmpty(_host))
                {
                    if (!string.IsNullOrEmpty(_message))
                    {
                        byte[] bsize = BitConverter.GetBytes(_message.Length);
                        byte[] bmsg = Encoding.UTF8.GetBytes(_message);
                        Array.Reverse(bsize);
                        byte[] bdata = bsize.Concat(bmsg).ToArray();
                        IPEndPoint _endpoint = new(IPAddress.Parse(_host), _port);
                        Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        try
                        {
                            _socket.Connect(_endpoint);
                            _socket.Send(bdata);
                            var _buffer = new byte[1024];
                            var _str = new StringBuilder();
                            do
                            {
                                int _size = _socket.Receive(_buffer);
                                _str.Append(Encoding.UTF8.GetString(_buffer, 0, _size));
                            }
                            while (_socket.Available > 0);
                            _socket.Shutdown(SocketShutdown.Both);
                            _socket.Close();
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
    }
}
