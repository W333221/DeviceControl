using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs.DeviceConfig.DeviceConfig
{
    public class SerialConfig : DeviceConfigBase
    {
        public string PortName { get; set; } = string.Empty;

        public int BuadRate { get; set; }

        public int DataBits { get; set; }

        public string Parity { get; set; } = string.Empty;

        public string StopBits { get; set; } = string.Empty;

        public string Misc { get; set; } = string.Empty;

        public bool Available { get; set; }
    }
}
