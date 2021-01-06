using API_Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Gateway.Controllers
{
	[Produces("application/json")]
	[Route("api/ServiceRegistry")]
	[ApiController]
	public class ServiceRegistryController : Controller
	{
		

		static List<MicroService> serviceList = new List<MicroService>() {
			new MicroService() { Name="API Gateway", Location="http://localhost:2343" }
		};
		// GET: api/ServiceRegistry
		[HttpGet]
		public IEnumerable<MicroService> Get()
		{
			return serviceList;
		}

		// POST: api/ServiceRegistry
		[HttpPost]
		public void Post([FromBody] MicroService service)
		{
			serviceList.Add(service);
		}

		public string PostAsync(string service)
		{
			serviceList.Add(new MicroService() { Name = service, Location = "RabbitMQ" });
			return service;
		}

		// DELETE: api/ServiceRegistry/{serviceName}
		[HttpDelete("{serviceName}")]
		public void Delete(string serviceName)
		{
			serviceList.Remove(serviceList.First(s => s.Name == serviceName));
		}
	}
}
