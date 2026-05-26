using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data.Model;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service
{
    public abstract partial class ExternSystemObserverAbstract : IExternSystemObserver
    {
        protected readonly IContainerProvider _containerProvider;
        protected Lazy<INotificationService> _notificationService;
        protected IConfiguration _configuration;
        protected IEventAggregator _eventAggregator;
        protected INotificationService NotificationService => _notificationService.Value;
        public List<IDevice> Devices { get; set; }
        public ExternSystemObserverAbstract(
           IContainerProvider containerProvider,
           Lazy<INotificationService> notificationService,
           IConfiguration configuration,
           IEventAggregator eventAggregator
       )
        {
            _containerProvider = containerProvider;
            _configuration = configuration;
            _notificationService = notificationService;
            _eventAggregator = eventAggregator;
        }


        private Task Device_OnProximitySwitchChanged(List<ProximitySwitchModuleModel> list)
        {
            if (list != null)
            {
                foreach (ProximitySwitchModuleModel model in list)
                {
                  Log.Information("{DeviceId} {Address} {Value}", model.DeviceId, model.Address, model.Value);
                }
            }
            return Task.CompletedTask;

        }

    }
}
