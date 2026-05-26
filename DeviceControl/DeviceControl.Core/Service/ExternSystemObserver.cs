using DeviceControl.Core.ServiceContract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service
{
    /// <summary>
    /// 监听服务
    /// </summary>
    public class ExternSystemObserver : ExternSystemObserverAbstract
    {
        public ExternSystemObserver(IContainerProvider containerProvider, Lazy<INotificationService> notificationService, IConfiguration configuration, IEventAggregator eventAggregator) : base(containerProvider, notificationService, configuration, eventAggregator)
        {
        }
    }
}