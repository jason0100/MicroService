using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace SayHelloService2
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
			using (var connection = factory.CreateConnection())//建立連線物件
			using (var channel = connection.CreateModel())//建立連線會話物件
			{
				//channel.QueueDeclare(
				//	queue: "hello",
				//	durable: false,//是否持久化,true持久化,佇列會儲存磁碟,伺服器重啟時可以保證不丟失相關資訊
				//                exclusive: false,//是否排他,true排他的,如果一個佇列宣告為排他佇列,該佇列僅對首次宣告它的連線可見,並在連線斷開時自動刪除.
				//                autoDelete: false,//是否自動刪除。true是自動刪除。自動刪除的前提是：致少有一個消費者連線到這個佇列，之後所有與這個佇列連線的消費者都斷開時,才會自動刪除.
				//                arguments: null);

				//交換機名稱
				string exchangeName = "exchange1";
				//宣告交換機+持久化設定
				//把交換機設定成Direct模式 有對應的routeKey才能接收訊息
				channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
				//訊息佇列名稱
				string queueName = "hello2";
				//宣告佇列
				channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);//聲明消息隊列，且為可持久化的

				//將佇列與交換機進行繫結
				string routeKey = "key2"; //匹配的key，

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
