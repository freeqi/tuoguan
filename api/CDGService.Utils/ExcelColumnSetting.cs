using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils
{
    /// <summary>
    /// Excel导出列设置
    /// </summary>
    public class ExcelColumnSetting
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public ExcelColumnSetting()
        {

        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExcelColumnSetting(string propertyName, string header)
        {
            PropertyName = propertyName;
            Header = header;
        }

        /// <summary>
        /// 实体属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 布尔值中文描述
        /// </summary>
        public BoolChineseDesc BoolChineseDesc { get; set; }

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFromat { get; set; }

        /// <summary>
        /// 数值格式
        /// </summary>
        public string NumberFormat { get; set; }

        /// <summary>
        /// 是否自动适应宽度
        /// </summary>
        public bool IsAutoFitWidth { get; set; } = true;

        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool WrapText { get; set; }

        /// <summary>
        /// 是否序号列
        /// </summary>
        public bool IsSeqColumn { get; set; }
    }
}
