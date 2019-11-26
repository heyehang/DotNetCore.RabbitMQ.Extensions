using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestBConnection : ConnectionChannelPool
    {
        public TestBConnection(ILogger<TestBConnection> logger) : base(logger)
        {
        }

        public override RabbitMQOptions opt => new RabbitMQOptions
        {
            HostName = "localhost",
            Port = 5672,
            VHost = "testb.host",
            UserName = "guest",
            PassWord = "guest"
        };

        public override string ConnectionKey => "TestBconn";
    }
}
