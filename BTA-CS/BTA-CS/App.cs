using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using BTAServer;

namespace BTA_CS
{
    public class App
    {
        private static Location locationBuffer = new Location();
        private static System.Collections.Generic.Dictionary<int, Location> busMap = null;

        private static ConnectionFactory factory = null;
        private static IConnection connection = null;
        private static IModel channel = null;
        private static String queueName = null;
        private static EventingBasicConsumer consumer = null;

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

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += MessageCallback;
        }

        private static void MessageCallback(Object model, BasicDeliverEventArgs eventArgs)
        {
            var message = System.Text.Encoding.ASCII.GetString(eventArgs.Body);
            var tokens = message.Split('|');
            var busNumber = int.Parse(tokens[0]);
            locationBuffer.FromString(tokens[1]);
            busMap[busNumber] = locationBuffer.Duplicate();
            Console.WriteLine(" [x] Received {0} from bus {1}", locationBuffer.ToString(), busNumber);
        }

        private static void ConsumeQueue()
        {
            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        public static void Main(String[] args)
        {
            busMap = new System.Collections.Generic.Dictionary<int, Location>();
            InitMessageQueue("BTA", "localhost");
            while (true)
            {
                ConsumeQueue();
            }
        }
    }
}