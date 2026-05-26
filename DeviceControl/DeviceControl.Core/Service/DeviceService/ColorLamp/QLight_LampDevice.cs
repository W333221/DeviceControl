using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data;
using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using DeviceControl.Data.Enum;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService.ColorLamp
{
    public class QLight_LampDevice : IColorLampDevice
    {
        [DllImport("Ux64_dllc.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Usb_Qu_Open();
        [DllImport("Ux64_dllc.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Usb_Qu_Close();
        [DllImport("Ux64_dllc.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Usb_Qu_write(byte Q_index, byte Q_type, byte[] pQ_data);
        [DllImport("Ux64_dllc.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Usb_Qu_Getstate();
        const byte lampOff = 0;


        #region Device
        public string DeviceId { get; set; }
        public string FriendlyName { get; set; }
        public bool DeviceStatus { get; set; }
        public string Class { get; set; } = DeviceClass.Light;
        public Action<IDevice, bool> OnStateChanged { get; set; }
        public string DeviceType { get; set; }
        #endregion

        ColorLampConfig Config { get; set; }

        public QLight_LampDevice()
        {
            IConfiguration configuration = IOC.Instance.ServiceProvider.Resolve<IConfiguration>();
            var qLightConfig = configuration.GetSection("AppSettings:DeviceConfig:ColorLampConfig").Get<ColorLampConfig>();

            Config = qLightConfig ?? throw new ArgumentNullException(nameof(qLightConfig));
            DeviceId = qLightConfig.DeviceId;
            FriendlyName = qLightConfig.FriendlyName;
            DeviceType = qLightConfig.DeviceType;
        }

        //public QLight_LampDevice(ColorLampConfig qLightConfig)
        //{
        //    Config = qLightConfig ?? throw new ArgumentNullException(nameof(qLightConfig));
        //    DeviceId = qLightConfig.DeviceId;
        //    FriendlyName = qLightConfig.FriendlyName;
        //    DeviceType = qLightConfig.DeviceType;
        //}

        public bool ReSet()
        {
            bool isSuccess;
            byte[] bbb = new byte[6];

            bbb[0] = lampOff;
            bbb[1] = lampOff;
            bbb[2] = lampOff;
            bbb[3] = lampOff;
            bbb[4] = lampOff;
            bbb[5] = lampOff;

            isSuccess = Usb_Qu_write(0, 0, bbb);
            return isSuccess;
        }

        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState)
        {
            bool isSuccess;
            byte[] bbb = new byte[6];

            bbb[0] = RedLampState;
            bbb[1] = YellowLampState;
            bbb[2] = GreenLampState;
            bbb[3] = lampOff;
            bbb[4] = lampOff;
            bbb[5] = SoundState;

            isSuccess = Usb_Qu_write(0, 0, bbb);
            return isSuccess;
        }

        public bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState, byte BlueLampState)
        {
            //关灯
            bool isSuccess;
            byte[] bbb = new byte[6];

            bbb[0] = lampOff;
            bbb[1] = lampOff;
            bbb[2] = lampOff;
            bbb[3] = lampOff;
            bbb[4] = lampOff;
            bbb[5] = lampOff;

            isSuccess = Usb_Qu_write(0, 0, bbb);
            return isSuccess;
        }

        public bool Init()
        {
            DeviceStatus = true;
            OnStateChanged?.Invoke(this, DeviceStatus);
            return true;
        }

        public async Task InitAsync()
        {
            await Task.Run(Init);
        }

        public void Close()
        {

        }
    }
}
