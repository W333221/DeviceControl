using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Enum
{
    public class DeviceClass
    {
        public const string RFID = "RFID";
        public const string CardReader = "MwCard";
        public const string Scanner = "Scanner";
        public const string IO = "IO模块";
        //public const string SiemensPLC = "SiemensPLC";
        public const string Light = "三色灯";
        public const string NetOPC = "网关服务";
        public const string Tighten = "Tighten";
        public const string Other = "其它";

        public static Dictionary<int, string> deviceTypeDic = new Dictionary<int, string>
        {
            {1, Scanner},
            {2, NetOPC},
            {3, Tighten},
            {4, IO},
            {5, Other},
            {6, CardReader},
            {7, Light},
            {8, RFID}
        };

        public static Dictionary<int, string> DeviceTypeDic { get => deviceTypeDic; set => deviceTypeDic = value; }
    }

}
