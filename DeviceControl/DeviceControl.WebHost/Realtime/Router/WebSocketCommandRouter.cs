using DeviceControl.WebHost.Realtime.Handlers;
using DeviceControl.WebHost.Realtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Router
{
    public class WebSocketCommandRouter
    {
        private readonly IEnumerable<IWsCommandHandler> _handlers;

        public WebSocketCommandRouter(IEnumerable<IWsCommandHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task<WsResponseEnvelope> RouteAsync(WsCommandEnvelope command)
        {
            var handler = _handlers.FirstOrDefault(x => x.CanHandle(command.Action));

            if (handler == null)
            {
                return new WsResponseEnvelope
                {
                    RequestId = command.RequestId,
                    Success = false,
                    Message = $"No handler found for action: {command.Action}"
                };
            }

            return await handler.HandleAsync(command);
        }
    }
}