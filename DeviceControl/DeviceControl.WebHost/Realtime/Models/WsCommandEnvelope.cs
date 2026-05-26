using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Models
{
    public class WsCommandEnvelope
    {
        public string Type { get; set; } = string.Empty;      // command
        public string Action { get; set; } = string.Empty;    // notification.ready
        public string RequestId { get; set; } = Guid.NewGuid().ToString("N");
        public JsonElement Data { get; set; }
    }
}
