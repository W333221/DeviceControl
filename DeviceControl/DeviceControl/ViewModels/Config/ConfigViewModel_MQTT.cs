using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DeviceControl.ViewModels.Config
{
    public partial class ConfigViewModel : BindableBase
    {
        public ObservableCollection<MQTTConfig> MQTTConfigs { get; }
   = new ObservableCollection<MQTTConfig>();
        private void Add()
        {
            MQTTConfigs.Add(new MQTTConfig());
        }

        private void ReadMQTTConfig()
        {
            try
            {
                MQTTConfigs.Clear();
                var configs = _configuration.GetSection("MQTTConfig").Get<List<MQTTConfig>>();
                MQTTConfigs.AddRange(configs);
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveMQTT()
        {
            try
            {
                if (!CheckUpdateConfigAuth())
                {
                    return;
                }
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var jsonContent = File.ReadAllText(configPath);
                var jsonConfig = JObject.Parse(jsonContent);
                JToken updatejson = jsonConfig.SelectToken("$.MQTTConfig");

                JArray jArray = new JArray(MQTTConfigs.Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.MqttHost)).Select(c => JObject.FromObject(c)));
                updatejson.Replace(jArray);
                File.WriteAllText(configPath, jsonConfig.ToString());
                var configRoot = (IConfigurationRoot)_configuration;
                configRoot.Reload();
                ReadMQTTConfig();
                Growl.Success(new GrowlInfo() { Message = "保存成功", WaitTime = 2 });
                UpdateComboBoxItemsSource();
            }
            catch (Exception)
            {
                Growl.Error(new GrowlInfo() { Message = "保存失败", WaitTime = 2 });
            }
        }

        public DelegateCommand AddCommand => new(Add);

        public DelegateCommand SaveMQTTCommand => new(SaveMQTT);

        private bool _IsOpenDrawer = false;
        public bool IsOpenDrawer
        {
            get { return _IsOpenDrawer; }
            set { SetProperty(ref _IsOpenDrawer, value); }
        }

    }

    public class MQTTConfig : BindableBase
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        private string _MqttHost;
        public string MqttHost
        {
            get { return _MqttHost; }
            set { SetProperty(ref _MqttHost, value); }
        }

        private int _MqttPort;
        public int MqttPort
        {
            get { return _MqttPort; }
            set { SetProperty(ref _MqttPort, value); }
        }

        private string _MqttUserName;
        public string MqttUserName
        {
            get { return _MqttUserName; }
            set { SetProperty(ref _MqttUserName, value); }
        }

        private string _MqttPwd;
        public string MqttPwd
        {
            get { return _MqttPwd; }
            set { SetProperty(ref _MqttPwd, value); }
        }
    }
}
