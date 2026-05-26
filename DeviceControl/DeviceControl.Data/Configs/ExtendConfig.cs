using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControl.Data.Configs
{
    public class ExtendConfig
    {
        /// <summary>
        /// 中文说明
        /// </summary>
        public string zh_cn { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// 下拉框数据源
        /// </summary>
        public string ItemsSource { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
    }

    public enum FieldType { TextBox, RadioGroup, NumericUpDown, PasswordBox, ComboBox }
}