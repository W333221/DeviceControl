using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public interface IRFIDDevice : IDevice
    {
        bool IsOpen { get; set; }
        /// <summary>
        /// 标签响应事件
        /// </summary>
        Action<IDevice, int, string> OnRFIDReaded { get; set; }

        /// <summary>
        /// 手动读取标签
        /// </summary>
        /// <returns></returns>
        Task<string> ReadAsync();

        /// <summary>
        /// 写标签
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <param name="address">起始地址</param>
        /// <returns></returns>
        Task<bool> WriteAsync(string data, int address = 0);

        ///// <summary>
        ///// 开始读取
        ///// </summary>
        ///// <returns></returns>
        //bool StartRead();

        ///// <summary>
        ///// 自动读取
        ///// </summary>
        ///// <returns></returns>
        //bool StopRead();
        bool StartAutoReading();


        /// <summary>
        /// 停止自动读取
        /// </summary>
        /// <returns></returns>
        bool StopAutoReading();

        /// <summary>
        /// 读前校验
        /// </summary>
        Task ReadCheckAsync();



    }
}
