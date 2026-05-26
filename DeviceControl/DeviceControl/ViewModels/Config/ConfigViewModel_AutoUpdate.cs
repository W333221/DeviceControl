using DeviceControl.Data.Configs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.ViewModels.Config
{
    public partial class ConfigViewModel : BindableBase
    {
        public ObservableCollection<FormFieldMetadata> AutoUpdate_FormFields { get; }
       = new ObservableCollection<FormFieldMetadata>();

        private void ReadAutoUpdate()
        {
            ReadConfig<AutoUpdateConfig>(AutoUpdate_FormFields, "AutoUpdate", "$.AutoUpdate");
        }

        private void UpdateComboBoxItemsSource_AutoUpdate()
        {
            UpdateComboBoxItemsSource(AutoUpdate_FormFields, new string[] { "MQTTConfigName" });
        }

        private async Task SaveAutoUpdate()
        {
            if (!CheckUpdateConfigAuth())
            {
                return;
            }
            ShowGrowlInfo(await SaveConfig<AutoUpdateConfig>(AutoUpdate_FormFields, "AutoUpdate", "$.AutoUpdate", () => ReadAutoUpdate()));
        }
    }
}

