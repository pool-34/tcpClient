
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MercAPI
{
    public class TcpSocket
    {
        private int _port;
        private string _host;
        private string _command;
        private string _answer;
        public TcpSocket(string host, int port)
        { 
            _host = host; 
            _port = port;
            _answer = "";
            _command = "";
        }
        public string Host { get => _host; set => _host = value; }
        public int Port { get => _port; set => _port = value; }
        public string Message { get =>  _command; set => _command = value;}
        public string Answer { get => _answer; }
        public bool Send(string message)
        {
            _command = message;
            if (_port > 0)
            {
                if (!string.IsNullOrEmpty(_host))
                {
                    if (!string.IsNullOrEmpty(_command))
                    {
                        byte[] bsize = BitConverter.GetBytes(_command.Length);
                        byte[] bmsg  = Encoding.UTF8.GetBytes(_command);
                        Array.Reverse(bsize);
                        var _data = bsize.Concat(bmsg);
                        byte[] bdata = _data.ToArray();
                        IPEndPoint _endpoint    = new(IPAddress.Parse(_host), _port);
                        Socket _socket          = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _socket.Connect(_endpoint);
                        _socket.Send(bdata);

                        var _buffer = new byte[1024];
                        var _str    = new StringBuilder();

                        do
                        {
                            int _size = _socket.Receive(_buffer);
                            _str.Append(Encoding.UTF8.GetString(_buffer, 0, _size));
                        }
                        while (_socket.Available > 0);

                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();

                        _answer = _str.ToString();

                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;

        }

    }
}
