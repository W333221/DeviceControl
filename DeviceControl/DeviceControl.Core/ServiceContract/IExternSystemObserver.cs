using DeviceControl.Core.ServiceContract.DeviceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract
{
    /// <summary>
    /// 监听服务，负责将外部系统的消息(各种设备、OPC网关)传递
    /// </summary>
    public interface IExternSystemObserver
    {
        /// <summary>
        /// 维持与各设备实例的引用，方便外部调用各设备的公共方法。
        /// </summary>
        List<IDevice> Devices { get; set; }

        /// <summary>
        /// 三色灯初始化
        /// </summary>
        /// <returns></returns>
        Task<IExternSystemObserver> StartColorLampAsync();

        /// <summary>
        /// 关闭三色灯
        /// </summary>
        /// <returns></returns>
        Task<IExternSystemObserver> StopColorLampAsync();

        /// <summary>
        /// 打开接近开关模块
        /// </summary>
        /// <returns></returns>
        public IExternSystemObserver StartProximitySwitchModule();

        /// <summary>
        /// 打开接近开关模块
        /// </summary>
        /// <returns></returns>
        public Task<IExternSystemObserver> StartProximitySwitchModuleAsync();
    }
}
