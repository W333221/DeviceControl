using DeviceControl.Core.Command.Modbus;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data.Configs.DeviceConfig;
using DeviceControl.Data.Enum;
using DeviceControl.Data.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService.ProximitySwitchModule
{
    public class ProximitySwitchModuleDevice : IProximitySwitchModuleDevice
    {
        private List<ProximitySwitchModuleConfig> _proximitySwitchModuleConfigs;
        private INotificationService _notificationService;
        public Func<List<ProximitySwitchModuleModel>, Task> OnProximitySwitchChanged { get; set; }
        public ProximitySwitchModuleDevice(List<ProximitySwitchModuleConfig> proximitySwitchModuleConfigs, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _proximitySwitchModuleConfigs = proximitySwitchModuleConfigs;
            if (_proximitySwitchModuleConfigs == null)
            {
                _proximitySwitchModuleConfigs = new List<ProximitySwitchModuleConfig>();
            }
        }
        public void Close()
        {
            foreach (var item in _cancellationTokenSources)
            {
                try
                {
                    item.Cancel();
                }
                catch (Exception)
                {

                }
            }
        }
        private List<Task> _tasks = new List<Task>();
        private List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();



        public bool Init()
        {
            try
            {
                foreach (var item in _proximitySwitchModuleConfigs)
                {
                    _notificationService.CommonNotice(NotificationLevel.Normal, $"接近开关模块：{item.DeviceId},IP：{item.Ip}，端口：{item.Port}，{(item.Available ? "启用" : "停用")}");
                    if (!item.Available)
                    {
                        continue;
                    }
                    var cts = new CancellationTokenSource();
                    Task task = Task.Factory.StartNew(async config =>
                    {
                        var _config = (ProximitySwitchModuleConfig)config;
                        try
                        {

                            IModbusHelper modbusHelper = new TouchSocketModbusHelper(_config.Ip, _config.Port);
                            await modbusHelper.ConnectAsync();
                            _notificationService.CommonNotice(NotificationLevel.Normal, $"接近开关模块：{_config.DeviceId},IP：{_config.Ip}，端口：{_config.Port}，连接成功");
                            Dictionary<int, bool> dic = new Dictionary<int, bool>();
                            while (!cts.IsCancellationRequested)
                            {
                                try
                                {

                                    var result = await modbusHelper.ReadDiscreteInputsAsync(1, 0, 60);
                                    if (result != null)
                                    {
                                        List<ProximitySwitchModuleModel> models = new List<ProximitySwitchModuleModel>();
                                        for (int i = 0; i < result.Length; i++)
                                        {
                                            bool value = result[i];

                                            // 检查是否需要处理
                                            if (dic.TryGetValue(i, out bool cached) && cached == value) continue;

                                            // 更新缓存
                                            dic[i] = value;

                                            // 创建并添加模型
                                            models.Add(new ProximitySwitchModuleModel
                                            {
                                                DeviceId = _config.DeviceId,
                                                Address = i,
                                                Value = value
                                            });
                                        }
                                        if (models.Count > 0)
                                        {
                                            if (OnProximitySwitchChanged != null)
                                            {
                                                OnProximitySwitchChanged.Invoke(models);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                finally
                                {
                                    await Task.Delay(_config.DelayTime);
                                }


                            }
                        }
                        catch (Exception ex)
                        {
                            _notificationService.CommonNotice(NotificationLevel.Normal, $"接近开关模块：{_config.DeviceId},IP：{_config.Ip}，端口：{_config.Port}，连接失败,{ex.Message}");
                        }
                    }, item, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    _tasks.Add(task);
                    _cancellationTokenSources.Add(cts);

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task InitAsync()
        {
            try
            {
                await Task.Run(() => Init());
            }
            catch (Exception ex)
            {
                Log.Error("接近开关模块设备初始化失败" + ex.ToString(), ex);
            }
        }
    }

}
