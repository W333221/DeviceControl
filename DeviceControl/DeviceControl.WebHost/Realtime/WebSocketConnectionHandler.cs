using DeviceControl.WebHost.Realtime.Models;
using DeviceControl.WebHost.Realtime.Router;
using DeviceControl.WebHost.Realtime.Session;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace DeviceControl.WebHost.Realtime
{
    public class WebSocketConnectionHandler
    {
        private readonly WebSocketSessionManager _sessionManager;
        private readonly WebSocketCommandRouter _router;

        private readonly JsonSerializerOptions _jsonOptions =
            new(JsonSerializerDefaults.Web);

        public WebSocketConnectionHandler(
            WebSocketSessionManager sessionManager,
            WebSocketCommandRouter router)
        {
            _sessionManager = sessionManager;
            _router = router;
        }

        public async Task HandleAsync(WebSocket socket)
        {
            var sessionId = _sessionManager.Add(socket);

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var message = await ReceiveFullMessageAsync(socket);

                    if (message == null)
                        break;

                    var response = await ProcessMessage(sessionId, message);

                    await _sessionManager.SendAsync(sessionId, response);
                }
            }
            catch (WebSocketException)
            {
                // 客户端断开
            }
            catch (Exception ex)
            {
                await _sessionManager.SendAsync(sessionId, new WsResponseEnvelope
                {
                    Success = false,
                    Message = $"Server error: {ex.Message}"
                });
            }
            finally
            {
                await _sessionManager.RemoveAsync(sessionId);
            }
        }

        #region ⭐ 核心：完整收包（生产级）

        private async Task<string?> ReceiveFullMessageAsync(WebSocket socket)
        {
            var buffer = new byte[8 * 1024];
            using var ms = new MemoryStream();

            while (true)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    return null;

                if (result.MessageType != WebSocketMessageType.Text)
                    continue;

                ms.Write(buffer, 0, result.Count);

                // ⭐ 关键：判断是否消息结束
                if (result.EndOfMessage)
                    break;
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        #endregion

        #region ⭐ 业务处理

        private async Task<WsResponseEnvelope> ProcessMessage(string sessionId, string json)
        {
            try
            {
                var command = JsonSerializer.Deserialize<WsCommandEnvelope>(json, _jsonOptions);

                if (command == null)
                {
                    return Fail("", "命令不能为空");
                }

                if (!string.Equals(command.Type, "command", StringComparison.OrdinalIgnoreCase))
                {
                    return Fail(command.RequestId, "非法消息类型");
                }

                return await _router.RouteAsync(command);
            }
            catch (JsonException ex)
            {
                return Fail("", $"JSON解析失败: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Fail("", $"处理异常: {ex.Message}");
            }
        }

        private static WsResponseEnvelope Fail(string requestId, string message)
        {
            return new WsResponseEnvelope
            {
                RequestId = requestId,
                Success = false,
                Message = message
            };
        }

        #endregion
    }
}