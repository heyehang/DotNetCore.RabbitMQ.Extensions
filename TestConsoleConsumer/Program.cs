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

            //连接池
            services.AddSingleton<IConnectionChannelPool, TestAConnection>();
            services.AddSingleton<IConnectionChannelPool, TestBConnection>();
            services.AddSingleton<IConnectionChannelPool, TestCConnection>();
            services.AddSingleton<IConnectionChannelPool, TestDConnection>();

            //消费者
            services.AddSingleton<IConsumerService, TestAConsumer>();
            services.AddSingleton<IConsumerService, TestBConsumer>();
            services.AddSingleton<IConsumerService, TestCConsumer>();
            services.AddSingleton<IConsumerService, TestDConsumer>();

            //生产者
            services.AddSingleton<TestBPublish>();

            //启动消费监听
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var consumerList = serviceProvider.GetService<IEnumerable<IConsumerService>>();
            Task.Run(() =>
            {
                foreach (var e in consumerList)
                {
                    e.Start();
                }
            });

            Console.WriteLine("准备接受消息");
            Console.ReadKey();
        }
    }
}
