using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCommon;
using System.Linq;

namespace TestConsoleConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

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

            Console.ReadKey();
        }
    }
}
