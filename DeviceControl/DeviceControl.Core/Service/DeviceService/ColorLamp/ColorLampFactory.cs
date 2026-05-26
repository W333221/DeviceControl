using DeviceControl.Core.Message.Device;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using DeviceControl.Data.Enum;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService.ColorLamp
{
    /// <summary>
    /// 三色灯工厂类
    /// </summary>
    public class ColorLampFactory
    {
        IConfiguration _config;
        public ColorLampFactory(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// 获取灯实例
        /// </summary>
        /// <returns>SqlSugarClient instance</returns>
        public IColorLampDevice GetInStance()
        {
            var colorLampConfig = _config.GetSection("AppSettings:DeviceConfig:ColorLampConfig").Get<ColorLampConfig>();
            var deviceType = colorLampConfig.DeviceType;
            if (colorLampConfig == null || string.IsNullOrEmpty(colorLampConfig.PortName) || string.IsNullOrEmpty(colorLampConfig.DeviceType))
            {
                Log.Information("色灯配置错误，请检查配置文件");
                Log.Error("色灯配置错误，请检查配置文件");
                return null;
            }
            if (colorLampConfig.Available == false)
            {
                Log.Information("配置为false，不加载该设备！");
                return new DefaultColorLampDevice();
            }
            Log.Information($"灯设备加载方法");
            if (!string.IsNullOrEmpty(deviceType))
            {
                Log.Information($"加载了{deviceType}设备");
                var deviceMap = $"{GetType().Namespace}.{deviceType}";
                var type = Type.GetType(deviceMap);
                if (type != null)
                {
                    // 查找是否有 ColorLampConfig 参数的构造函数
                    var ctor = type.GetConstructor(new[] { typeof(ColorLampConfig) });
                    if (ctor != null)
                        return (IColorLampDevice)ctor.Invoke(new object[] { colorLampConfig });
                    // 否则尝试无参构造
                    ctor = type.GetConstructor(Type.EmptyTypes);
                    if (ctor != null)
                        return (IColorLampDevice)ctor.Invoke(null);
                }
            }
            return new DefaultColorLampDevice();
        }
    }
    /// <summary>
    /// 空对象模式的三色灯设备实现
    /// </summary>
    public class DefaultColorLampDevice : IColorLampDevice
    {
        public DefaultColorLampDevice()
        {
            OnStateChanged?.Invoke(this, false);
        }
        public Action<IDevice, bool> OnStateChanged { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Class { get; set; } = DeviceClass.Light;
        public string FriendlyName { get; set; } = string.Empty;
        public bool DeviceStatus { get; set; } = false;
        public string DeviceType { get; set; } = string.Empty;

        public void Close() { }
        public bool Init() => false;
        public Task InitAsync() => Task.CompletedTask;
        public bool ReSet() => false;
        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState) => false;
        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState, byte BlueLampState) => false;
    }
}
