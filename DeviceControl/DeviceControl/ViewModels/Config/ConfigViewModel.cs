using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Newtonsoft.Json.Linq;
using DeviceControl.Data.Configs;
using Newtonsoft.Json;

namespace DeviceControl.ViewModels.Config
{
    public partial class ConfigViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        IContainerProvider _containerProvider;
        IConfiguration _configuration;
        private bool IsZh_CH = true;
        private bool isLoad = true;

        public ConfigViewModel(IContainerProvider containerProvider, IConfiguration configuration, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _containerProvider = containerProvider;
            _configuration = configuration;
            ReadStationConfig();
            ReadColorLampConfig();          
            ReadAutoUpdate();
            ReadMQTTConfig();
        }
        private void ChangeLanguage()
        {
            ChangeLanguage(AutoUpdate_FormFields);
            ChangeLanguage(ColorLampConfig_FormFields);
            ChangeLanguage(Station_FormFields);
            IsZh_CH = !IsZh_CH;
        }

        private void ChangeLanguage(ObservableCollection<FormFieldMetadata> formFieldMetadata)
        {
            foreach (var item in formFieldMetadata)
            {
                item.Label = IsZh_CH ? item.EN_Label : item.Zh_CN_Label;
            }
        }

        private List<string> GetDerivedClassesNames<T>()
        {
            var implementTypes = AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(t =>
                    t != null
                    && t.IsClass
                    && !t.IsAbstract
                    && typeof(T).IsAssignableFrom(t)
                )
                .Select(x => x.Name)
                .ToList();
            return implementTypes;
        }

        private bool CheckUpdateConfigAuth()
        {

            return true;
            //var config = _containerProvider.Resolve<IConfiguration>();
            //SwipeCardWindow swipeCardWindow = new();
            //if (swipeCardWindow.DataContext is SwipeCardWindowViewModel vm)
            //{
            //    vm.FunctionKey = config.GetSection("AppSettings:ClientPermission:FunctionKey:UpdateConfig").Get<string>();
            //}
            //bool? result = swipeCardWindow.ShowDialog();
            //if (result.HasValue && result.Value)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        private void ShowGrowlInfo(bool value)
        {
            if (value)
            {
                Growl.Success(new GrowlInfo() { Message = "保存成功", WaitTime = 2 });
            }
            else
            {
                Growl.Error(new GrowlInfo() { Message = "保存失败", WaitTime = 2 });
            }
        }

        private async Task SaveDeviceConfig()
        {
            if (!CheckUpdateConfigAuth())
            {
                return;
            }
            List<bool> reslut = new List<bool>() {
                await SaveColorLampConfig(),             
            };
            ShowGrowlInfo(reslut.All(x => true));
        }

        private void UpdateComboBoxItemsSource()
        {            
            UpdateComboBoxItemsSource_AutoUpdate();
        }

        private void UpdateComboBoxItemsSource(ObservableCollection<FormFieldMetadata> formFieldMetadata, string[] arr)
        {
            try
            {
                var base_configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var base_jsonContent = File.ReadAllText(base_configPath);
                var base_jsonConfig = JObject.Parse(base_jsonContent);
                foreach (var item in arr)
                {
                    try
                    {
                        int index = formFieldMetadata.ToList().FindIndex(x => x.BindingPath == item);
                        if (formFieldMetadata[index].FieldType == FieldType.ComboBox)
                        {
                            var jtokens = base_jsonConfig.SelectTokens(formFieldMetadata[index].ItemsSourceRoute);
                            var source = jtokens.Select(x => Convert.ToString(x)).ToList();
                            var value = formFieldMetadata[index].CurrentValue;
                            formFieldMetadata[index].ItemsSource.Clear();
                            formFieldMetadata[index].ItemsSource.AddRange(source);
                            formFieldMetadata[index].CurrentValue = value;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ReadConfig<T>(ObservableCollection<FormFieldMetadata> formFieldMetadata, string key, string jsonPath)
        {
            try
            {
                var base_configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var base_jsonContent = File.ReadAllText(base_configPath);
                var base_jsonConfig = JObject.Parse(base_jsonContent);

                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.extend.json");
                var jsonContent = File.ReadAllText(configPath);
                var jsonConfig = JObject.Parse(jsonContent);

                formFieldMetadata.Clear();
                var config = _configuration.GetSection(key).Get<T>();
                if (config == null)
                {
                    return;
                }
                PropertyInfo[] propertyInfos = this.GetType().GetProperties();
                if (propertyInfos == null)
                {
                    return;
                }
                PropertyInfo[] _propertyInfos = config.GetType().GetProperties();
                List<FormFieldMetadata> formFields = new List<FormFieldMetadata>();
                foreach (var item in _propertyInfos)
                {
                    try
                    {
                        var extendJToken = jsonConfig.SelectToken($"{jsonPath}.{item.Name}");
                        if (extendJToken == null)
                        {
                            continue;
                        }
                        if (extendJToken.Type == JTokenType.Array)
                        {
                            continue;
                        }
                        var extendJObject = (JObject)extendJToken;
                        if (extendJObject["zh_cn"] == null || extendJObject["Sort"] == null || extendJObject["IsEnable"] == null)
                        {
                            continue;
                        }
                        ExtendConfig extendConfig = JsonConvert.DeserializeObject<ExtendConfig>(extendJObject.ToString());
                        if (!extendConfig.IsEnable)
                        {
                            continue;
                        }
                        var value = item.GetValue(config);

                        string zh_CN_Label = extendConfig.zh_cn;
                        string eN_Label = item.Name;
                        string desc = string.IsNullOrEmpty(extendConfig.Desc) ? zh_CN_Label : extendConfig.Desc;
                        switch (extendConfig.FieldType)
                        {
                            case FieldType.TextBox:
                                formFields.Add(new TextFieldMetadata()
                                {
                                    Label = IsZh_CH ? $"{zh_CN_Label}：" : $"{eN_Label}：",
                                    EN_Label = $"{eN_Label}：",
                                    Zh_CN_Label = $"{zh_CN_Label}：",
                                    BindingPath = eN_Label,
                                    Sort = extendConfig.Sort,
                                    Desc = desc,
                                    CurrentValue = value
                                });
                                break;
                            case FieldType.RadioGroup:
                                formFields.Add(new RadioGroupMetadata()
                                {
                                    Label = IsZh_CH ? $"{zh_CN_Label}：" : $"{eN_Label}：",
                                    EN_Label = $"{eN_Label}：",
                                    Zh_CN_Label = $"{zh_CN_Label}：",
                                    BindingPath = eN_Label,
                                    Sort = extendConfig.Sort,
                                    Desc = desc,
                                    CurrentValue = value
                                });
                                break;
                            case FieldType.NumericUpDown:
                                formFields.Add(new NumericFieldMetadata()
                                {
                                    Label = IsZh_CH ? $"{zh_CN_Label}：" : $"{eN_Label}：",
                                    EN_Label = $"{eN_Label}：",
                                    Zh_CN_Label = $"{zh_CN_Label}：",
                                    BindingPath = eN_Label,
                                    Sort = extendConfig.Sort,
                                    Desc = desc,
                                    CurrentValue = Convert.ToInt32(value)
                                });
                                break;
                            case FieldType.PasswordBox:
                                formFields.Add(new PasswordBoxMetadata()
                                {
                                    Label = IsZh_CH ? $"{zh_CN_Label}：" : $"{eN_Label}：",
                                    EN_Label = $"{eN_Label}：",
                                    Zh_CN_Label = $"{zh_CN_Label}：",
                                    BindingPath = eN_Label,
                                    Sort = extendConfig.Sort,
                                    Desc = desc,
                                    CurrentValue = value
                                });
                                break;
                            case FieldType.ComboBox:
                                var jtokens = base_jsonConfig.SelectTokens(extendConfig.ItemsSource);
                                var source = jtokens.Select(x => Convert.ToString(x)).ToList();
                                formFields.Add(new ComboBoxMetadata()
                                {
                                    Label = IsZh_CH ? $"{zh_CN_Label}：" : $"{eN_Label}：",
                                    EN_Label = $"{eN_Label}：",
                                    Zh_CN_Label = $"{zh_CN_Label}：",
                                    BindingPath = eN_Label,
                                    Sort = extendConfig.Sort,
                                    Desc = desc,
                                    CurrentValue = value,
                                    ItemsSource = new ObservableCollection<string>(source),
                                    ItemsSourceRoute = extendConfig.ItemsSource
                                });
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                formFieldMetadata.AddRange(formFields.OrderBy(x => x.Sort).ToList());

            }
            catch (Exception ex)
            {

            }

        }

        private async Task<bool> SaveConfig<T>(ObservableCollection<FormFieldMetadata> formFieldMetadata, string key, string saveKey, Action successCallback)
        {
            try
            {
                var config = _configuration.GetSection(key).Get<T>();
                if (config == null)
                {
                    return false;
                }
                PropertyInfo[] propertyInfos = this.GetType().GetProperties();
                if (propertyInfos == null)
                {
                    return false;
                }
                PropertyInfo[] _propertyInfos = config.GetType().GetProperties();


                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.extend.json");
                var jsonContent = File.ReadAllText(configPath);
                var jsonConfig = JObject.Parse(jsonContent);

                foreach (var item in _propertyInfos)
                {
                    FormFieldMetadata fieldMetadata = formFieldMetadata.FirstOrDefault(x => x.BindingPath == item.Name);
                    if (fieldMetadata?.CurrentValue == null) continue;
                    object value = ConvertValue(item.PropertyType, fieldMetadata.CurrentValue);
                    item.SetValue(config, value);
                }

                List<string> withOutKeys = new List<string>();
                foreach (var item in _propertyInfos)
                {
                    string item_key = $"{saveKey}.{item.Name}";
                    var extendJToken = (JToken)jsonConfig.SelectToken(item_key);
                    if (extendJToken == null)
                    {
                        continue;
                    }
                    if (extendJToken.Type == JTokenType.Array)
                    {
                        withOutKeys.Add(item_key);
                        continue;
                    }
                    var extendJObject = (JObject)extendJToken;

                    if (extendJObject["zh_cn"] == null || extendJObject["Sort"] == null || extendJObject["IsEnable"] == null)
                    {
                        withOutKeys.Add(item_key);
                        continue;
                    }
                    ExtendConfig extendConfig = JsonConvert.DeserializeObject<ExtendConfig>(extendJObject.ToString());
                    if (!extendConfig.IsEnable)
                    {
                        withOutKeys.Add(item_key);
                    }
                }
                return await SaveConfig(saveKey, withOutKeys.ToArray(), config, successCallback);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private object ConvertValue(Type targetType, object value)
        {
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value.ToString());

            if (underlyingType == typeof(bool))
            {
                string strVal = value.ToString().ToLower();
                return strVal == "true" || strVal == "1";
            }

            return Convert.ChangeType(value, underlyingType);
        }

        private async Task<bool> SaveConfig(string key, string[] withOutKeys, object value, Action successCallback)
        {
            try
            {
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var jsonContent = File.ReadAllText(configPath);
                var jsonConfig = JObject.Parse(jsonContent);

                //暂存不需要修改的配置项
                Dictionary<string, JToken> dic = new Dictionary<string, JToken>();
                if (withOutKeys != null)
                {
                    foreach (var item in withOutKeys)
                    {
                        dic.Add(item, jsonConfig.SelectToken(item));
                    }
                }

                //写入配置文件
                JToken updatejson = jsonConfig.SelectToken(key);
                updatejson.Replace(JObject.FromObject(value));


                if (withOutKeys != null)
                {
                    foreach (var item in withOutKeys)
                    {
                        JToken itemjson = jsonConfig.SelectToken(item);
                        itemjson.Replace(dic[item]);
                    }
                }

                await File.WriteAllTextAsync(configPath, jsonConfig.ToString());

                //重新加载配置
                var configRoot = (IConfigurationRoot)_configuration;
                configRoot.Reload();
                successCallback?.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Uri.OriginalString == "Config" && isLoad)
            {
                if (!CheckUpdateConfigAuth())
                {
                    IRegionNavigationJournal journal = navigationContext.NavigationService.Journal;
                    if (journal.CanGoBack)
                    {
                        journal.GoBack();
                    }
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (navigationContext.Uri.OriginalString == "Config")
            {
                isLoad = false;
            }
            else
            {
                isLoad = true;
            }
        }

        public AsyncDelegateCommand SaveStationConfigCommand => new(SaveStationConfig);
        public AsyncDelegateCommand SaveAutoUpdateCommand => new(SaveAutoUpdate);
        public AsyncDelegateCommand SaveDeviceConfigCommand => new(SaveDeviceConfig);
        public DelegateCommand ChangeLanguageCommand => new(ChangeLanguage);
    }


    public abstract class FormFieldMetadata : BindableBase
    {
        public string _Label;
        public string Label
        {
            get => _Label;
            set => SetProperty(ref _Label, value);
        }

        public string Zh_CN_Label { get; set; }

        public string EN_Label { get; set; }


        public string _Desc;
        public string Desc
        {
            get => _Desc;
            set => SetProperty(ref _Desc, value);
        }

        public string BindingPath { get; set; } // 路径字符串

        public int Sort { get; set; }

        private object _CurrentValue;
        public object CurrentValue
        {
            get => _CurrentValue;
            set => SetProperty(ref _CurrentValue, value); // Prism 的 SetProperty 触发通知
        }

        public string ItemsSourceRoute { get; set; }

        private ObservableCollection<string> _ItemsSource;
        public ObservableCollection<string> ItemsSource
        {
            get => _ItemsSource;
            set => SetProperty(ref _ItemsSource, value);
        }
        public FieldType FieldType { get; set; }  // 控件类型
    }

    // 文本框字段
    public class TextFieldMetadata : FormFieldMetadata
    {
        public TextFieldMetadata() => FieldType = FieldType.TextBox;
    }

    // 单选按钮组字段
    public class RadioGroupMetadata : FormFieldMetadata
    {
        public RadioGroupMetadata() => FieldType = FieldType.RadioGroup;
    }

    // 数字输入字段
    public class NumericFieldMetadata : FormFieldMetadata
    {
        public NumericFieldMetadata() => FieldType = FieldType.NumericUpDown;
    }
    public class PasswordBoxMetadata : FormFieldMetadata
    {
        public PasswordBoxMetadata() => FieldType = FieldType.PasswordBox;
    }

    public class ComboBoxMetadata : FormFieldMetadata
    {
        public ComboBoxMetadata() => FieldType = FieldType.ComboBox;
    }

    public class FieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; }
        public DataTemplate RadioTemplate { get; set; }
        public DataTemplate NumericTemplate { get; set; }

        public DataTemplate PasswordBoxTemplate { get; set; }

        public DataTemplate ComboBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) =>
            item switch
            {
                TextFieldMetadata => TextBoxTemplate,
                RadioGroupMetadata => RadioTemplate,
                NumericFieldMetadata => NumericTemplate,
                PasswordBoxMetadata => PasswordBoxTemplate,
                ComboBoxMetadata => ComboBoxTemplate,
                _ => null
            };
    }
}

