using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Gateway.Controllers
{
	[Produces("application/json")]
	[Route("api/SayHello")]
	[ApiController]
	public class SayHelloController : ControllerBase
	{


		[HttpPost]
		public string Post([FromQuery] string name)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };

			using (var connection = factory.CreateConnection())//建立連線物件
			{
				using (var channel = connection.CreateModel())//建立連線會話物件
				{
					string exchangeName = "exchange1";//交換機名稱
													  //把交換機設定成fanout釋出訂閱模式
													  //channel.ExchangeDeclare(exchangeName, type: "fanout");
					string routeKey = "key1"; //匹配的key，

					//把交換機設定成Direct模式 有對應的routeKey才能接收訊息+持久化設定
					channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);


					string message = name;
					var body = Encoding.UTF8.GetBytes(message);
					//把交換機設定成fanout訂閱模式
					//channel.BasicPublish(exchangeName, "", null, body);

					//把交換機設定成Direct模式 v需要設定routeKEy
					//exchange 持久化設定 way1
					channel.BasicPublish(exchangeName, routeKey, null, body);

					//exchange 持久化設定 way2
					//var properties = channel.CreateBasicProperties();
					//properties.Persistent = true;
					//channel.BasicPublish(exchangeName, routeKey, basicProperties: properties, body);

			
				}
			}
			return $"Hello,{name}";
		}
	}
}
