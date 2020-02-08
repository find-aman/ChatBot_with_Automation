using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace DailogFlow_UiPath_Connector
{
    public class Startup
    {
		// we use appsettings.json as configuration file to store key-value pairs, 
		// like database connection string, etc
		public IConfiguration configuration { get; }

		// we are using Autofac container here to add services and setup Dependency Injection
		public static IContainer container { get; private set; }

		// Constructor: initialize configuration 
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder().
							SetBasePath(env.ContentRootPath).
							AddJsonFile("appsettings.json", false, true).
							AddEnvironmentVariables();
			configuration = builder.Build();
		}

		// we added MVC and CORS with a new policy named "AllowAll" to allow visiting from any domains
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			
			services.AddMvc();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					p => p.AllowAnyOrigin().
						AllowAnyHeader().
						AllowAnyMethod().
						AllowCredentials()
						);
			});
			var builder = new ContainerBuilder();

			builder.Populate(services);

			container = builder.Build();
			return new AutofacServiceProvider(container);
		}

		// We applied the "AllowAll" CORS policy and MVC service 
		// the container would be disposed when the application stopped
		public void Configure(IApplicationBuilder app,
						IHostingEnvironment env,
						ILoggerFactory loggerFctory,
						IApplicationLifetime applicationLifetime)
		{
			app.UseCors("AllowAll");
			app.UseMvc();
			applicationLifetime.ApplicationStopped.Register(() => container.Dispose());

		}

	}
}
