using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.RabbitMQ.Extensions
{
    public class RabbitMQOptions
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public string HostName { get; set; }
        public string VHost { get; set; }
        public int Port { get; set; } = -1;
    }
}
