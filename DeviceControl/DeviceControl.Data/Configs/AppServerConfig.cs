using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs
{
    public class AppServerConfig
    {
        public int ServerPort { get; set; }
        public string [] Origins { get; set; }
    }
}
