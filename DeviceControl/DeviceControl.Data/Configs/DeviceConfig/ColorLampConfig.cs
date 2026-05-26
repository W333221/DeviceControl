using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs.DeviceConfig.DeviceConfig
{
    /// <summary>
    /// 色灯配置
    /// </summary>
    public class ColorLampConfig : DeviceConfigBase
    {
        /// <summary>
        /// COM口
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// 灯塔个数
        /// </summary>
        public int LightHouseNumber { get; set; }
        
    }
}
