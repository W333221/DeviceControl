using DeviceControl.Core.ServiceContract;
using DeviceControl.WebHost.Extensions;
using DeviceControl.WebHost.HttpApi.Dtos;
using DeviceControl.WebHost.Realtime.Models;
using System.Text.Json;

namespace DeviceControl.WebHost.Realtime.Handlers
{
    public class NotificationWsCommandHandler : IWsCommandHandler
    {
        private readonly INotificationService _notificationService;

        public NotificationWsCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public bool CanHandle(string action)
        {
            return action.StartsWith("notification.", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<WsResponseEnvelope> HandleAsync(WsCommandEnvelope command)
        {
            try
            {
                switch (command.Action.ToLower())
                {
                    case "notification.ready":
                        {
                            var req = command.Data.DeserializeSafe<NotificationRequest>();
                            if (req == null)
                                return Fail(command.RequestId, "Invalid request data.");

                            await _notificationService.ReadyNoticeAsync(req.Level, req.Message);
                            return Success(command.RequestId, "ReadyNotice executed.");
                        }

                    case "notification.success":
                        {
                            var req = command.Data.DeserializeSafe<NotificationRequest>();
                            if (req == null)
                                return Fail(command.RequestId, "Invalid request data.");

                            await _notificationService.SuccseNoticeAsync(req.Level, req.Message);
                            return Success(command.RequestId, "SuccseNotice executed.");
                        }

                    case "notification.error":
                        {
                            var req = command.Data.DeserializeSafe<NotificationRequest>();
                            if (req == null)
                                return Fail(command.RequestId, "Invalid request data.");

                            //Exception? ex = null;
                            //if (!string.IsNullOrWhiteSpace(req.ExceptionMessage))
                            //{
                            //    ex = new Exception(req.ExceptionMessage);
                            //}

                            await _notificationService.ErrNoticeAsync(req.Level, req.Message);
                            return Success(command.RequestId, "ErrNotice executed.");
                        }

                    case "notification.common":
                        {
                            var req = command.Data.DeserializeSafe<NotificationRequest>();
                            if (req == null)
                                return Fail(command.RequestId, "Invalid request data.");

                            await _notificationService.CommonNoticeAsync(req.Level, req.Message);
                            return Success(command.RequestId, "CommonNotice executed.");
                        }

                    //case "notification.log-button":
                    //    {
                    //        var req = command.Data.DeserializeSafe<LogButtonRequest>();
                    //        if (req == null)
                    //            return Fail(command.RequestId, "Invalid request data.");

                    //        var result = await _notificationService.LogButtonAsync(
                    //            req.Type, req.Message, req.UserID, req.UserName);

                    //        return Success(command.RequestId, "LogButton executed.", result);
                    //    }

                    //case "notification.log-api":
                    //    {
                    //        var req = command.Data.DeserializeSafe<LogApiRequest>();
                    //        if (req == null)
                    //            return Fail(command.RequestId, "Invalid request data.");

                    //        await _notificationService.LogAPIAsync(
                    //            req.Type, req.Message, req.Content, req.IsLogInformation);

                    //        return Success(command.RequestId, "LogAPI executed.");
                    //    }

                    default:
                        return Fail(command.RequestId, $"Unsupported action: {command.Action}");
                }
            }
            catch (Exception ex)
            {
                return Fail(command.RequestId, ex.Message);
            }
        }

        private static WsResponseEnvelope Success(string requestId, string message, object? data = null)
        {
            return new WsResponseEnvelope
            {
                RequestId = requestId,
                Success = true,
                Message = message,
                Data = data
            };
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
    }
}