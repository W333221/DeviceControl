using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data
{
    public class IOC
    {
        private static readonly Lazy<IOC> _instance = new Lazy<IOC>(() => new IOC());
        private IContainerProvider _serviceProvider = null!;
        public IContainerProvider ServiceProvider => _serviceProvider;

        private IOC() { }

        public static IOC Instance => _instance.Value;
        public void Init(IContainerProvider p)
        {
            _serviceProvider = p;
        }
    }
}
