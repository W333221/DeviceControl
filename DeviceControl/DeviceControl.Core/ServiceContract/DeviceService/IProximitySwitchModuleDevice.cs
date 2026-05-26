using DeviceControl.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract.DeviceService
{
    public interface IProximitySwitchModuleDevice
    {
        public Func<List<ProximitySwitchModuleModel>, Task> OnProximitySwitchChanged { get; set; }
        public bool Init();
        public void Close();

        public Task InitAsync();
    }
}
