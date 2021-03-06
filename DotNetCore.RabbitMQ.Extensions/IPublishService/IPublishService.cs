﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.RabbitMQ.Extensions
{
    public interface IPublishService : IServiceKey
    {
        Task PublishAsync(object objmsg);

        void Publish(object objmsg);
    }
}
