using DeviceControl.Core.ServiceContract;
using DeviceControl.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service
{
    public class LogViewerService : ILogViewerService
    {
        private readonly object _lock = new();
        private readonly LinkedList<LogItem> _logs = new();

        private const int MaxCount = 100;

        public event Action<LogItem>? LogAdded;

        public IReadOnlyCollection<LogItem> GetLogs()
        {
            lock (_lock)
            {
                return _logs.ToList().AsReadOnly();
            }
        }

        public void Add(LogItem item)
        {
            lock (_lock)
            {
                _logs.AddLast(item);

                while (_logs.Count > MaxCount)
                {
                    _logs.RemoveFirst();
                }
            }

            LogAdded?.Invoke(item);
        }

        public void Clear()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }
    }
}
