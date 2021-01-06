using API_Gateway.Controllers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Gateway.Models
{
	public class RabbitMQListener
	{
        private static IConnection connection;
        private static IModel channel;

        public static void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(
                    queue: "registry",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                ServiceRegistryController src = new ServiceRegistryController();
                src.PostAsync(message);
            };

            channel.BasicConsume(
                queue: "registry",
                autoAck: true,
                consumer: consumer);
        }

        public static void Stop()
        {
            channel.Close(500, "Channel closed");
            connection.Close();
        }
    }
}
