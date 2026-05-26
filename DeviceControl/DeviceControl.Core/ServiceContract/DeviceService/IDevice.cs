using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public interface IDevice
    {

        public Action<IDevice, bool> OnStateChanged { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        string DeviceId { get; set; }
        /// <summary>
        /// 设备分组
        /// 描述设备的类型，例如：色灯、IO模块、RFID等
        /// </summary>
        string Class { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        string FriendlyName { get; set; }
        /// <summary>
        /// 设备状态
        /// 如果有链接方法则这个为链接状态，否则为设备状态
        /// </summary>
        bool DeviceStatus { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        string DeviceType { get; set; }
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        bool Init();
        /// <summary>
        /// 异步初始化设备
        /// </summary>
        Task InitAsync();
        /// <summary>
        /// 关闭设备
        /// </summary>
        void Close();


    }
}
