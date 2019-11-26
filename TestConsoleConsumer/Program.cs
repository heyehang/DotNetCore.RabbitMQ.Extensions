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

            #region 测试不同队列消息相互传递
            //services.AddSingleton<IConnectionChannelPool, TestAConnection>();
            //services.AddSingleton<IConnectionChannelPool, TestBConnection>();

            //services.AddSingleton<IConsumerService, TestCConsumer>();
            //services.AddSingleton<IConsumerService, TestBConsumer>();
            //services.AddSingleton<IPublishService, TestBPublish>();
            //IServiceProvider serviceProvider = services.BuildServiceProvider();
            //var connList = serviceProvider.GetService<IEnumerable<IConnectionChannelPool>>();

            //var consumerList = serviceProvider.GetService<IEnumerable<IConsumerService>>();

            //Task.Run(() =>
            //{
            //    foreach (var e in consumerList)
            //    {
            //        e.Start();
            //    }
            //});
            #endregion 测试不同队列消息相互传递

            #region 测试单个实例多消费者
            services.AddSingleton<IConnectionChannelPool, TestCConnection>();
            services.AddSingleton<IConsumerService, TestCConsumer>();

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
            #endregion 测试单个实例多消费者

            Console.ReadKey();
        }
    }
}
