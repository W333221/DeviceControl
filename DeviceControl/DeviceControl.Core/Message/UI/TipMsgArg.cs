using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Message.UI
{
    public class TipMsgEvent : PubSubEvent<TipMsgArg>
    {
    }
    public class TipMsgArg
    {
        public string TipMsgColor { get; set; } = string.Empty;
        public string TipMsg { get; set; } = string.Empty;
        public TipMsgArg(string msg, string color)
        {
            TipMsg = msg;
            TipMsgColor = color;
        }
    }
}
