using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace SayHelloService
{
	class Program
	{
		static void Main(string[] args)
		{
			SubscribeToAPIGateway();

		}
		private static void SubscribeToAPIGateway()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				
				//交換機名稱
				string exchangeName = "exchange1";
				//宣告交換機+持久化設定
				channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
				//訊息佇列名稱
				string queueName = "hello1";
				//宣告佇列
				channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);//聲明消息隊列，且為可持久化的
				string routeKey = "key1"; //匹配的key，
										  //將佇列與交換機進行繫結
				channel.QueueBind(queueName, exchangeName, routeKey, null);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (ch, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine($"Received: {message}");
					Console.WriteLine($"Hello, {message}");
					//Console.WriteLine($"Received:{ea.DeliveryTag}延遲發送");
					channel.BasicAck(ea.DeliveryTag, false);//返回訊息確認
				};

				//開啟監聽
				channel.BasicConsume(
					//queue: "hello",
					queue: queueName,
					autoAck: false,////將autoAck設定false 關閉自動確認.
					consumer: consumer);

				Console.WriteLine("Pess any key to exit");
				Console.ReadLine();
			}
		}
	}
}
