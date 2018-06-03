using MiniCloudServer.Core;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MiniCloud.Core
{
    public class Connection
    {
        public Session Session { get; set;}
        public Socket Socket { get; set; }

        public Connection(Socket socket)
        {
            Socket = socket;
            Session=new Session();
        }
        public void SendText(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            Socket.Send(data);
        }
    }
}
