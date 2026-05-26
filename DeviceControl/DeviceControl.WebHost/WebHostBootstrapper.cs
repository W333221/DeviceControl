using DeviceControl.Core.ServiceContract;
using DeviceControl.Data.Configs;
using DeviceControl.WebHost.Abstractions;
using DeviceControl.WebHost.HttpApi.Dtos;
using DeviceControl.WebHost.HttpApi.Results;
using DeviceControl.WebHost.Realtime;
using DeviceControl.WebHost.Realtime.Handlers;
using DeviceControl.WebHost.Realtime.Models;
using DeviceControl.WebHost.Realtime.Router;
using DeviceControl.WebHost.Realtime.Session;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace DeviceControl.WebHost
{
    public class WebHostBootstrapper : IWebHostBootstrapper
    {
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly WebSocketCommandRouter _router;

        private HttpListener? _listener;
        private CancellationTokenSource? _cts;

        private readonly WebSocketSessionManager _wsManager = new();
        private readonly WebSocketConnectionHandler _wsHandler;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        public WebHostBootstrapper(
            INotificationService notificationService,
            IConfiguration configuration)
        {
            _notificationService = notificationService;
            _configuration = configuration;

            // 手动初始化 Router
            _router = new WebSocketCommandRouter(new List<IWsCommandHandler>
            {
                new NotificationWsCommandHandler(_notificationService)
            });
            _wsHandler = new WebSocketConnectionHandler(
                _wsManager,
                _router
            );
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var config = _configuration.GetSection("AppSettings:AppServerConfig")
                .Get<AppServerConfig>();

            string prefix = $"http://127.0.0.1:{config.ServerPort}/";

            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);

            _listener.Start();

            _cts = new CancellationTokenSource();

            Log.Information($"http服务启动成功：{prefix}");

            _ = Task.Run(() => ListenLoop(_cts.Token));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _cts?.Cancel();

            _listener?.Stop();
            _listener?.Close();

            return Task.CompletedTask;
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener!.GetContextAsync();

                    _ = Task.Run(() => HandleRequest(context));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "监听异常");
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                var req = context.Request;

                if (req.IsWebSocketRequest)
                {
                    await HandleWebSocket(context);
                    return;
                }

                switch (req.Url!.AbsolutePath.ToLower())
                {
                    case "/api/system/ping":
                        await WriteJson(context, ApiResult.Ok($"服务启动成功:{DateTime.Now}"));
                        break;
                    case "/api/notification/ready":
                        await HandleReadyNotice(context);
                        break;
                    case "/api/notification/success":
                        await HandleSuccessNotice(context);
                        break;

                    case "/api/notification/error":
                        await HandleErrorNotice(context);
                        break;

                    case "/api/notification/common":
                        await HandleCommonNotice(context);
                        break;

                    default:
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "请求处理异常");
            }
        }       
        private async Task HandleWebSocket(HttpListenerContext context)
        {
            var wsContext = await context.AcceptWebSocketAsync(null);
            var socket = wsContext.WebSocket;

            await _wsHandler.HandleAsync(socket);
        }

        #region http 处理
        // 提取公共方法：读取并反序列化 NotificationRequest
        private async Task<NotificationRequest?> ReadNotificationRequestAsync(HttpListenerContext context)
        {
            if (!context.Request.HasEntityBody)
                return null;

            using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            //                            var req = command.Data.DeserializeSafe<NotificationRequest>();

            return JsonSerializer.Deserialize<NotificationRequest>(body, _jsonOptions);
        }

        private async Task HandleReadyNotice(HttpListenerContext context)
        {
            var request = await ReadNotificationRequestAsync(context);
            if (request == null)
            {
                await WriteJson(context, ApiResult.Fail("无效的请求体"), 400);
                return;
            }

            await _notificationService.ReadyNoticeAsync(request.Level, request.Message);
            await WriteJson(context, ApiResult.Ok("执行成功"));
        }

        private async Task HandleSuccessNotice(HttpListenerContext context)
        {
            var request = await ReadNotificationRequestAsync(context);
            if (request == null)
            {
                await WriteJson(context, ApiResult.Fail("无效的请求体"), 400);
                return;
            }

            await _notificationService.SuccseNoticeAsync(request.Level, request.Message);
            await WriteJson(context, ApiResult.Ok("执行成功"));
        }

        private async Task HandleErrorNotice(HttpListenerContext context)
        {
            var request = await ReadNotificationRequestAsync(context);
            if (request == null)
            {
                await WriteJson(context, ApiResult.Fail("无效的请求体"), 400);
                return;
            }

            await _notificationService.ErrNoticeAsync(request.Level, request.Message);
            await WriteJson(context, ApiResult.Ok("执行成功"));
        }

        private async Task HandleCommonNotice(HttpListenerContext context)
        {
            var request = await ReadNotificationRequestAsync(context);
            if (request == null)
            {
                await WriteJson(context, ApiResult.Fail("无效的请求体"), 400);
                return;
            }

            await _notificationService.CommonNoticeAsync(request.Level, request.Message);
            await WriteJson(context, ApiResult.Ok("执行成功"));
        }

        private async Task WriteJson(HttpListenerContext context, object obj, int statusCode = 200)
        {
            var json = JsonSerializer.Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(json);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = bytes.Length;

            await context.Response.OutputStream.WriteAsync(bytes);
            context.Response.Close();
        }
        #endregion

    }

}