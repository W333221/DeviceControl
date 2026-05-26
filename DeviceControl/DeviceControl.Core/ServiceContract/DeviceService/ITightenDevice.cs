using DeviceControl.Core.Service.DeviceService;
using DeviceControl.Data.Model.Tighten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    /// <summary>
    /// 拧紧控制器HAL
    /// 正对开放协议
    /// </summary>
    public interface ITightenDevice : IDevice
    {
        #region "事件"区
        //<ToolId,拧紧数据>
        /// <summary>
        /// 接收单轴拧紧数据
        /// </summary>
        //Action<IDevice, TightenData> OnReceiveTightenData { get; set; }
        Func<IDevice, TightenData, Task> OnReceiveTightenData { get; set; }
        ///// <summary>
        ///// 接收多轴拧紧数据
        ///// </summary>
        //Action<IDevice, List<TightenData>> OnReceiveTightenDataList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Action<IDevice, string> OnVehicleNumberAction { get; set; }

        Action<IDevice, int> OnPsetChangedAction { get; set; }

        Action<IDevice, int> OnJobChangedAction { get; set; }
        /// <summary>
        /// 报警信息
        /// </summary>
        public Action<IDevice, string> OnAlarmChangeAction { get; set; }

        Action<IDevice, CurveData> OnReceiveCurveData { get; set; }

        #endregion


        bool SubcribeLastTightenData();

        bool SubscribeVehicleNumber();

        /// <summary>
        /// 设置VIN码(条码)
        /// </summary>
        /// <param name="vin">条码数据</param>
        /// <returns></returns>
        bool SetVehicleNumber(string vin);

        /// <summary>
        /// 设置PSet(ParameterSet)值.即,内置拧紧程序序号
        /// </summary>
        /// <param name="pSetNum"></param>
        /// <returns></returns>
        bool SetPSet(int pSetNum);

        bool SubscribePset();

        bool SelectJob(int jobNum);

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        bool EnableTightenTool();

        /// <summary>
        /// 锁枪
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        bool DisableTightenTool();

        bool AbortJob();
        /// <summary>
        /// 获取Pset信息
        /// </summary>
        /// <returns></returns>
        //List<PSetData> GetPsets();
    }
}
