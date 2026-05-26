using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public interface IColorLampDevice : IDevice
    {
        /// <summary>
        /// 关闭所有灯光与声音
        /// </summary>
        /// <returns></returns>
        bool ReSet();

        /// <summary>
        /// 控制三色灯和蜂鸣器
        /// </summary>
        /// <param name="RedLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="YellowLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="GreenLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="SoundState">0-OFF, 1-5(Sound Select 1:Fire A-WANG,2:Emergency,3:Ambulance,4:PI-PI-PI,5:Pl_contiune), Else- Don’t change before state</param>
        /// <returns></returns>
        bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState);


        /// <summary>
        /// 控制四色灯和蜂鸣器
        /// </summary>
        /// <param name="RedLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="YellowLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="GreenLampState">0:OFF 1:blink 2:ON</param>
        /// <param name="SoundState">0-OFF, 1-5(Sound Select 1:Fire A-WANG,2:Emergency,3:Ambulance,4:PI-PI-PI,5:Pl_contiune), Else- Don’t change before state</param>
        /// <param name="BlueLampState">0:OFF 1:blink 2:ON</param>
        /// <returns></returns>
        bool LampWrite(byte RedLampState, byte YellowLampState, byte GreenLampState, byte SoundState, byte BlueLampState);
    }
}
