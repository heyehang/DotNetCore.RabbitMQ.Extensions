using Microsoft.Extensions.DependencyInjection;
using System;
using DotNetCore.RabbitMQ.Extensions;
using TestCommon;

namespace TestConsolePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();

            services.AddSingleton<IConnectionChannelPool, TestAConnection>();

            services.AddSingleton<IPublishService, TestAPublish>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var testPublish = serviceProvider.GetService<IPublishService>();

            #region 普通测试

            while (true)
            {
                Console.WriteLine("请输入要发送的消息：");
                var msg = Console.ReadLine();
                testPublish.Publish(msg);
                Console.WriteLine($"发送的消息{msg}成功");
                Console.ReadKey();
            }
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
        }
    }
}
