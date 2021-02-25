using CH.Project.Commont.MQCommont.RabbitMQExtention;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CH.Project.Commont.WebSocketCommont
{
    public enum WebSocketSendType
    {
        /// <summary>
        /// 系统发送
        /// </summary>
        SystemMsg,
        /// <summary>
        /// 单独发送
        /// </summary>
        PointToPoint,
        /// <summary>
        /// 群发
        /// </summary>
        GroupChat,
        /// <summary>
        /// 广播
        /// </summary>
        Broadcast
    }
    public enum DataType
    {
        String,
        Json,
    }
    public class WebSocketModel
    {
        public virtual WebSocket Sk { get; set; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 用户关联ID
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public virtual string ServerIp { get; set; }
    }

    public class MessageModel
    {
        public virtual DataType DataType { get; set; } = DataType.Json;
        public virtual WebSocketSendType SendType { get; set; }
        public virtual string DataCore { get; set; }
        public virtual string SenderName { get; set; }
        public virtual Guid SenderId { get; set; }
        public virtual Guid TargetId { get; set; }
    }
    public static class WebSocketHelper
    {
        private static object obj = new object();

        /// <summary>
        /// Socket存放位置
        /// </summary>
        private static ConcurrentDictionary<string, WebSocketModel> SocketList { get; set; } = new ConcurrentDictionary<string, WebSocketModel>();

        #region 发送消息核心
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="buffer">消息内容</param>
        /// <param name="msgType">消息类型</param>
        /// <param name="endOfMessage">是否全部发送</param>
        /// <returns></returns>
        public async static Task<bool> SendMsgCoreAsync(WebSocket webSocket, byte[] buffer, WebSocketMessageType msgType = WebSocketMessageType.Text, bool endOfMessage = true)
        {
            await Task.Factory.StartNew(() =>
            {
                webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), msgType, endOfMessage, CancellationToken.None);//发送数据
            }).ContinueWith((n) =>
            {
                //用于写回调
                Console.WriteLine("");
            });
            var actionResult = true;
            return actionResult;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="msg"></param>
        /// <param name="msgType"></param>
        /// <param name="endOfMessage"></param>
        /// <returns></returns>
        public async static Task<bool> SendMsgAsync(WebSocket webSocket, string msg, Encoding encoding, WebSocketMessageType msgType = WebSocketMessageType.Text, bool endOfMessage = true)
        {
            var buffer = encoding.GetBytes(msg);
            return await SendMsgCoreAsync(webSocket, buffer, msgType, endOfMessage);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="msg"></param>
        /// <param name="msgType"></param>
        /// <param name="endOfMessage"></param>
        /// <returns></returns>
        public async static Task<bool> SendMsgAsync(WebSocket webSocket, string msg, WebSocketMessageType msgType = WebSocketMessageType.Text, bool endOfMessage = true)
        {
            return await SendMsgAsync(webSocket, msg, Encoding.Default, msgType, endOfMessage);
        }
        #endregion

        /// <summary>
        /// 点对点发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="msg"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async static Task<bool> PointToPointSendMsgAsync(MessageModel msg)
        {
            var targetSocket = SocketList.FirstOrDefault(u => u.Key == msg.TargetId.ToString());
            if (targetSocket.Value != null)
            {
                var data = $"send hello word to {msg.TargetId}";
                await SendMsgAsync(targetSocket.Value.Sk, data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="webSocket">自己的Socket</param>
        /// <param name="msg"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async static Task<bool> BroadCastSendMsgAsync(MessageModel msg)
        {
            var actionResult = false;
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    foreach (var item in SocketList.Values)
                    {
                        var data = $"send BroadCastSend to {item.UserId}";
                        await SendMsgAsync(item.Sk, data);
                    }
                });
                actionResult = true;
            }
            catch (Exception)
            {
            }
            return actionResult;
        }

        /// <summary>
        /// 监听接收
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public async static Task ReceiveAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!webSocket.CloseStatus.HasValue)//连接中
            {
                //【记录发送内容】【点对点】记录数据库，标识,【群聊】群聊信息表，表连接查询没有发送过的，发送过后记录日志
                //【点对点通讯】判断是否是本机的客户端，是的话直接发送，不是的话找到对应的路由发过去
                //【群聊】找出本机的异步发送，分别找出服务器然后发送
                var msgValue = Encoding.Default.GetString(buffer);//
                var msg = JsonConvert.DeserializeObject<MessageModel>(msgValue);
                var appServerList = Commont.RedisCommont.StackExchangeRedisHelper.redisClient.GetStringKey<Dictionary<string, string>>("APP-ServerList");
                if (appServerList.ContainsKey(msg.TargetId.ToString()))
                {
                    var host = appServerList.FirstOrDefault(u => u.Key == msg.TargetId.ToString()).Value;
                    RabbitMQProducterHelper.CreateInstance().SendMessage(new ConsumerCmd() { Param = msgValue });
                    await SendMsgAsync(webSocket, "11111111111");//给自己发送消息
                }
                //重置消息容器
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            #region 断开或者异常退出后操作
            lock (obj)
            {
                var key = SocketList.Where(c => c.Value.Sk == webSocket).FirstOrDefault().Key ?? "";
                if (!string.IsNullOrEmpty(key))
                {
                    SocketList.Remove(key);
                    //移除
                    var appServerList = Commont.RedisCommont.StackExchangeRedisHelper.redisClient.GetStringKey<Dictionary<string, string>>("APP-ServerList");
                    if (appServerList.ContainsKey(key))
                    {
                        appServerList.Remove(key);
                        Commont.RedisCommont.StackExchangeRedisHelper.redisClient.SetStringKey("APP-ServerList", JsonConvert.SerializeObject(appServerList));
                    }

                }
            }
            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
            #endregion

        }

        /// <summary>
        /// 【1】初始化 WebSocket
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public async static Task InitSocket(string userId, WebSocket webSocket)
        {
            var response = new MessageModel
            {
                DataType = DataType.Json,
                SendType = WebSocketSendType.SystemMsg,
                SenderName = "Server",
                DataCore = "connected success"
            };
            if (!SocketList.Keys.Any(u => u == userId))//后期要用到 Redis 进行分布式判断
            {
                var addScoket = SocketList.TryAdd(userId, new WebSocketModel
                {
                    Id = Guid.NewGuid(),
                    Sk = webSocket,
                    UserId = userId
                });
                if (addScoket)
                {
                    response.DataCore = JsonConvert.SerializeObject(new { Code = 200, Msg = "connected sussecc", UserId = userId });  //连接成功，发送反馈
                }
            }
            byte[] byteArray = Encoding.Default.GetBytes(JsonConvert.SerializeObject(response));
            await webSocket.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// 【2】消费者事件接收
        /// </summary>
        /// <param name="consumerCmd"></param>
        public static void ConsumerLogicDel(ConsumerCmd consumerCmd)
        {
            var result = false;
            if (!string.IsNullOrEmpty(consumerCmd.Param))
            {
                var msg = JsonConvert.DeserializeObject<MessageModel>(consumerCmd.Param);
                switch (msg.SendType)
                {
                    case WebSocketSendType.Broadcast:
                        result = BroadCastSendMsgAsync(msg).Result;
                        break;
                    case WebSocketSendType.PointToPoint:
                        result = PointToPointSendMsgAsync(msg).Result;
                        break;
                }
            }
        }


    }
}
