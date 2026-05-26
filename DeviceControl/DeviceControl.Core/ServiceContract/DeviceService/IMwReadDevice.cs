using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    /// <summary>
    /// 明华刷卡服务
    /// </summary>
    public interface IMwReadDevice : IDevice
    {
        /// <summary>
        /// 收到刷卡信息
        /// </summary>
        Action<string> SeekCard { get; set; }
    }
}
