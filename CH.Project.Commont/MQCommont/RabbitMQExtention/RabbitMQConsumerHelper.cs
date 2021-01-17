using CH.Project.Commont.ConfigCommont;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static CH.Project.Commont.MQCommont.RabbitMQExtention.RabbitMQConsumer;

namespace CH.Project.Commont.MQCommont.RabbitMQExtention
{
    public class RabbitMQConsumerHelper : SingleCommont<RabbitMQConsumerHelper>
    {
        public static RabbitMQConsumer Consumer;
        public virtual ConsumerLogicDel ConsumerLogicDelegate { get; set; }

        public RabbitMQConsumerHelper()
        {
            if (Consumer == null)
            {
                var exchangName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQVirtualHost");
                var queueName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangQueue");
                var exchangeType = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangType");

                Consumer = new RabbitMQConsumer();
                Consumer.SetExchangQueueName(exchangName, queueName, exchangeType);
                Consumer.RabbitMQConsumerReceived += ConsumerReceived;
                Consumer.InitRabbitMQ();
            }
        }

        public void ConsumerReceived(object sender, ConsumerEventArgs args)
        {
            string value = System.Text.Encoding.Default.GetString(args.Body.ToArray());
            var cmdValue = JsonConvert.DeserializeObject<ConsumerCmd>(value);
            if (cmdValue != null)
            {
                ConsumerLogicDelegate?.Invoke(cmdValue);
            }
        }

        public void Start()
        {
            Console.WriteLine("runing");
        }

        public delegate void ConsumerLogicDel(ConsumerCmd consumerCmd);
    }

    public class ConsumerCmd
    {
        public virtual ConsumerCommandType CmdType { get; set; }
        public virtual string Param { get; set; }
        public virtual string Res { get; set; }

        public virtual SubActionType ActionType { get; set; }
    }

    public enum ConsumerCommandType
    {
        LoginCount = 1,
        ViewCount = 2,
        ThumbCount = 3,
        ShareAction = 4
    }

    public enum SubActionType
    {
        View = 1,
        Thumb = 2,
    }
}
