using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Core.Service.DeviceService
{
    /*
         * 将不同SDK的结果统一到TightenData上来，
         * 方便BLL层处理
         * 
         */
    /// <summary>
    /// 拧紧机输出的结果
    /// </summary>
    public class TightenData
    {
        public string ToolId { get; set; }
        /// <summary>
        /// 工具类型
        /// </summary>
        public string ToolType { get; set; }
        /// <summary>
        /// 车身编号
        /// </summary>
        public string BodyNumber { get; set; }
        /// <summary>
        /// VIN号
        /// </summary>
        public string EngineCode { get; set; }
        /// <summary>
        /// 程序号
        /// </summary>
        public int Pset { get; set; }
        /// <summary>
        /// 螺栓序号
        /// </summary>
        public int BoltNo { get; set; }
        /// <summary>
        /// 螺栓总数
        /// </summary>
        public int BoltCount { get; set; }
        /// <summary>
        /// 力矩
        /// </summary>
        public decimal Torque { get; set; }//double
        /// <summary>
        /// 角度
        /// </summary>
        public decimal Angle { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 批次总结果
        /// </summary>
        public int JobResult { get; set; }
        /// <summary>
        /// 力矩结果
        /// </summary>
        public int TorqueStatus;
        /// <summary>
        /// 角度结果
        /// </summary>
        public int AngleStatus;
        /// <summary>
        /// 拧紧时间
        /// </summary>
        public DateTime TightenTime { get; set; }

        /// <summary>
        /// 拧紧ID
        /// </summary>
        public int TighteningId { get; set; }
        public TightenData()
        {
            TightenTime = DateTime.Now;
            JobResult = 0;
        }

        /// <summary>
        /// 目标力矩
        /// </summary>        
        public decimal TorqueTarget { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 放弃螺栓数量
        /// </summary>
        public int GiveUpCount { get; set; }
        /// <summary>
        /// 工具编号
        /// </summary>
        public string ToolSerialNumber { get; set; }
        /// <summary>
        /// 是否放弃
        /// </summary>
        public int Brand { get; set; }
        /// <summary>
        /// Job号
        /// </summary>
        public int JobID { get; set; }

    }
}
