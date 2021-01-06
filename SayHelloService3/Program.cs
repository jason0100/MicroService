using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace SayHelloService3
{
	class Program
	{
		static void Main(string[] args)
		{
			RegistryInAPIGateway();
			SubscribeToAPIGateway();
		}

		private static void SubscribeToAPIGateway()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())

			using (var channel = connection.CreateModel())
			{
				//channel.QueueDeclare(
				//    queue: "hello",
				//    durable: true,
				//    exclusive: false,Z
				//    autoDelete: false,
				//    arguments: null);

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
		/// <summary>
		/// 利用RabbitMQ註冊服務到API gateway(Web API)
		/// </summary>
		private static void RegistryInAPIGateway()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };

			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					string exchangeName = "exchange1";//交換機名稱
													  //把交換機設定成fanout釋出訂閱模式
													  //channel.ExchangeDeclare(exchangeName, type: "fanout");
					string routeKey = "key1"; //匹配的key，

					//把交換機設定成Direct模式 有對應的routeKey才能接收訊息+持久化設定
					channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);

					string message = "SayHelloService";
					var body = Encoding.UTF8.GetBytes(message);
					//把交換機設定成fanout訂閱模式
					//channel.BasicPublish(exchangeName, "", null, body);

					//把交換機設定成Direct模式 v需要設定routeKEy
					//exchange 持久化設定 way1
					channel.BasicPublish(exchangeName, routeKey, null, body);

			
				}
			}
		}
	}
}
