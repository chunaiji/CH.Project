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

        /// <summary>
        /// 点对点发送消息
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="msg"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async static Task<bool> PointToPointSendMsgAsync(WebSocket webSocket, MessageModel msg, WebSocketReceiveResult result, byte[] buffer)
        {
            var actionResult = false;
            if (SocketList.Any(u => u.Key == msg.TargetId.ToString()))
            {
                var target = SocketList.Where(u => u.Key == msg.TargetId.ToString()).FirstOrDefault();
                if (target.Value != null)
                {
                    try
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给自己发消息表示我的消息已送达
                        });
                        await Task.Factory.StartNew(() =>
                        {
                            target.Value.Sk.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给对方发消息表示
                        });
                        actionResult = true;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return actionResult;
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="msg"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async static Task<bool> BroadCastSendMsgAsync(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var actionResult = false;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给自己发消息表示我的消息已送达
                });
                Task task = new Task(() =>
                {
                    foreach (var item in SocketList.Values)
                    {
                        item.Sk.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给对方发消息表示
                    }
                });
                await Task.Factory.ContinueWhenAny(new Task[] { task },
                               (m) =>
                               {
                                   Console.WriteLine("发送成功");
                               });
                actionResult = true;
            }
            catch (Exception)
            {

                throw;
            }
            return actionResult;
        }

        /// <summary>
        /// 群发
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="targetList"></param>
        /// <param name="result"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async static Task<bool> GroupChatSendMsgAsync(WebSocket webSocket, List<WebSocket> targetList, WebSocketReceiveResult result, byte[] buffer)
        {
            var actionResult = false;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给自己发消息表示我的消息已送达
                });
                Task task = new Task(() =>
                {
                    foreach (var item in targetList)
                    {
                        item.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);//给对方发消息表示
                    }
                });
                await Task.Factory.ContinueWhenAny(new Task[] { task },
                     (m) =>
                     {
                         Console.WriteLine("发送成功");
                     });
                actionResult = true;
            }
            catch (Exception ex)
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
                //拆包
                var msgValue = Encoding.Default.GetString(buffer);
                var msg = JsonConvert.DeserializeObject<MessageModel>(msgValue);

                var appServerList = Commont.RedisCommont.StackExchangeRedisHelper.redisClient.GetStringKey<Dictionary<string, string>>("APP-ServerList");
                if (appServerList.ContainsKey(msg.TargetId.ToString()))
                {
                    var host = appServerList.FirstOrDefault(u => u.Key == msg.TargetId.ToString()).Value;
                    Commont.RedisCommont.StackExchangeRedisHelper.redisClient.Publish($"Server_topic_{host}", msgValue);
                }
                //switch (msg.SendType)
                //{
                //    case WebSocketSendType.Broadcast:
                //        await BroadCastSendMsgAsync(webSocket, result, buffer);
                //        break;
                //    case WebSocketSendType.PointToPoint:
                //        await PointToPointSendMsgAsync(webSocket, msg, result, buffer);
                //        break;
                //}
                //重置消息容器
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
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
        }

        /// <summary>
        /// 初始化 WebSocket
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
    }
}
