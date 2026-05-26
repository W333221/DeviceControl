using DeviceControl.Data.Configs;
using DeviceControl.Views;
using DeviceControl.Views.Config;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly IRegionManager _regionManager;
        private IConfiguration _configuration;

        public bool IsRealClose { get; private set; } = false;

        public MainWindow(IRegionManager regionManager, IConfiguration configuration)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _configuration = configuration;


        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate("ControlMain", nameof(Main));

            if (!_configuration.GetSection("AppSettings:IsDebug").Get<bool>())
            {
                //调试模式不隐藏，
                await Task.Delay(100);
                this.Hide();
                this.ShowInTaskbar = false;
            }

        }
        public void AllowRealClose()
        {
            IsRealClose = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //// 取消关闭事件
            //e.Cancel = true;
            //// 隐藏主窗口，让它“消失”到托盘
            //this.Hide();
            if (!IsRealClose)
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
                return;
            }

            // 释放托盘图标
            if (MyNotifyIcon != null)
            {
                MyNotifyIcon.Dispose();
            }

            base.OnClosing(e);
        }
    }
}