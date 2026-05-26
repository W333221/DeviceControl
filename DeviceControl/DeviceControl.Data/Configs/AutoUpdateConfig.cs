using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs
{
    public class AutoUpdateConfig
    {
        public string AppId { get; set; }
        public string TargetGuard { get; set; }
        public string Url { get; set; }
        public string MQTTConfigName { get; set; }
        public string UpdateTime { get; set; }
        public List<string> Exclude { get; set; }
        public List<string> ExcludeDir { get; set; }
    }
}
