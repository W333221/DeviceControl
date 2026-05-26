using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs
{
    public class MqttConfig
    {
        public string Name { get; set; }
        public string MqttHost { get; set; }
        public int MqttPort { get; set; }
        public string MqttUserName { get; set; }
        public string MqttPwd { get; set; }
    }
}
