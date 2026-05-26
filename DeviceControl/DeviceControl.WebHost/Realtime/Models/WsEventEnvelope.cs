using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.Realtime.Models
{
    public class WsEventEnvelope
    {
        public string Type { get; set; } = "event";
        public string Event { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.Now;
        public object? Data { get; set; }
    }
}
