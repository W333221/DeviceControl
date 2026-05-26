using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Model
{
    public class ProximitySwitchModuleModel
    {
        public string DeviceId { get; set; }
        public int Address { get; set; }
        public bool Value { get; set; }
    }
}
