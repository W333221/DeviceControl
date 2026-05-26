using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs.DeviceConfig.DeviceConfig
{
    /// <summary>
    /// 设备配置基类
    /// </summary>
    public class DeviceConfigBase
    {
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备类型  描述设备的型号
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Available { get; set; }
        /// <summary>
        /// 设备类别 描述设备的类别
        /// </summary>
        public string DeviceClass { get; set; }
    }
}
