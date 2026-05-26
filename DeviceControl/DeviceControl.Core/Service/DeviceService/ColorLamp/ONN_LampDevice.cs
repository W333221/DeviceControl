using DeviceControl.Core.Message.Device;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data;
using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using DeviceControl.Data.Enum;
using GodSharp.SerialPort;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService.ColorLamp
{
    public class ONN_LampDevice : IColorLampDevice
    {
        static GodSerialPort godSerialPort = null;
        ColorLampConfig Config { get; set; }

        #region Device
        public Action<IDevice, bool> OnStateChanged { get; set; }
        public string DeviceType { get; set; }
        public string DeviceId { get; set; }
        public string FriendlyName { get; set; }
        public bool DeviceStatus { get; set; }
        public string Class { get; set; } = DeviceClass.Light;
        #endregion

        public ONN_LampDevice()
        {
            IConfiguration configuration = IOC.Instance.ServiceProvider.Resolve<IConfiguration>();
            var qLightConfig = configuration.GetSection("AppSettings:DeviceConfig:ColorLampConfig").Get<ColorLampConfig>();

            Config = qLightConfig ?? throw new ArgumentNullException(nameof(qLightConfig));
            DeviceId = qLightConfig.DeviceId;
            FriendlyName = qLightConfig.FriendlyName;
            DeviceStatus = false;
            DeviceType = qLightConfig.DeviceType;
        }
        //public ONN_LampDevice(ColorLampConfig qLightConfig)
        //{
        //    Config = qLightConfig ?? throw new ArgumentNullException(nameof(qLightConfig));
        //    DeviceId = qLightConfig.DeviceId;
        //    FriendlyName = qLightConfig.FriendlyName;
        //    DeviceStatus = false;
        //    DeviceType = qLightConfig.DeviceType;
        //}

        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState)
        {
            bool OK = false;
            byte LightState = 0x01;//1: 长亮状态 2:闪烁  默认长亮
            byte ColorNum = 0x01;//颜色状态 1不闪 2红 3黄 4绿 5蓝
            byte Sound = 0x01;//是否蜂鸣
            try
            {

                if (SoundState == 5)
                {
                    Sound = 0x03;
                }
                if (RedLampState != 0)
                {
                    ColorNum = 0x02;
                    if (RedLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                if (YellowLampState != 0)
                {
                    ColorNum = 0x03;
                    if (YellowLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                if (GreenLampState != 0)
                {
                    ColorNum = 0x04;
                    if (GreenLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                //三色灯报警
                //命令头0xFF
                //灯颜色（1关灯，2红色，3黄色，4绿色，5 蓝色）
                //蜂鸣（1关闭，2小声模式，3大声模式）
                //闪光（1、关闭闪光，2、0.85秒闪，3、1.7秒闪，4、2.5秒闪）
                //命令尾 0xAA
                byte[] sendMsgs = { 0xFF, ColorNum, Sound, LightState, 0xAA };
                Task.Run(() =>
                {
                    Log.Information("调用三色灯");
                    if (godSerialPort.IsOpen)
                    {
                        return godSerialPort?.Write(sendMsgs) > 0;
                    }
                    else
                    {
                        return false;
                    }
                });

                OK = true;
            }
            catch (Exception ex)
            {
                Log.Information($"变更三色灯颜色异常：【{ex.Message}】");
                Log.Error($"{ex.Message}\r\n{ex.StackTrace}");
                OK = false;
            }
            finally
            {

            }
            return OK;
        }
        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState, byte BlueLampState)
        {
            bool OK = false;
            byte LightState = 0x01;//1: 长亮状态 2:闪烁  默认长亮
            byte ColorNum = 0x01;//颜色状态 1不闪 2红 3黄 4绿 5蓝
            byte Sound = 0x01;//是否蜂鸣
            try
            {

                if (SoundState == 5)
                {
                    Sound = 0x03;
                }
                if (RedLampState != 0)
                {
                    ColorNum = 0x02;
                    if (RedLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                if (YellowLampState != 0)
                {
                    ColorNum = 0x03;
                    if (YellowLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                if (GreenLampState != 0)
                {
                    ColorNum = 0x04;
                    if (GreenLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                if (BlueLampState != 0)
                {
                    ColorNum = 0x05;
                    if (BlueLampState == 2)
                    {
                        LightState = 0x02;
                    }
                }
                //三色灯报警
                //命令头0xFF
                //灯颜色（1关灯，2红色，3黄色，4绿色，5 蓝色）
                //蜂鸣（1关闭，2小声模式，3大声模式）
                //闪光（1、关闭闪光，2、0.85秒闪，3、1.7秒闪，4、2.5秒闪）
                //命令尾 0xAA
                byte[] sendMsgs = { 0xFF, ColorNum, Sound, LightState, 0xAA };
                if (godSerialPort?.IsOpen == true)
                {
                    return godSerialPort?.Write(sendMsgs) > 0;
                }
                else
                {
                    return false;
                }
                OK = true;

            }
            catch (Exception ex)
            {
                Log.Information($"变更三色灯颜色异常：【{ex.Message}】");
                Log.Error($"{ex.Message}\r\n{ex.StackTrace}"); OK = false;
            }
            finally
            {

            }
            return OK;
        }

        public bool ReSet()
        {
            byte LightState = 0x01;//1: 长亮状态 2:闪烁  默认长亮
            byte ColorNum = 0x01;//颜色状态 1不闪 2红 3黄 4绿 5蓝
            byte Sound = 0x01;//是否蜂鸣
            byte[] sendMsgs = { 0xFF, ColorNum, Sound, LightState, 0xAA };
            if (godSerialPort == null)
            {
                Log.Information("灯光设备未连接！！！！");
                return false;
            }
            if (godSerialPort.IsOpen)
            {
                return godSerialPort?.Write(sendMsgs) > 0;
            }
            else
            {
                return false;
            }
        }

        public bool Init()
        {
            try
            {
                godSerialPort = new GodSerialPort(Config.PortName, Config.BaudRate, 0);
                if (godSerialPort != null)
                {
                    DeviceStatus = godSerialPort.Open();
                }
                else
                {
                    DeviceStatus = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("三色灯开启失败！");
                DeviceStatus = false;
            }
            OnStateChanged?.Invoke(this, DeviceStatus);
            return DeviceStatus;
        }

        public async Task InitAsync()
        {
            await Task.Run(Init);
        }

        public void Close()
        {
            godSerialPort?.Close();
            DeviceStatus = false;
            OnStateChanged?.Invoke(this, DeviceStatus);
        }
    }
}
