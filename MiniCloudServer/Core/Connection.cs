using System;
using System.Net.Sockets;
using System.Text;

namespace MiniCloudServer.Core
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

            if (text.Length > 133)
                Console.WriteLine($"Response: {text.Substring(33, 133)}...");
            else
                Console.WriteLine("Response: " + text.Substring(33));
            byte[] data = Encoding.ASCII.GetBytes(text);
            Socket.Send(data);
        }
    }
}
