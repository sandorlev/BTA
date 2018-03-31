using System;
using System.Net;
using System.Net.Sockets;

namespace BTAClient
{
    public class Client
    {
        private static BTAServer.Location location = new BTAServer.Location();

        private static void ConnectToServer(IPAddress address, int port)
        {
            IPEndPoint endpoint = new IPEndPoint(address, port);
            Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Byte[] buffer = new Byte[1024];

            try
            {
                socket.Connect(endpoint);
                Console.WriteLine("Connected to {0}", endpoint);

                while (true)
                {
                    socket.Receive(buffer);
                    location.FromBytes(buffer);
                    Console.WriteLine("Received location: {0:N6}:{1:N6}", location.Latitude, location.Longitude);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Main(String[] args)
        {
            IPAddress host = Dns.GetHostAddresses("localhost")[0];
            int port = 7777;
            ConnectToServer(host, port);
        }
    }
}