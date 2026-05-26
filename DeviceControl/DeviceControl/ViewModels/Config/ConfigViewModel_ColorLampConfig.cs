using DeviceControl.Data.Configs.DeviceConfig.DeviceConfig;
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
        public ObservableCollection<FormFieldMetadata> ColorLampConfig_FormFields { get; }
       = new ObservableCollection<FormFieldMetadata>();
        private void ReadColorLampConfig()
        {
            ReadConfig<ColorLampConfig>(ColorLampConfig_FormFields, "AppSettings:DeviceConfig:ColorLampConfig", "$.AppSettings.DeviceConfig.ColorLampConfig");
        }


        private async Task<bool> SaveColorLampConfig()
        {
            return await SaveConfig<ColorLampConfig>(ColorLampConfig_FormFields, "AppSettings:DeviceConfig:ColorLampConfig", "$.AppSettings.DeviceConfig.ColorLampConfig", () => ReadColorLampConfig());
        }

    }
}

