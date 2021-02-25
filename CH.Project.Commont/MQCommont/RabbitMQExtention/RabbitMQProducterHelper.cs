using CH.Project.Commont.ConfigCommont;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.MQCommont.RabbitMQExtention
{
    public class RabbitMQProducterHelper : SingleCommont<RabbitMQProducterHelper>
    {
        public static RabbitMQProducter Producter;

        public RabbitMQProducterHelper()
        {
            Producter = new RabbitMQProducter();
        }

        public void SetExchangQueueName(string exchangName = "", string queueName = "", string exchangeType = "")
        {
            if (string.IsNullOrEmpty(exchangName))
            {
                exchangName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQVirtualHost");
            }
            if (string.IsNullOrEmpty(queueName))
            {
                queueName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangQueue");
            }
            if (string.IsNullOrEmpty(exchangeType))
            {
                exchangeType = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangType");
            }
            Producter.SetExchangQueueName(exchangName, queueName, exchangeType);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="value"></param>
        public void SendMessage(string value)
        {
            Producter.SendMessage(value);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="consumerCmd"></param>
        public void SendMessage(ConsumerCmd consumerCmd)
        {
            SendMessage(JsonConvert.SerializeObject(consumerCmd));
        }
    }
}
