using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Enum
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum NotificationType
    {
        [Description("准备通知")]
        ReadyNotice = 1,
        [Description("成功通知")]
        SuccseNotice = 2,
        [Description("错误通知")]
        ErrNotice = 3,
        [Description("一般通知")]
        CommonNotice = 4,
        [Description("按钮通知")]
        LogButton = 5,
        [Description("API通知")]
        LogAPI = 6,
    }
}
