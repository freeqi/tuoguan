using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 物品基础单位
    /// </summary>
    public class MedicalUnit:ISoftDelete
    {
        public string Id { get; set; }
        /// <summary>
        /// 系统编码 U开头
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 代码值
        /// </summary>
        public string UnitCode { get; set; }
        /// <summary>
        /// 中文单位
        /// </summary>
        public string SpeUnitCHS { get; set; }
        /// <summary>
        /// 英文单位
        /// </summary>
        public string SpeUnitUS { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNum { get; set; }
        /// <summary>
        /// 单位类型
        /// 1制剂单位：瓶、片、支……
        ///2包装单位：箱、盒、包……
        ///3剂量单位：克、毫克……
        ///4其他通用单位：个，台、辆
        /// </summary>
        public int UnitType { get; set; }
        public int  DataState { get; set; }
        public string Remark { get; set; }
        public string  Founder { get; set; }
        public DateTime  FounderDate { get; set; }
        public string  Modifier { get; set; }
        public DateTime  ModifierDate { get; set; }

        public bool IsDelete { get; set; }
    }
}
