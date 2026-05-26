using DeviceControl.Core.ServiceContract;
using DeviceControl.Data.Model;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Command
{
    public class UiLogSink : ILogEventSink
    {
        private readonly ILogViewerService _logViewerService;

        public UiLogSink(ILogViewerService logViewerService)
        {
            _logViewerService = logViewerService;
        }

        public void Emit(LogEvent logEvent)
        {
            var item = new LogItem
            {
                Time = logEvent.Timestamp.LocalDateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                Exception = logEvent.Exception?.ToString()
            };

            _logViewerService.Add(item);
        }
    }
}
