using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using MiniCloudServer.Core;

namespace MiniCloudServer
{
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static readonly List<Connection> connections = new List<Connection>();
        private const int BUFFER_SIZE = 50 * 1000 * 1000;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;
            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            var client = new Connection(socket);
            connections.Add(client);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            var client = IdentifyClientByConnection(AR.AsyncState as Socket);
            int received;
            try
            {
                received = client.Socket.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                client.Socket.Close();
                connections.Remove(client);
                return;
            }
            string text = GetReceivedText(received);
            
            if(text.Length>133)
                Console.WriteLine($"Request: {text.Substring(33, 100)}...");
            else
                Console.WriteLine("Request: " + text.Substring(33));
            var requestHandler = new RequestHandler(client);
            requestHandler.Handle(text).Wait();
            BeginReceive(client.Socket);
        }
        private static string GetReceivedText(int length)
        {
            byte[] recBuf = new byte[length];
            Array.Copy(buffer, recBuf, length);
            string text = Encoding.ASCII.GetString(recBuf);
            return text;
        }
        private static void BeginReceive(Socket socket)
        {
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
        }

        private static Connection IdentifyClientByConnection(Socket connection)
        {
            return connections.SingleOrDefault(x => x.Socket == connection);
        }

    }
}
