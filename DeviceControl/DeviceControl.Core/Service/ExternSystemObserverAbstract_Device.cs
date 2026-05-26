using DeviceControl.Core.Service.DeviceService.ProximitySwitchModule;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data.Enum;
using Prism.Ioc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service
{
    public abstract partial class ExternSystemObserverAbstract : IExternSystemObserver
    {
        /// <summary>
        /// 三色灯
        /// </summary>
        protected IColorLampDevice _colorLampDevice;
        private IProximitySwitchModuleDevice _proximitySwitchModuleDevice;
        #region 打开设备
        /// <summary>
        /// 打开三色灯
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IExternSystemObserver> StartColorLampAsync()
        {
            try
            {
                _colorLampDevice = _containerProvider.Resolve<IColorLampDevice>();
                _colorLampDevice.OnStateChanged = Device_OnStateChanged;
                await _colorLampDevice.InitAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "三色灯初始化出错");
            }
            return this;
        }


        /// <summary>
        /// 打开接近开关模块
        /// </summary>
        /// <returns></returns>
        public IExternSystemObserver StartProximitySwitchModule()
        {
            _proximitySwitchModuleDevice = _containerProvider.Resolve<IProximitySwitchModuleDevice>();
            _proximitySwitchModuleDevice.OnProximitySwitchChanged += Device_OnProximitySwitchChanged;
            _proximitySwitchModuleDevice.Init();
            return this;
        }

        /// <summary>
        /// 打开接近开关模块
        /// </summary>
        /// <returns></returns>
        public async Task<IExternSystemObserver> StartProximitySwitchModuleAsync()
        {
            _proximitySwitchModuleDevice = _containerProvider.Resolve<IProximitySwitchModuleDevice>();
            _proximitySwitchModuleDevice.OnProximitySwitchChanged += Device_OnProximitySwitchChanged;
            await _proximitySwitchModuleDevice.InitAsync();
            return this;
        }

        #endregion


        #region 关闭设备     
        /// <summary>
        /// 关闭三色灯
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IExternSystemObserver> StopColorLampAsync()
        {
            await Task.Run(() =>
            {
                if (_colorLampDevice != null && _colorLampDevice.DeviceStatus)
                {
                    _colorLampDevice.Close();
                }
            });
            return this;
        }

        #endregion


        /// <summary>
        /// 设备连接状态
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        protected void Device_OnStateChanged(IDevice device, bool value)
        {
            string deviceInterface = device switch
            {
                //IRFIDDevice => "IRFIDDevice",
                //IScanDevice => "IScanDevice",
                IColorLampDevice => "IColorLampDevice",
            };
            NotificationService.CommonNotice(NotificationLevel.Normal, $"DeviceInterface：{deviceInterface} DeviceState：{value}");
            _eventAggregator.GetEvent<DeviceControl.Core.Message.Device.ConnectStatusEvent>().Publish(
                    new Message.Device.ConnectStatusArg() { DeviceInfo = device, Connected = value }
                );
        }
    }
}
