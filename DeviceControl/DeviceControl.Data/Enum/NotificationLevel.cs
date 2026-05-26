using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Enum
{
    /// <summary>
    /// 通知等级
    /// </summary>
    public enum NotificationLevel
    {
        [Description("一般")]
        Normal = 1,
        [Description("紧急")]
        Urgent = 2,
        [Description("危险")]
        Perilous = 3,
    }
}
