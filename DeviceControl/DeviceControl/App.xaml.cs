using Microsoft.Extensions.Configuration;
using Serilog;
using System.Configuration;
using System.Data;
using Prism.Container.DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using DeviceControl.Views;
using DeviceControl.ViewModels;
using DeviceControl.Core.Service.DeviceService.ColorLamp;
using DeviceControl.Core.ServiceContract.DeviceService;
using DeviceControl.Views.Config;
using DeviceControl.WebHost.Abstractions;
using DeviceControl.WebHost;
using DeviceControl.Core.Service;
using DeviceControl.Core.ServiceContract;
using DeviceControl.Core.Command;
using DeviceControl.Data.Enum;
using DeviceControl.Service;
using DeviceControl.Data;
using Newtonsoft.Json;
using System.Reflection;
using DeviceControl.Data.Configs;
using DeviceControl.Data.Model;
using Serilog.Core;
using DeviceControl.Core.Service.DeviceService.ProximitySwitchModule;

namespace DeviceControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private IConfiguration _configuration;
        private IWebHostBootstrapper? _webHostBootstrapper;


        public App()
        {
            InitializeComponent();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // 构建配置
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Process proceMain = Process.GetCurrentProcess();
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(proceMain.ProcessName);
                string appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                //Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                if (processes.Length > 1)
                {
                    //MessageBox.Show("其他用户正在使用或应用程序正在运行中...");
                    //Thread.Sleep(500);
                    //Environment.Exit(1);
                    var args = Environment.GetCommandLineArgs();
                    bool isRestart = args.Contains("--restart");
                    if (isRestart)
                    {
                        foreach (Process process in processes)//获取所有同名进程id
                        {
                            if (process.Id != proceMain.Id)//根据进程id删除所有除本进程外的所有相同进程
                                process.Kill();
                        }
                    }
                    else
                    {
                        System.Windows.MessageBoxResult dialogResult = HandyControl.Controls.MessageBox.Show($"已运行{processes.Length - 1}个重复的程序,确认是否关闭其他程序？", "提示", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                        if (dialogResult == System.Windows.MessageBoxResult.Yes)
                        {
                            foreach (Process process in processes)//获取所有同名进程id
                            {
                                if (process.Id != proceMain.Id)//根据进程id删除所有除本进程外的所有相同进程
                                    process.Kill();
                            }
                        }
                        else//没有关闭其他的进程，不允许打开新的程序
                        {
                            proceMain.Kill();
                            return;
                        }
                    }

                }
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message.ToString() + ",堆栈:" + ex.StackTrace);
            }

        }
        protected override async void OnInitialized()
        {
            base.OnInitialized();
            _webHostBootstrapper = Container.Resolve<IWebHostBootstrapper>();
            await _webHostBootstrapper.StartAsync();
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            if (_webHostBootstrapper != null)
            {
                await _webHostBootstrapper.StopAsync();
            }
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            // 先初始化日志
            var logViewerService = Container.Resolve<ILogViewerService>();
            InitializeLogger(logViewerService);

            InitializeStartup();
            return Container.Resolve<MainWindow>();
        }
        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }


        #region 初始化开机启动

        private void InitializeStartup()
        {
            bool isStartUp = _configuration.GetSection("AppSettings:IsStartUp").Get<bool>();
            string appName = "DeviceControl";
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (isStartUp)
            {
                StartupManager.AddToStartup(appName, exePath, "设备控制");
            }
            else
            {
                if (StartupManager.IsInStartup(appName))
                {
                    StartupManager.RemoveFromStartup(appName);
                    Log.Information("已关闭开机自启动");
                }
            }
            bool isAutoStart = StartupManager.IsInStartup(appName);
            Log.Information($"自启动状态：{(isAutoStart ? "已启用" : "未启用")}");

        }

        #endregion        

        #region 初始化日志

        /// <summary>
        /// 初始化日志
        /// </summary>
        private void InitializeLogger(ILogViewerService logViewerService)
        {
            string output = "{Timestamp:HH:mm:ss.fff}: [{Level:u3}]{Message:lj}{NewLine}{Exception}";
            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                 //.Enrich.WithProperty("Application", "Test123") // 为每个项目添加一个唯一标识属性
                 //.WriteTo.Seq("http://localhost:5341")       // 发送到本地 Seq 服务器
                 .ReadFrom.Configuration(_configuration)

                .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                // UI实时日志
                .WriteTo.Sink(new UiLogSink(logViewerService))

                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(f => f.Level == Serilog.Events.LogEventLevel.Debug)
                   .WriteTo.Async(a => a.File("Log\\Debug\\Debug.txt", rollingInterval: RollingInterval.Day, outputTemplate: output, retainedFileCountLimit: 120)))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(f => f.Level == Serilog.Events.LogEventLevel.Information)
                    .WriteTo.Async(a => a.File("Log\\info\\Info.txt", rollingInterval: RollingInterval.Day, outputTemplate: output, retainedFileCountLimit: 120)))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(f => f.Level >= Serilog.Events.LogEventLevel.Error)
                    .WriteTo.Async(a => a.File("Log\\Error\\Error.txt", rollingInterval: RollingInterval.Day, outputTemplate: output, retainedFileCountLimit: 120)))
                 .WriteTo.Logger(l => l.Filter.ByIncludingOnly(f => f.Level == Serilog.Events.LogEventLevel.Warning)
                    .WriteTo.Async(a => a.File("Log\\Warning\\Warning.txt", rollingInterval: RollingInterval.Day, outputTemplate: output, retainedFileCountLimit: 120)))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(f => f.Level == Serilog.Events.LogEventLevel.Verbose)
                   .WriteTo.Async(a => a.File("Log\\API\\ApiLog.txt", rollingInterval: RollingInterval.Day, outputTemplate: output, retainedFileCountLimit: 120)))
                .CreateLogger();
            Log.Logger = log;
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            Log.Information("\r\n");
            Log.Information("***************系统启动*****************");

        }

        #endregion

        #region 服务注册
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (containerRegistry is IContainerProvider containerProvider)
                Data.IOC.Instance.Init(containerProvider);
            RegisterServices(containerRegistry);
            RegisterViewModels(containerRegistry);
            RegisterViews(containerRegistry);
            CreateTable(containerRegistry);
        }
        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IConfiguration>(_configuration);
            containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            containerRegistry.RegisterSingleton<IExternSystemObserver, ExternSystemObserver>();

            #region 设备
            //色灯
            containerRegistry.RegisterInstance<IColorLampDevice>(new ColorLampFactory(_configuration).GetInStance());

            containerRegistry.RegisterInstance<IProximitySwitchModuleDevice>(new ProximitySwitchModuleFactory(_configuration, Container.Resolve<INotificationService>()).GetInStance());
            #endregion

            containerRegistry.RegisterSingleton<IWebHostBootstrapper, WebHostBootstrapper>();
            containerRegistry.RegisterSingleton<ILogViewerService, LogViewerService>();

            containerRegistry.Register<IShowDialogService, HandyDialogService>();

        }
        private void RegisterViewModels(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainViewModel>();

        }

        private void RegisterViews(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow, MainViewModel>();

            containerRegistry.RegisterForNavigation<Main>();
            containerRegistry.RegisterForNavigation<Config>();
            containerRegistry.RegisterForNavigation<DeviceLog>();
            containerRegistry.RegisterForNavigation<FileLogQuery>();
            containerRegistry.RegisterForNavigation<ModbusPage>();

        }

        private void CreateTable(IContainerRegistry containerRegistry)
        {
            try
            {
                //ISqlSugarClient db = containerRegistry.GetContainer().Resolve<ISqlSugarClient>();
                //db.DbMaintenance.CreateDatabase();
                //Type[] types = new Type[] { typeof(LogInfo), typeof(QueueJob) };
                //foreach (var item in types)
                //{
                //    if (!db.DbMaintenance.IsAnyTable(item.Name))
                //    {
                //        db.CodeFirst.InitTables(item);
                //    }
                //    else
                //    {
                //        var diff = db.CodeFirst.GetDifferenceTables(item).ToDiffList();
                //        if (diff.Count > 0)
                //        {
                //            db.CodeFirst.InitTables(item);
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region 异常处理
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                Log.Error("全局UI异常捕获" + e.Exception.Message.ToString() + ",堆栈:" + e.Exception?.StackTrace?.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("App_DispatcherUnhandledException异常" + ex.Message + ",堆栈:" + ex.StackTrace);
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                Log.Error("全局应用程序异常" + exception?.Message.ToString() + ",堆栈:" + exception?.StackTrace?.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("CurrentDomain_UnhandledException异常" + ex.Message + ",堆栈:" + ex.StackTrace);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                foreach (var exception in e.Exception?.InnerExceptions)
                {
                    Log.Error("全局应用程序异常" + exception?.Message);
                }
                Log.Error("全局应用程序异常" + ",堆栈:" + e.Exception.StackTrace?.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("TaskScheduler_UnobservedTaskException异常" + ex.Message + ",堆栈:" + ex.StackTrace);
            }

        }

        #endregion
    }

}
