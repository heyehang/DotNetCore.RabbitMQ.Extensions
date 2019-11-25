using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using DotNetCore.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DotNetCore.RabbitMQ.Extensions
{
    public class ConnectionChannelPool : IConnectionChannelPool, IDisposable
    {
        private static readonly object SLock = new object();
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private const int DefaultPoolSize = 15;
        private int _count;
        private int _maxSize;
        ILogger logger;
        private IConnection _connection;
        private readonly ConcurrentQueue<IModel> _pool;
        RabbitMQOptions opt;
        public ConnectionChannelPool(ILogger<ConnectionChannelPool> logger, IOptions<RabbitMQOptions> opt)
        {
            this.opt = opt.Value;
            this.logger = logger;
            _maxSize = DefaultPoolSize;
            _pool = new ConcurrentQueue<IModel>();
        }

        public IConnection GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }
            _connectionLock.Wait();
            try
            {
                var factory = new ConnectionFactory() { HostName = opt.HostName, Port = opt.Port, VirtualHost = opt.VHost, UserName = opt.UserName, Password = opt.PassWord };
                var serviceName = Assembly.GetEntryAssembly()?.GetName().Name.ToLower();
                _connection = factory.CreateConnection(serviceName);

                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        IModel IConnectionChannelPool.Rent()
        {
            lock (SLock)
            {
                while (_count > _maxSize)
                {
                    Thread.SpinWait(1);
                }
                return Rent();
            }
        }

        public virtual IModel Rent()
        {
            if (_pool.TryDequeue(out var model))
            {
                Interlocked.Decrement(ref _count);

                Debug.Assert(_count >= 0);

                return model;
            }

            try
            {
                model = GetConnection().CreateModel();
            }
            catch (Exception e)
            {
                logger.LogError(e, "RabbitMQ channel model 创建失败!");
                Console.WriteLine(e);
                throw;
            }

            return model;
        }

        public void Dispose()
        {
            _maxSize = 0;

            while (_pool.TryDequeue(out var context))
            {
                context.Dispose();
            }
        }

        bool IConnectionChannelPool.Return(IModel connection)
        {
            return Return(connection);
        }

        public virtual bool Return(IModel connection)
        {
            if (Interlocked.Increment(ref _count) <= _maxSize)
            {
                _pool.Enqueue(connection);

                return true;
            }

            Interlocked.Decrement(ref _count);

            Debug.Assert(_maxSize == 0 || _pool.Count <= _maxSize);

            return false;
        }
    }
}
