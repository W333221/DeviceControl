using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Message
{
    public class EventAggregator
    {
        public static Prism.Events.IEventAggregator Ea = new Prism.Events.EventAggregator();
    }
}
