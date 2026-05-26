using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Models
{
    public class WsResponseEnvelope
    {
        public string Type { get; set; } = "response";
        public string RequestId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
