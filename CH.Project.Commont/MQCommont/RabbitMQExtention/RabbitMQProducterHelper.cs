using CH.Project.Commont.ConfigCommont;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.MQCommont.RabbitMQExtention
{
    public class RabbitMQProducterHelper : SingleCommont<RabbitMQProducterHelper>
    {
        public static RabbitMQProducter Producter ;

        public RabbitMQProducterHelper()
        {
            if (Producter == null)
            {
                var exchangName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQVirtualHost");
                var queueName = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangQueue");
                var exchangeType = ConfigActionCommont.CreateInstance().GetValue("MQSetting:MQExchangType");

                Producter = new RabbitMQProducter();
                Producter.SetExchangQueueName(exchangName, queueName, exchangeType);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SendMessage(string value)
        {
            Producter.SendMessage(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerCmd"></param>
        public void SendMessage(ConsumerCmd consumerCmd)
        {
            SendMessage(JsonConvert.SerializeObject(consumerCmd));
        }
    }
}
