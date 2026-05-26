using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Model.Tighten
{
    public class CurveData
    {
        public int TighteningID { get; set; }
        public string AnglePoints { get; set; }
        public string TorquePoints { get; set; }
        public string CurrentPoints { get; set; }
        public int PointCount { get; set; }
        public double TimeCoefficient { get; set; }
        public int BoltNO { get; set; }
    }
}
