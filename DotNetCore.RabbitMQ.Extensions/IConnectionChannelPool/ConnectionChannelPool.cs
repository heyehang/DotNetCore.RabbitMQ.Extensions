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
    public abstract class ConnectionChannelPool : IConnectionKey, IConnectionChannelPool, IDisposable
    {
        private static readonly object SLock = new object();
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private const int DefaultPoolSize = 15;
        private int _count;
        private int _maxSize;
        ILogger logger;
        private IConnection _connection;
        private readonly ConcurrentQueue<IModel> _pool;
        public abstract RabbitMQOptions opt { get; }
        public abstract string ConnectionKey { get; }

        public ConnectionChannelPool(ILogger logger)
        {
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

                _connection = factory.CreateConnection(ConnectionKey);
                _connection.ConnectionShutdown += (e, s) => { logger.LogWarning($"RabbitMQ channel model 创建失败!-->{s.ReplyText}"); };

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
                logger.LogError(e, $"RabbitMQ channel model 创建失败!-->{e.StackTrace}");
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
