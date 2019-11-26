using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public interface IConnectionKey
    {
        /// <summary>
        /// Rabbit连接池key,连接复用
        /// 多继承类关系注入可能会用上，示例 service.AddSingleton<IService,AService>();service.AddSingleton<IService,BService>();
        /// </summary>
        string ConnectionKey { get; }
    }
}
