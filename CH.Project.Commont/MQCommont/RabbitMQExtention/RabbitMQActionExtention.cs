//using CH.Project.Commont.LogCommont;
//using Polly;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using RabbitMQ.Client.Exceptions;
//using System;
//using System.Collections.Generic;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using static CH.Project.Commont.MQCommont.RabbitMQExtention.RabbitMQConsumer;



using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CH.Project.Commont.MQCommont.RabbitMQExtention.RabbitMQConsumer;

namespace CH.Project.Commont.MQCommont.RabbitMQExtention
{
    /// <summary>
    /// 使用教程
    /// 1.guest guest 下创建用户              cnj008@126
    /// 2.创建 Virtual host                   Supervisor 
    /// 3.创建发布者
    ///   3.1    RabbitMQProducter Producter = new RabbitMQProducter();
    ///   3.2    Producter.SetExchangQueueName(exchangName, queueName, exchangeType)
    ///   3.3    Producter.SendMessage("I am cnj")
    /// 4.创建接收者
    ///   4.1    RabbitMQConsumer Consumer = new RabbitMQConsumer();
    ///   4.2    Consumer.SetExchangQueueName(exchangName, queueName, exchangeType)
    ///   4.3    Consumer.ConsumerReceived += ConsumerReceived;
    ///   4.4    Consumer.InitRabbitMQ();
    /// </summary>
    public class RabbitMQActionExtention : IDisposable
    {
        protected internal partial class RabbitMQExtentionCore : IDisposable
        {
            public virtual string ExchangName { get; set; }
            public virtual string QueueName { get; set; }
            private string UserName { get; set; }
            private string Password { get; set; }
            private string HostName { get; set; }
            private int Port { get; set; }
            private string VirtualHost { get; set; }

            public virtual ConnectionFactory connectionFactory { get; set; }
            public virtual EventHandler<BasicDeliverEventArgs> ConsumerReceived { get; set; }

            #region 新增重新连接机制
            private object sync_root = new object();
            private bool _disposed;
            protected virtual IConnection connection { get; set; }
            public bool IsConnected => this.connection != null && this.connection.IsOpen && this._disposed;

            public int ConnectCount = 0;
            #endregion

            public RabbitMQExtentionCore(string userName, string password, string hostName, int port, string virtualHost)
            {
                connectionFactory = new ConnectionFactory
                {
                    UserName = userName,//用户名
                    Password = password,//密码
                    HostName = hostName,//rabbitmq ip
                    Port = port,
                    VirtualHost = virtualHost
                };
            }

            /// <summary>
            /// 注册持久的通道跟队列(
            /// Fanout Exchange：不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout交换机转发消息是最快的
            /// Direct Exchange：处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “test”，则只有被标记为“test”的消息才被转发，不会转发test.aaa，也不会转发dog.123，只会转发test
            /// Topic Exchange：将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”
            /// </summary>
            /// <param name="exchangeType">交换器类型</param>
            public void RegisterDurableExchangeAndQueue(string exchangeType = "direct")
            {
                try
                {
                    using (connection = connectionFactory.CreateConnection())
                    {
                        using (IModel channel = connection.CreateModel())
                        {
                            channel.ExchangeDeclare(exchange: ExchangName, type: exchangeType, durable: true, autoDelete: false, arguments: null);
                            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                            //绑定队列到交换器  
                            channel.QueueBind(queue: QueueName, exchange: ExchangName, routingKey: QueueName);
                        }
                    }
                }
                catch (Exception)
                {
                    if (!this.IsConnected)
                    {
                        this.TryConnect();
                    }
                }
            }

            #region 重连机制

            /// <summary>
            /// 重连机制
            /// </summary>
            /// <returns></returns>
            public bool TryConnect()
            {
                lock (this.sync_root)
                {
                    Policy policy = Policy.Handle<SocketException>()//如果我们想指定处理多个异常类型通过OR即可
                        .Or<BrokerUnreachableException>()//ConnectionFactory.CreateConnection期间无法打开连接时抛出异常
                        .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {

                        });
                    // 重试次数，提供等待特定重试尝试的持续时间的函数，每次重试时调用的操作。
                    policy.Execute(() =>
                    {
                        this.connection = connectionFactory.CreateConnection();
                        ConnectCount++;
                        if (ConnectCount > 10)
                        {
                            this._disposed = true;
                        }
                    });

                    if (this.IsConnected)
                    {
                        //当连接被破坏时引发。如果在添加事件处理程序时连接已经被销毁对于此事件，事件处理程序将立即被触发。
                        connection.ConnectionShutdown += this.OnConnectionShutdown;
                        //在连接调用的回调中发生异常时发出信号。当ConnectionShutdown处理程序抛出异常时，此事件将发出信号。如果将来有更多的事件出现在RabbitMQ.Client.IConnection上，那么这个事件当这些事件处理程序中的一个抛出异常时，它们将被标记。
                        connection.CallbackException += this.OnCallbackException;
                        connection.ConnectionBlocked += this.OnConnectionBlocked;

                        ConnectCount = 0;
                        //LogHelperNLog.Info($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                        return true;
                    }
                    else
                    {
                        // LogHelperNLog.Info("FATAL ERROR: RabbitMQ connections could not be created and opened");

                        return false;
                    }
                }
            }
            /// <summary>
            /// 连接断开事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="reason"></param>
            void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
            {
                if (this._disposed) return;
                //RabbitMQ连接正在关闭。 尝试重新连接...
                //LogHelperNLog.Info("A RabbitMQ connection is on shutdown. Trying to re-connect...");

                this.TryConnect();
            }
            /// <summary>
            ///   
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OnCallbackException(object sender, CallbackExceptionEventArgs e)
            {
                if (this._disposed)
                    return;

                // LogHelperNLog.Info("A RabbitMQ connection throw exception. Trying to re-connect...");

                this.TryConnect();
            }
            private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
            {
                if (this._disposed)
                    return;

                //  LogHelperNLog.Info("A RabbitMQ connection is shutdown. Trying to re-connect...");

                this.TryConnect();
            }
            #endregion
            public void Dispose()
            {
                if (this._disposed)
                {
                    return;
                }
                this._disposed = true;
                try
                {
                    this.connection.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    LogCommont.LogBuilder.CreateInstance().Error("Dispose Error:" + ex.Message);
                }
            }
        }

        protected virtual IConnection connection { get; set; }
        protected virtual RabbitMQExtentionCore RabbitMQCore { get; set; }

        public RabbitMQActionExtention(string userName, string password, string hostName, int port, string virtualHost)
        {
            RabbitMQCore = new RabbitMQExtentionCore(userName, password, hostName, port, virtualHost);
        }

        /// <summary>
        /// 默认初始化函数
        /// </summary>
        public RabbitMQActionExtention() : this("cnj008@126.com", "CHU383039284", "139.199.190.97", 5672, "Supervisor")
        {

        }

        /// <summary>
        /// 设置路由
        /// </summary>
        /// <param name="exchangName">CExchangName</param>
        /// <param name="queueName">CQueue</param>
        /// <param name="exchangeType">direct</param>
        public void SetExchangQueueName(string exchangName = "CExchangName", string queueName = "CQueue", string exchangeType = "topic")
        {
            RabbitMQCore.ExchangName = exchangName;
            RabbitMQCore.QueueName = queueName;
            RegisterDurableExchangeAndQueue(exchangeType);
        }

        /// <summary>
        /// 注册持久的通道跟队列
        /// Fanout Exchange：不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout交换机转发消息是最快的
        /// Direct Exchange：处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “test”，则只有被标记为“test”的消息才被转发，不会转发test.aaa，也不会转发dog.123，只会转发test
        /// Topic Exchange：将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”
        /// </summary>
        /// <param name="exchangeType">交换器类型</param>
        protected virtual void RegisterDurableExchangeAndQueue(string exchangeType = "direct")
        {
            RabbitMQCore.RegisterDurableExchangeAndQueue(exchangeType);
        }

        public void Dispose()
        {
            RabbitMQCore.Dispose();
        }
    }

    public interface IRabbitMQProducter
    {
        void SetExchangQueueName(string exchangName, string queueName, string exchangeType);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="persistent"></param>
        void SendMessage(string message, bool persistent = true);
    }

    /// <summary>
    /// 生产者
    /// </summary>
    public class RabbitMQProducter : RabbitMQActionExtention
    {
        public RabbitMQProducter(string userName, string password, string hostName, int port, string virtualHost) :
          base(userName, password, hostName, port, virtualHost)
        {

        }

        public RabbitMQProducter() : base()
        {

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="persistent">是否持久化</param>
        public void SendMessage(string message, bool persistent = true)
        {
            try
            {
                using (connection = RabbitMQCore.connectionFactory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        var props = channel.CreateBasicProperties();
                        if (persistent == true)
                        {
                            props.Persistent = true;//持久化到硬盘
                        }
                        var messageBody = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: RabbitMQCore.ExchangName, routingKey: RabbitMQCore.QueueName, basicProperties: props, body: messageBody);
                    }
                }
            }
            catch (Exception)
            {
                if (!RabbitMQCore.IsConnected)
                {
                    RabbitMQCore.TryConnect();
                }
            }

        }
    }

    public interface IRabbitMQQueueConsumer
    {
        void SetExchangQueueName(string exchangName, string queueName, string exchangeType);

        /// <summary>
        /// 消息处理委托
        /// </summary>
        ConsumerReceivedDel RabbitMQConsumerReceived { get; set; }

        /// <summary>
        /// 初始化消费者
        /// </summary>
        void InitRabbitMQ();
    }

    /// <summary>
    /// 消费者
    /// </summary>
    public class RabbitMQConsumer : RabbitMQActionExtention, IRabbitMQQueueConsumer
    {
        public class ConsumerEventArgs : EventArgs
        {
            //
            // 摘要:
            //     The message body.
            public ReadOnlyMemory<byte> Body { get; set; }
            //
            // 摘要:
            //     The consumer tag of the consumer that the message /// was delivered to.
            public string ConsumerTag { get; set; }
            //
            // 摘要:
            //     The delivery tag for this delivery. See /// IModel.BasicAck.
            public ulong DeliveryTag { get; set; }
            //
            // 摘要:
            //     The exchange the message was originally published /// to.
            public string Exchange { get; set; }
            //
            // 摘要:
            //     The AMQP "redelivered" flag.
            public bool Redelivered { get; set; }
            //
            // 摘要:
            //     The routing key used when the message was /// originally published.
            public string RoutingKey { get; set; }
        }

        public delegate void ConsumerReceivedDel(object sender, ConsumerEventArgs args);
        public ConsumerReceivedDel RabbitMQConsumerReceived { get; set; }

        public RabbitMQConsumer(string userName, string password, string hostName, int port, string virtualHost) :
          base(userName, password, hostName, port, virtualHost)
        {

        }
        public RabbitMQConsumer() : base()
        {

        }

        public void InitRabbitMQ()
        {
            RabbitMQCore.ConsumerReceived += (model, ea) =>
            {
                var args = new ConsumerEventArgs()
                {
                    Body = ea.Body,
                    ConsumerTag = ea.ConsumerTag,
                    DeliveryTag = ea.DeliveryTag,
                    Exchange = ea.Exchange,
                    Redelivered = ea.Redelivered,
                    RoutingKey = ea.RoutingKey
                };
                RabbitMQConsumerReceived?.Invoke(model, args);
            };

            Task.Factory.StartNew(() =>
            {
                GetMessage();
            });
        }

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <returns></returns>
        public void GetMessage()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (connection = RabbitMQCore.connectionFactory.CreateConnection())
                    {
                        using (IModel channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: RabbitMQCore.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                            channel.BasicQos(0, 1, false);//公平分发 为了改变这一状态，我们可以使用basicQos方法，设置perfetchCount=1 。这样就告诉RabbitMQ 不要在同一时间给一个工作者发送多于1个的消息
                            var consumer = new EventingBasicConsumer(channel);
                            consumer.Received += RabbitMQCore.ConsumerReceived;//接收到消息执行操作
                            consumer.Received += (model, ea) =>
                            {
                                channel.BasicAck(ea.DeliveryTag, false);//返回ACK
                            };
                            channel.BasicConsume(queue: RabbitMQCore.QueueName, autoAck: false, consumer: consumer);
                            while (true)//让消息一直连接
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (!RabbitMQCore.IsConnected)
                    {
                        RabbitMQCore.TryConnect();
                    }
                }
            });
        }
    }
}
