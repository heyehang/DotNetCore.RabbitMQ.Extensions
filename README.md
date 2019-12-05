
# DotNetCore.RabbitMQ.Extensions介绍

* 这是一个 基于.NETStandard 2.0的Rabbit轻量级框架，可以让开发人员无需关注底层变动，专注编写业务代码，从而达到便捷开发。

# 特性

* DotNetCore.RabbitMQ.Extensions，非常的小巧，下面将介绍 DotNetCore.RabbitMQ.Extensions 的项目框架。
* 开发设计思路是将Rabbit的连接池，生产者，消费者三种业务类型分层分离，从而实现解耦轻量化。
* 连接池，生产者，消费者的设计实现逻辑采用适配器设计,实现各自之间单一职责与开闭原则，是非常有利于业务的扩展和维护。
* 连接池：内置连接池管理，无需重复创建连接和信道，并采用安全线程控制。在这里用户只需要关心配置连接池相关参数。
* 生产者和消费者：底层已经全部抽象实现，无须关注底层逻辑，在这里用户只需要关心配置生产者/消费者相关参数，并且消费者支持单例多重消费者。
* 开发人员只需要在Rabbit管控台新建相关的VHost，其他参数（Exchange，Queue，ExchangeType,RoutingKey）全部代码自动帮你建立完好，无须手动新建，解决繁琐操作。
* 项目 gitbhub 地址：<https://github.com/shininggold/DotNetCore.RabbitMQ.Extensions>

---


## 参数说明

*  HostName，Rabbit所在服务器地址
*  port,端口号
*  username，登录账号名称
*  password，登录密码
*  VHost，虚拟主机
*  Exchange，交换机
*  ExchangeType,交换机类型
*  Queue，队列名称
*  RoutingKey，队列与交换机绑定的key
*  ServiceKey,当前服务的key，推荐：nameof(当前类名)
*  ConnectionKey,当前连接池服务的key，推荐：nameof(当前连接池类名)
*  ConsumerTotal,当前消费队列所对应的消费者数量（默认为1，支持单例消费者支持单列多重消费者,继承ConsumerService并重写ConsumerTotal即可）
# 如何开始？



1. 下载安装 DotNetCore.RabbitMQ.Extensions

```
安装命令：Install-Package DotNetCore.RabbitMQ.Extensions
```
    

## 连接池

* 继承ConnectionChannelPool类，并实现相关连接池参数和连接池的唯一标识ConnectionKey。

## 示例代码

``` C#
namespace TestCommon
{
    public class TestDConnection : ConnectionChannelPool
    {
        public TestDConnection(ILogger<TestDConnection> logger) : base(logger)
        {
        }

        public override RabbitMQOptions opt => new RabbitMQOptions
        {
            HostName = "localhost",
            Port = 5672,
            VHost = "testd.host",
            UserName = "guest",
            PassWord = "guest"
        };

        public override string ConnectionKey => nameof(TestDConnection);
    }
}
```

---

## 生产者

* 继承PublishService类，并实现相关参数，并且绑定所需要使用的连接池ConnectionKey，发送队列消息：TestDPublish.Publish(objmsg);/await TestDPublish.PublishAsync(objmsg);

## 示例代码

``` C#
namespace TestCommon
{
    public class TestDPublish : PublishService
    {
        public TestDPublish(ILogger<TestDPublish> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
        }

        public override string ExchangeType => "direct";

        public override string Exchange => "testd.ex";

        public override string Queue => "testd.query";

        public override string RoutingKey => "testd.key";

        public override string ConnectionKey => nameof(TestDConnection);

        public override string ServiceKey => nameof(TestDPublish);
    }
}
```

---

## 消费者

* 继承ConsumerService类，并实现相关参数，并且绑定所需要使用的连接池ConnectionKey，与具体业务消费逻辑。

## 示例代码

``` C#
namespace TestCommon
{
    public class TestDConsumer : ConsumerService
    {
        ILogger logger;
        public TestDConsumer(ILogger<TestDConsumer> logger, IEnumerable<IConnectionChannelPool> connectionList) : base(logger, connectionList)
        {
             this.logger = logger;
        }

        public override string Queue => "test.query";

        public override bool AutoAck => true;

        public override string ServiceKey => nameof(TestDConsumer);

        public override string ConnectionKey => nameof(TestDConnection);

        public override void Received(object sender, BasicDeliverEventArgs e)
        {
            RemoveEnvironmentModel model = new RemoveEnvironmentModel();
            try
            {
                model = JsonConvert.DeserializeObject<RemoveEnvironmentModel>(Encoding.UTF8.GetString(e.Body));
            }
            catch (Exception)
            {
                logger.LogError($"{ServiceKey}服务消费解析model错误");
                throw;
            }
            Console.WriteLine($"消费者{ServiceKey}：收到消息");
        }
    }
}
```

---

## 服务注册


## 示例代码

``` C#
IServiceCollection services = new ServiceCollection();

            services.AddLogging();

            //连接池
            services.AddSingleton<IConnectionChannelPool, TestDConnection>();

            //消费者
            services.AddSingleton<IConsumerService, TestDConsumer>();

            //生产者
            services.AddSingleton<TestDPublish>();

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


```

---
