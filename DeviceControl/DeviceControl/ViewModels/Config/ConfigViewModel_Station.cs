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
        public ObservableCollection<FormFieldMetadata> Station_FormFields { get; }
        = new ObservableCollection<FormFieldMetadata>();
        private void ReadStationConfig()
        {
            ReadConfig<StationConfig>(Station_FormFields, "AppSettings:Station", "$.AppSettings.Station");
        }



        private async Task SaveStationConfig()
        {
            if (!CheckUpdateConfigAuth())
            {
                return;
            }
            ShowGrowlInfo(await SaveConfig<StationConfig>(Station_FormFields, "AppSettings:Station", "$.AppSettings.Station", () => ReadStationConfig()));
        }

        #region Station
        private string _Station_Title;
        public string Station_Title
        {
            get { return _Station_Title; }
            set { SetProperty(ref _Station_Title, value); }
        }

        private string _Station_LineClass;
        public string Station_LineClass
        {
            get { return _Station_LineClass; }
            set { SetProperty(ref _Station_LineClass, value); }
        }

        private string _Station_LineName;
        public string Station_LineName
        {
            get { return _Station_LineName; }
            set { SetProperty(ref _Station_LineName, value); }
        }

        private string _Station_LineCode;
        public string Station_LineCode
        {
            get { return _Station_LineCode; }
            set { SetProperty(ref _Station_LineCode, value); }
        }

        private string _Station_StationID;
        public string Station_StationID
        {
            get { return _Station_StationID; }
            set { SetProperty(ref _Station_StationID, value); }
        }

        private string _Station_StationName;
        public string Station_StationName
        {
            get { return _Station_StationName; }
            set { SetProperty(ref _Station_StationName, value); }
        }

        private string _Station_ClientID;
        public string Station_ClientID
        {
            get { return _Station_ClientID; }
            set { SetProperty(ref _Station_ClientID, value); }
        }

        private bool _Station_Available;
        public bool Station_Available
        {
            get { return _Station_Available; }
            set { SetProperty(ref _Station_Available, value); }
        }

        private int _Station_UploadInterval;
        public int Station_UploadInterval
        {
            get { return _Station_UploadInterval; }
            set { SetProperty(ref _Station_UploadInterval, value); }
        }

        private bool _Station_ErrToPBS;
        public bool Station_ErrToPBS
        {
            get { return _Station_ErrToPBS; }
            set { SetProperty(ref _Station_ErrToPBS, value); }
        }

        private int _Station_TypeMode;
        public int Station_TypeMode
        {
            get { return _Station_TypeMode; }
            set { SetProperty(ref _Station_TypeMode, value); }
        }

        private int _Station_InStationDelayTime;
        public int Station_InStationDelayTime
        {
            get { return _Station_InStationDelayTime; }
            set { SetProperty(ref _Station_InStationDelayTime, value); }
        }
        private bool _Station_IsSubAssembly;
        public bool Station_IsSubAssembly
        {
            get { return _Station_IsSubAssembly; }
            set { SetProperty(ref _Station_IsSubAssembly, value); }
        }
        #endregion

    }
}
