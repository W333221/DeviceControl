using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Data.Configs.DeviceConfig;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService.ProximitySwitchModule
{
    public class ProximitySwitchModuleFactory
    {
        IConfiguration _config;
        INotificationService _notificationService;

        public ProximitySwitchModuleFactory(IConfiguration config, INotificationService notificationService)
        {
            _config = config;
            _notificationService = notificationService;
        }

        public IProximitySwitchModuleDevice GetInStance()
        {
            _notificationService.CommonNotice(Data.Enum.NotificationLevel.Normal, "进入接近开关模块设备加载");
            var proximitySwitchModuleConfigs = _config.GetSection("AppSettings:DeviceConfig:ProximitySwitchModuleConfig").Get<List<ProximitySwitchModuleConfig>>();
            if (proximitySwitchModuleConfigs == null || proximitySwitchModuleConfigs.Count == 0)
            {
                Log.Information("接近开关模块配置错误，请检查配置文件");
                return null;
            }
            Log.Information($"接近开关模块设备加载方法");
            return new ProximitySwitchModuleDevice(proximitySwitchModuleConfigs, _notificationService);
        }
    }
}
