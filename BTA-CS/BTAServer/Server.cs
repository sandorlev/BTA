using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace BTAServer
{
    class ServerClient
    {
        public static readonly int maxRetries = 3;
        public static readonly int timeout = 1;

        public Socket ClientSocket { get; set; }
        private int retries = maxRetries;
        private int lastRetry = 0;

        public ServerClient(Socket socket)
        {
            ClientSocket = socket;
            ResetHealth();
        }

        public void ResetHealth()
        {
            retries = maxRetries;
            lastRetry = 0;
        }

        public bool Healthy()
        {
            return lastRetry == 0;
        }

        public bool Ready()
        {
            int timestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return Healthy() || timestamp - lastRetry > timeout;
        }

        public bool Fail()
        {
            if (--retries == 0)
                return true;
            lastRetry = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return false;
        }
    }

    class Server
    {
        private static Dictionary<IPEndPoint, ServerClient> clients = new Dictionary<IPEndPoint, ServerClient>();

        // XXX - returns random location atm
        private static readonly Random random = new Random();
        public static Location GetLocation() {
            float latitude = (float)(random.NextDouble() * 180);
            float longitude = (float)(random.NextDouble() * 180);
            return new Location(latitude, longitude);
        }

        public static void StartListening(IPAddress address, int port)
        {
            Byte[] bytes = new Byte[StateObject.bufferSize];
            IPEndPoint endpoint = new IPEndPoint(address, port);
            Socket listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(endpoint);
                listener.Listen(64);

                while (true)
                {
                    listener.BeginAccept(new AsyncCallback(AcceptClient), listener);
                    TransmitLocation();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        private static void AcceptClient(IAsyncResult result)
        {
            Socket listener = (Socket)result.AsyncState;
            ServerClient client = new ServerClient(listener.EndAccept(result));
            IPEndPoint endpoint = client.ClientSocket.RemoteEndPoint as IPEndPoint;
            clients[endpoint] = client;
            Console.WriteLine("New client at {0}", endpoint);
        }

        private static void TransmitLocation()
        {
            Location location = GetLocation();
            Byte[] bytes = location.ToBytes();
            foreach (KeyValuePair<IPEndPoint, ServerClient> pair in clients)
            {
                ServerClient client = pair.Value;
                if (!client.Ready())
                    continue;

                Socket socket = client.ClientSocket;
                try
                {
                    Console.WriteLine("Sending to {0} location {1}", pair.Key, location.ToString());
                    socket.BeginSend(
                        bytes, 0,
                        bytes.Length, 0,
                        WriteToClient,
                        socket
                    );
                    client.ResetHealth();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Failed to send to client {0}", pair.Key);
                    if (client.Fail())
                    {
                        Console.WriteLine("banned lol");
                        clients.Remove(pair.Key);
                        break;
                    }
                }
            }
        }

        //private static void SendString(Socket handler, String data)
        //{
        //    byte[] byteData = Encoding.ASCII.GetBytes(data);
        //    handler.BeginSend(
        //        byteData, 0,
        //        byteData.Length, 0,
        //        new AsyncCallback(WriteToClient),
        //        handler
        //    );
        //}

        private static void WriteToClient(IAsyncResult result)
        {
            try
            {
                Socket handler = (Socket)result.AsyncState;
                handler.EndSend(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //    private static void ReadFromClient(IAsyncResult result)
        //    {
        //        String content = String.Empty;
        //        ClientMessage message = (ClientMessage)result.AsyncState;
        //        Socket handler = message.socket;

        //        int bytesRead = handler.EndReceive(result);

        //        if (bytesRead > 0)
        //        {
        //            message.sb.Append(Encoding.ASCII.GetString(message.buffer, 0, bytesRead));
        //            content = message.sb.ToString();
        //            if (content.IndexOf("<EOF>") > -1)
        //            {
        //                Console.WriteLine("Read {0} bytes. Data: {1}", content.Length, content);
        //                SendString(handler, content);
        //            }
        //            else
        //            {
        //                handler.BeginReceive(
        //                    message.buffer, 0,
        //                    ClientMessage.bufferSize, 0,
        //                    new AsyncCallback(ReadFromClient),
        //                    message
        //                );
        //            }
        //        }
        //    }

        static void Main(string[] args)
        {
            IPAddress host = Dns.GetHostAddresses("localhost")[0];
            int port = 7777;
            StartListening(host, port);
        }
    }
}
