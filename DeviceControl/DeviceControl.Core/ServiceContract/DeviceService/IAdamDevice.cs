using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public delegate void OnSignalChangedDelegate(IDevice device, string key, bool value);
    /// <summary>
    /// IO模块
    /// </summary>
    public interface IAdamDevice : IDevice
    {
        /// <summary>
        /// 输入信号变化Action
        /// </summary>
        event OnSignalChangedDelegate OnSignalDIChanged;
        /// <summary>
        /// 输出信号变化Action
        /// </summary>
        event OnSignalChangedDelegate OnSignalDOChanged;

        /// <summary>
        /// DI读取指定通道号的值
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        bool DIReadByChannel(int channel);
        /// <summary>
        /// DI读取指定通道名称的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool DIReadByKey(string key);

        /// <summary>
        /// 设置通道状态
        /// </summary>
        /// <param name="pinNum"></param>
        /// <param name=""></param>
        /// <returns></returns>
        bool Set(string key, bool value);
    }
}
