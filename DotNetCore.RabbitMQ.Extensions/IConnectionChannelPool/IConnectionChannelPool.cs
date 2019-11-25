using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public interface IConnectionChannelPool : IConnectionKey
    {
        IConnection GetConnection();

        IModel Rent();

        bool Return(IModel context);
    }
}
