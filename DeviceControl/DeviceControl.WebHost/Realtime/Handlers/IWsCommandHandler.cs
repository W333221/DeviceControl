using DeviceControl.WebHost.Realtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Handlers
{
    public interface IWsCommandHandler
    {
        bool CanHandle(string action);
        Task<WsResponseEnvelope> HandleAsync(WsCommandEnvelope command);
    }
}