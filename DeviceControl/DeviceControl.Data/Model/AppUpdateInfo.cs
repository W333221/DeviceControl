using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Model
{
    public class AppUpdateInfo
    {
        public string Version { get; set; }
        public int UpdateType { get; set; }
        public DateTime ReleaseTime { get; set; }
        public string ReleaseNotes { get; set; }
    }
}
