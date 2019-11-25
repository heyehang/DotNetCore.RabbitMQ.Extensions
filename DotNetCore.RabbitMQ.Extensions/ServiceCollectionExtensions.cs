using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQConnectionChannelPool(this IServiceCollection services, Action<RabbitMQOptions> opt)
        {
            if (opt == null)
            {
                throw new ArgumentNullException(nameof(opt));
            }

            return services.Configure<RabbitMQOptions>(opt).AddTransient<IConnectionChannelPool, ConnectionChannelPool>();
        }
    }
}
