using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;
using MiniCloudGUI;

namespace MultiClient
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 100;
        private const int BUFFER_SIZE = 50 * 1000 * 1000;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        static void Main()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }

        private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");

            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            Console.Write("Send a request: ");
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        private static void SendString(string text)
        {
            var md5request = $"{CheksumGenerator.CreateMD5(text)} {text}";
            byte[] buffer = Encoding.ASCII.GetBytes(md5request);
            ClientSocket.Send(buffer);
        }

        private static void ReceiveResponse()
        {
            string text;
            do {
                int received = ClientSocket.Receive(buffer, SocketFlags.None);
                if (received == 0) 
                    return;
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                text = Encoding.ASCII.GetString(data);
                var cleanText = text.Substring(33);
                var md5 = text.Substring(0, 32);
                if (CheksumGenerator.CreateMD5(cleanText) != md5)
                {
                    Console.WriteLine("Wystapil blad polaczenia");
                    continue;
                }
                Console.WriteLine(cleanText);
                text=cleanText;
            }while(!text.StartsWith("_OK_") && !text.StartsWith("_ERROR_"));
            
        }
    }
}
