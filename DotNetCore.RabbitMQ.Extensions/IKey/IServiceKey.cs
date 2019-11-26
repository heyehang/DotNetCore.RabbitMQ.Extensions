using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public interface IServiceKey
    {
        /// <summary>
        /// 实现服务的Key,服务发现和日志跟踪 -->示例(Namespace.Class.Method)
        /// 多继承类关系注入可能会用上，示例 service.AddSingleton<IService,AService>();service.AddSingleton<IService,BService>();
        /// </summary>
        string ServiceKey { get; }
    }
}
