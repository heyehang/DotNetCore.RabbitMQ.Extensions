using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.RabbitMQ.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestCommon;

namespace TestProjectConsumer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddRabbitMQConnectionChannelPool(opt =>
            {
                opt.HostName = "localhost";
                opt.Port = 5672;
                opt.VHost = "test.host";
                opt.UserName = "guest";
                opt.PassWord = "guest";
            });
            services.AddSingleton<IConsumerService, TestConsumer>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var consumerList = serviceProvider.GetService<IEnumerable<IConsumerService>>();

            Task.Run(() =>
            {
                foreach (var e in consumerList)
                {
                    e.Start();
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
        }
    }
}
