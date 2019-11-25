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

            services.AddSingleton<IConnectionChannelPool, TestAConnection>();
            services.AddSingleton<IConnectionChannelPool, TestBConnection>();

            services.AddSingleton<IConsumerService, TestAConsumer>();
            services.AddSingleton<IConsumerService, TestBConsumer>();
            services.AddSingleton<IPublishService, TestBPublish>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var connList = serviceProvider.GetService<IEnumerable<IConnectionChannelPool>>();

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
