using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs
{
    /// <summary>
    /// 工位配置
    /// </summary>
    public class StationConfig
    {
        public string Title { get; set; }
        public string LineClass { get; set; }
        public string LineName { get; set; }
        public string LineCode { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string ClientCode { get; set; }
    }
}
