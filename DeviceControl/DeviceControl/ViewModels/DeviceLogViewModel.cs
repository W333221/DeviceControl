using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
using DeviceControl.Data.Configs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using DeviceControl.Core.Message.Device;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Core.Message.UI;
using Microsoft.Extensions.Configuration;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Data.Model;
using System.Windows;

namespace DeviceControl.ViewModels
{
    public class DeviceLogViewModel : BindableBase
    {
        private IConfiguration _configuration;
        private IEventAggregator _eventAggregator;

        private readonly ILogViewerService _logViewerService;
        public ObservableCollection<LogItem> Logs { get; } = new();

        public DeviceLogViewModel( IConfiguration configuration, IEventAggregator eventAggregator, ILogViewerService logViewerService)
        {
            _configuration = configuration;
            _eventAggregator = eventAggregator;
            _logViewerService = logViewerService;

            InitData();
            InitSubscribe();


            // 订阅新增日志
            _logViewerService.LogAdded += OnLogAdded;
        }


        #region 属性

   

        #endregion

        #region 事件
        public DelegateCommand ClearCommand => new DelegateCommand(ClearLogs);
        private void ClearLogs()
        {
            Logs.Clear();
            _logViewerService.Clear();
        }
        #endregion

        #region 订阅消息

        private void InitSubscribe()
        {
          

        }

        #endregion

        #region  帮助方法

        private void InitData() {
         
           // 初始化已有日志
            foreach (var item in _logViewerService.GetLogs())
            {
                Logs.Add(item);
            }

        }

        private void OnLogAdded(LogItem item)
        {
            // Serilog 线程 != UI线程，所以必须切到 Dispatcher
            if (Application.Current == null) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Add(item);

                while (Logs.Count > 100)
                {
                    Logs.RemoveAt(0);
                }
            });
        }
        #endregion
    }
}
