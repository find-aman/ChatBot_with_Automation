using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DailogFlow_UiPath_Connector
{
    public class Program
	{// Invoke web server and host it
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Run();
		}

		// Setup a web server with default url and Startup
		public static IWebHost CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseUrls("http://localhost:5002")
				.UseKestrel()
				.UseIISIntegration()
				.UseStartup<Startup>().Build();
		}
	}
}
