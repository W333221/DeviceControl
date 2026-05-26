using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Model
{
    public class LogItem
    {
        public DateTime Time { get; set; }

        public string Level { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Exception { get; set; }

        public string Display =>
            $"{Time:HH:mm:ss.fff} [{Level}] {Message}" +
            (string.IsNullOrWhiteSpace(Exception) ? "" : Environment.NewLine + Exception);
    }
}
