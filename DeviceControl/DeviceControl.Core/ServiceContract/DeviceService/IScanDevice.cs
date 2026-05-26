using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public delegate void OnScanCodedDelegate(IDevice device, string code);
    /// <summary>
    /// 串口扫描器
    /// </summary>
    public interface IScanDevice : IDevice
    {
        SerialConfig Config { get; }

        event OnScanCodedDelegate OnScanCoded;

        void Subscribe();
    }
}
