using DeviceControl.WebHost.Realtime.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Session
{
    public class WebSocketSessionManager
    {
        private readonly ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _clients = new();
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public string Add(System.Net.WebSockets.WebSocket socket)
        {
            var id = Guid.NewGuid().ToString("N");
            _clients.TryAdd(id, socket);
            return id;
        }

        public async Task RemoveAsync(string sessionId)
        {
            if (_clients.TryRemove(sessionId, out var socket))
            {
                try
                {
                    if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    }
                }
                catch
                {
                    // ignore
                }
                finally
                {
                    socket.Dispose();
                }
            }
        }

        public async Task SendAsync(string sessionId, object payload)
        {
            if (!_clients.TryGetValue(sessionId, out var socket))
                return;

            if (socket.State != WebSocketState.Open)
                return;

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        public async Task BroadcastEventAsync(string eventName, object? data)
        {
            var envelope = new WsEventEnvelope
            {
                Event = eventName,
                Data = data,
                Time = DateTime.Now
            };

            var deadSessions = new List<string>();

            foreach (var item in _clients)
            {
                var sessionId = item.Key;
                var socket = item.Value;

                if (socket.State != WebSocketState.Open)
                {
                    deadSessions.Add(sessionId);
                    continue;
                }

                try
                {
                    await SendAsync(sessionId, envelope);
                }
                catch
                {
                    deadSessions.Add(sessionId);
                }
            }

            foreach (var dead in deadSessions.Distinct())
            {
                await RemoveAsync(dead);
            }
        }

        public int Count => _clients.Count;
    }
}