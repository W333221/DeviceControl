using DeviceControl.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.ServiceContract
{
    public interface ILogViewerService
    {
        event Action<LogItem>? LogAdded;

        IReadOnlyCollection<LogItem> GetLogs();

        void Add(LogItem item);

        void Clear();
    }
}
