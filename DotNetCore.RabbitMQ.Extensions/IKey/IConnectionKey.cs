using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public interface IConnectionKey
    {
        /// <summary>
        /// Rabbit连接池key,连接复用
        /// </summary>
        string ConnectionKey { get; }
    }
}
