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

            //交換機名稱
            string exchangeName = "exchange1";
            //宣告交換機+持久化設定
            //把交換機設定成Direct模式 有對應的routeKey才能接收訊息
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
            //訊息佇列名稱
            string queueName = "hello1";
            //宣告佇列
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);//聲明消息隊列，且為可持久化的

            //將佇列與交換機進行繫結
            string routeKey = "key1"; //匹配的key，

            channel.QueueBind(queueName, exchangeName, routeKey, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                ServiceRegistryController src = new ServiceRegistryController();
                src.PostAsync(message);
                channel.BasicAck(ea.DeliveryTag, false);//返回訊息確認
            };

            //開啟監聽
            channel.BasicConsume(
                //queue: "hello",
                queue: queueName,
                autoAck: false,////將autoAck設定false 關閉自動確認.
                consumer: consumer);

        }

        public static void Stop()
        {
            channel.Close(500, "Channel closed");
            connection.Close();
        }
    }
}
