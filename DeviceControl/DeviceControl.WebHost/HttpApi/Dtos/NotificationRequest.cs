using DeviceControl.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.WebHost.HttpApi.Dtos
{
    public class NotificationRequest
    {
        public NotificationLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}