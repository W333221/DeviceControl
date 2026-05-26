using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs.DeviceConfig
{
    public class ProximitySwitchModuleConfig
    {
        public string DeviceId { get; set; }
        public string FriendlyName { get; set; }
        public bool Available { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public int DelayTime { get; set; }
    }
}
