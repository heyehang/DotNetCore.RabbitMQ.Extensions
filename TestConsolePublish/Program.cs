using Microsoft.Extensions.DependencyInjection;
using System;
using DotNetCore.RabbitMQ.Extensions;
using TestCommon;
using System.Collections.Generic;
using System.Linq;

namespace TestConsolePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();
            //连接池
            services.AddSingleton<IConnectionChannelPool, TestAConnection>();
            services.AddSingleton<IConnectionChannelPool, TestCConnection>();
            services.AddSingleton<IConnectionChannelPool, TestDConnection>();

            //生产者
            services.AddSingleton<TestAPublish>();
            services.AddSingleton<TestBPublish>();
            services.AddSingleton<TestCPublish>();
            services.AddSingleton<TestDPublish>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var testAPublish = serviceProvider.GetService<TestAPublish>();
            var testBPublish = serviceProvider.GetService<TestBPublish>();
            var testCPublish = serviceProvider.GetService<TestCPublish>();
            var testDPublish = serviceProvider.GetService<TestDPublish>();

            #region 普通测试

            //while (true)
            //{
            //    Console.WriteLine("请输入要发送的消息：");
            //    var msg = Console.ReadLine();
            //    testAPublish.Publish(msg);
            //    Console.WriteLine($"发送的消息{msg}成功");
            //    Console.ReadKey();
            //}
            #endregion 普通测试

            //#region 压力测试
            //Console.WriteLine("准备压力测试,按任意键继续");
            //Console.ReadKey();
            //for (int i = 1; i < 10000; i++)
            //{
            //    testPublish.Publish(i);
            //    Console.WriteLine($"发送第{i}消息成功");
            //}
            //Console.WriteLine("压力测试完成");
            //Console.ReadKey();
            //#endregion 压力测试

            #region 测试单个实例多消费者
            //while (true)
            //{
            //    Console.WriteLine("请输入要发送的消息：");
            //    var msg = Console.ReadLine();
            //    testCPublish.Publish("测试单个实例多消费者");
            //    Console.WriteLine($"发送的消息{msg}成功");
            //    Console.ReadKey();
            //}

            #endregion 测试单个实例多消费者

            while (true)
            {
                Console.WriteLine("请输入要发送的消息：");
                var msg = Console.ReadLine();
                testDPublish.Publish("测试单个实例多消费者");
                Console.WriteLine($"发送的消息{msg}成功");
                Console.ReadKey();
            }
        }
    }
}
