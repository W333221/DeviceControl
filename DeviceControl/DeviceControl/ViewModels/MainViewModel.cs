using DeviceControl.Core.Message.Device;
using DeviceControl.Core.Message.UI;
using DeviceControl.Core.Service;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Data;
using DeviceControl.Data.Configs;
using DeviceControl.Data.Configs.DeviceConfig;
using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using DeviceControl.Data.Enum;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceControl.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private IConfiguration _configuration;
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private readonly IContainerProvider _containerProvider;
        protected Lazy<INotificationService> _notificationService;
        private Dictionary<string, bool> DeviceStatusDic = new Dictionary<string, bool>();

        public MainViewModel(IRegionManager regionManager, IConfiguration configuration, IEventAggregator eventAggregator, IContainerProvider containerProvider, Lazy<INotificationService> notificationService)
        {
            _regionManager = regionManager;
            _configuration = configuration;
            _eventAggregator = eventAggregator;
            _containerProvider = containerProvider;
            _notificationService = notificationService;

            InitData();
            InitSubscribe();
            Task.Run(async () =>
            {
                await InitDevice();
            });
        }

        #region 属性

        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        private string _StationNo;
        public string StationNo
        {
            get { return _StationNo; }
            set { SetProperty(ref _StationNo, value); }
        }

        private string _StationName;
        public string StationName
        {
            get { return _StationName; }
            set { SetProperty(ref _StationName, value); }
        }
        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }



        private string _TipMsgColor = string.Empty;
        public string TipMsgColor
        {
            get { return _TipMsgColor; }
            set { SetProperty(ref _TipMsgColor, value); }
        }
        private string _TipMsg = string.Empty;
        public string TipMsg
        {
            get { return _TipMsg; }
            set
            {
                SetProperty(ref _TipMsg, value);
                Task.Run(async () =>
                {
                    TipIsOpen = string.IsNullOrEmpty(TipMsg)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                    if (TipIsOpen == Visibility.Visible)
                    {
                        await Task.Delay(3000);
                        TipIsOpen = Visibility.Collapsed;
                    }
                });
            }
        }
        private Visibility _TipIsOpen = Visibility.Collapsed;

        public Visibility TipIsOpen
        {
            get { return _TipIsOpen; }
            set
            {
                SetProperty(ref _TipIsOpen, value);
            }
        }

        private string _ServiceStatus = "OK";

        // <summary>
        /// 服务器状态
        /// </summary>
        public string ServiceStatus
        {
            get { return _ServiceStatus; }
            set { SetProperty(ref _ServiceStatus, value); }
        }

        private bool _AllDeviceStatus;
        public bool AllDeviceStatus
        {
            get => _AllDeviceStatus;
            set => SetProperty(ref _AllDeviceStatus, value);
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private ObservableCollection<DeviceConnectStatus> _DeviceConnectStatuses = new ObservableCollection<DeviceConnectStatus>();
        public ObservableCollection<DeviceConnectStatus> DeviceConnectStatuses
        {
            get => _DeviceConnectStatuses;
            set => SetProperty(ref _DeviceConnectStatuses, value);
        }

        private bool isShowDeviceStatus;
        public bool IsShowDeviceStatus { get => isShowDeviceStatus; set => SetProperty(ref isShowDeviceStatus, value); }

        #endregion

        #region 事件
        public DelegateCommand<string> PageNavCommand =>
            new DelegateCommand<string>((page) => NavigateTo(page));
        private void NavigateTo(string page)
        {
            _regionManager.RequestNavigate("ContentRegion", page);
        }

        public DelegateCommand TipMsgVisibilityCommand => new(Execute_TipMsgVisibility);
        private void Execute_TipMsgVisibility()
        {
            TipIsOpen = TipIsOpen == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 关闭软件
        /// </summary>
        public AsyncDelegateCommand CloseCommand => new(Close);
        private async Task Close()
        {
            try
            {

                IsLoading = true;
                await _notificationService.Value.CommonNoticeAsync(NotificationLevel.Perilous, "关闭软件");
                IExternSystemObserver externSystemObserver = _containerProvider.Resolve<IExternSystemObserver>();
                await externSystemObserver.StopColorLampAsync();

                // 获取主窗口
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // 允许真正退出
                    mainWindow.AllowRealClose();

                    // 关闭窗口
                    mainWindow.Close();
                }

                // 保险起见，确保应用退出
                Application.Current.Shutdown();
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "关闭软件失败");
                IsLoading = false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public AsyncDelegateCommand SimulatedTighteningCommand => new (Execute_SimulatedTightening);
        private async Task Execute_SimulatedTightening()
        {
           await _notificationService.Value.CommonNoticeAsync(NotificationLevel.Perilous, "报警复位");
        }

        public DelegateCommand DeviceStatusCommand => new DelegateCommand(DeviceStatus);
        private void DeviceStatus()
        {
            IsShowDeviceStatus = !IsShowDeviceStatus;
        }
        #endregion

        #region 订阅消息

        private void InitSubscribe()
        {
            _eventAggregator.GetEvent<TipMsgEvent>().Subscribe(Msg);//订阅消息通知
            _eventAggregator.GetEvent<ConnectStatusEvent>().Subscribe(OnConnectChanged, ThreadOption.UIThread); //设备状态消息订阅

        }
        private void Msg(TipMsgArg arg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TipMsg = arg.TipMsg;
                TipMsgColor = arg.TipMsgColor;
            });
        }


        #endregion

        #region  帮助方法
        private void InitData()
        {
            var station = _configuration.GetSection("AppSettings:Station").Get<StationConfig>();
            _StationName = station.StationName;
            _StationNo = station.StationID;
            _Version = "1.0.0.1";
            _Title = station.Title;
            TipMsg = $"{station.Title}启动成功！";

            var deviceConfig = _configuration.GetSection("AppSettings:DeviceConfig").Get<AllDeviceConfig>();
            Dictionary<string, DeviceConfigBase> deviceConfigDic = new Dictionary<string, DeviceConfigBase>();
            deviceConfigDic.Add("IColorLampDevice", deviceConfig.ColorLampConfig);
            //deviceConfigDic.Add("IRFIDDevice", deviceConfig.RfidConfig);
            //deviceConfigDic.Add("IScanDevice", deviceConfig.ScanConfig);
            //deviceConfigDic.Add("IOModuleDevice", deviceConfig.IOModelConfig);
            //deviceConfigDic.Add("ISwipeCardDevice", deviceConfig.SwipeCardConfig);
            foreach (var item in deviceConfigDic)
            {
                if (item.Value != null && item.Value.Available)
                {
                    bool deviceStatus = false;
                    if (item.Key != "ISwipeCardDevice")
                    {
                        DeviceStatusDic.Add(item.Key, deviceStatus);
                    }
                    DeviceConnectStatuses.Add(new DeviceConnectStatus() { Key = item.Key, Name = item.Value.FriendlyName, Status = deviceStatus });
                }
            }

            //DeviceConnectStatuses.Add(new DeviceConnectStatus() { Key = "IOpcService", Name = "OPC网关", Status = false });
            //DeviceStatusDic.Add("IOpcService", false);
        }

        private async Task InitDevice()
        {
            IExternSystemObserver externSystemObserver = _containerProvider.Resolve<IExternSystemObserver>();
            await externSystemObserver.StartColorLampAsync();
            await externSystemObserver.StartProximitySwitchModuleAsync();

        }


        private void OnConnectChanged(ConnectStatusArg deviceConnect)
        {
            try
            {

                Log.Information(String.Format("{0} ConnectStatus={1}！", deviceConnect.DeviceInfo.FriendlyName, deviceConnect.Connected ? "连接成功" : "连接断开"));
                switch (deviceConnect.DeviceInfo)
                {
                    //case IOpcService:
                    //    OnConnectChanged("IOpcService", deviceConnect.Connected);
                    //    break;
                    //case IRFIDDevice:
                    //    OnConnectChanged("IRFIDDevice", deviceConnect.Connected);
                    //    break;
                    //case ISwipeCardDevice:
                    //    OnConnectChanged("ISwipeCardDevice", deviceConnect.Connected);
                    //    break;
                    //case IScanDevice:
                    //    OnConnectChanged("IScanDevice", deviceConnect.Connected);
                    //    break;
                    //case IOModuleDevice:
                    //    OnConnectChanged("IOModuleDevice", deviceConnect.Connected);
                    //    break;
                    case IColorLampDevice:
                        OnConnectChanged("IColorLampDevice", deviceConnect.Connected);
                        break;
                    default:
                        break;
                }
                UpdateAllDeviceStatus();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ",StackTrace:" + ex.StackTrace, ex);
            }
        }

        private void OnConnectChanged(string key, bool value)
        {
            try
            {
                if (DeviceStatusDic.ContainsKey(key))
                {
                    DeviceStatusDic[key] = value;
                }
                int index = DeviceConnectStatuses.ToList().FindIndex(x => x.Key == key);
                if (index != -1)
                {
                    DeviceConnectStatuses[index].Status = value;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ",StackTrace:" + ex.StackTrace, ex);
            }
        }

        private void UpdateAllDeviceStatus()
        {
            if (DeviceStatusDic.Select(x => x.Value).Any(x => !x))
            {
                AllDeviceStatus = false;
            }
            else
            {
                if (ServiceStatus == "OK")
                {
                    AllDeviceStatus = true;
                }
                else
                {
                    AllDeviceStatus = false;
                }
            }
        }

        #endregion
    }



    public class DeviceConnectStatus : BindableBase
    {
        private string _Key;
        public string Key
        {
            get => _Key;
            set => SetProperty(ref _Key, value);
        }
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }
        private bool _Status;
        public bool Status
        {
            get => _Status;
            set => SetProperty(ref _Status, value);
        }


        public AsyncDelegateCommand ReconnectCommand => new(Execute_Reconnect);
        private async Task Execute_Reconnect()
        {
            IExternSystemObserver externSystemObserver = _containerProvider.Resolve<IExternSystemObserver>();
            if (Key == "IColorLampDevice") {
                await externSystemObserver.StopColorLampAsync();
                await externSystemObserver.StartColorLampAsync();
            }
        }

        private IContainerProvider _containerProvider;
        public DeviceConnectStatus()
        {
            _containerProvider = IOC.Instance.ServiceProvider.Resolve<IContainerProvider>();

        }
    }
}
