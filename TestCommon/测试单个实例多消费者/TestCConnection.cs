using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public class TestCConnection : ConnectionChannelPool
    {
        public TestCConnection(ILogger<TestCConnection> logger) : base(logger)
        {
        }

        public override RabbitMQOptions opt => new RabbitMQOptions
        {
            HostName = "localhost",
            Port = 5672,
            VHost = "testc.host",
            UserName = "guest",
            PassWord = "guest"
        };

        public override string ConnectionKey => "TestCconn";
    }
}
