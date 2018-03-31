using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace BTAServer
{
    class ServerClient
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1);
        private static readonly int maxRetries = 3;
        private static readonly int timeout = 1;

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
            int timestamp = (int)(DateTime.UtcNow.Subtract(epoch)).TotalSeconds;
            return Healthy() || timestamp - lastRetry > timeout;
        }

        public bool Fail()
        {
            if (--retries == 0)
                return true;
            lastRetry = (int)(DateTime.UtcNow.Subtract(epoch)).TotalSeconds;
            return false;
        }
    }

    class Server
    {
        private static readonly int busNumber = 36;

        // Sockets clients
        private static Dictionary<IPEndPoint, ServerClient> clients = new Dictionary<IPEndPoint, ServerClient>();
        // RabbitMQ
        private static ConnectionFactory factory = null;
        private static IConnection connection = null;
        private static IModel channel = null;
        private static String queueName = null;

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

            Location location = null;

            try
            {
                listener.Bind(endpoint);
                listener.Listen(64);

                while (true)
                {
                    location = GetLocation();
                    listener.BeginAccept(new AsyncCallback(AcceptClient), listener);
                    TransmitLocationToClients(location);
                    QueueLocation(location);
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

        private static void TransmitLocationToClients(Location location)
        {
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
                catch (SocketException)
                {
                    Console.WriteLine("Failed to send to client {0}", pair.Key);
                    if (client.Fail())
                    {
                        Console.WriteLine("Client removed");
                        clients.Remove(pair.Key);
                        break;
                    }
                }
            }
        }

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

        private static void InitMessageQueue(String name, String hostName)
        {
            queueName = name;
            factory = new ConnectionFactory() { HostName = hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private static void QueueLocation(Location location)
        {
            var body = System.Text.Encoding.ASCII.GetBytes(busNumber + "|" + location.ToString());

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);
        }

        static void Main(String[] args)
        {
            const String hostName = "localhost";
            IPAddress host = Dns.GetHostAddresses(hostName)[0];
            int port = 7777;

            // Set up RabbitMQ connection
            InitMessageQueue("BTA", hostName);
            // Start socket server
            StartListening(host, port);
        }
    }
}
