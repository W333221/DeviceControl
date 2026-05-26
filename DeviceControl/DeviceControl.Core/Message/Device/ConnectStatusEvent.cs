using DeviceControl.Core.ServiceContract.DeviceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Message.Device
{
    public class ConnectStatusEvent : PubSubEvent<ConnectStatusArg>
    {
    }

    public class ConnectStatusArg
    {
        public ConnectStatusArg()
        {

        }

        public ConnectStatusArg(IDevice deviceInfo, bool connected)
        {
            DeviceInfo = deviceInfo;
            Connected = connected;
        }

        public IDevice DeviceInfo { get; set; }

        public bool Connected { get; set; }
    }
}
