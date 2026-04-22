using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 透析充分性记录
    /// </summary>
    public class DialysisAdequacyRecord
    {
        public string Id { get; set; }
        /// <summary>
        /// 患者ID
        /// </summary>
        public string PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public Patient PatientDar { get; set; }
        /// <summary>
        /// 透析前尿素氮
        /// </summary>
        public decimal? C01 { get; set; }
        /// <summary>
        /// 透析后尿素氮
        /// </summary>
        public decimal? C02 { get; set; }
        /// <summary>
        /// 第二次透析前尿素氮
        /// </summary>
        public decimal? C03 { get; set; }
        /// <summary>
        /// 透析前体重
        /// </summary>
        public decimal? W01 { get; set; }
        /// <summary>
        /// 透析后体重
        /// </summary>
        public decimal? W02 { get; set; }
        /// <summary>
        /// 透析时间
        /// </summary>
        public decimal? T { get; set; }
        /// <summary>
        /// 透析间期时间
        /// </summary>
        public decimal? Q { get; set; }
        /// <summary>
        /// URR
        /// </summary>
        public decimal? URR { get; set; }
        /// <summary>
        /// Kt/V
        /// </summary>
        public decimal? KtV { get; set; }
        /// <summary>
        /// TACurea
        /// </summary>
        public decimal? TACurea { get; set; }
        /// <summary>
        /// PCR
        /// </summary>
        public decimal? PCR { get; set; }
        /// <summary>
        /// NPCR
        /// </summary>
        public decimal? NPCR { get; set; }
        /// <summary>
        /// 是否归档
        /// </summary>
        public string IsFiled { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }

        public string CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public CenterDialysis centerDar { get; set; }

        public DateTime? CollectData { get; set; }

        /// <summary>
        /// 透析器型号
        /// </summary>
        public string Dialyzer { get; set; }
        /// <summary>
        /// 血流量
        /// </summary>
        /// <summary>
        /// 记录时间
        /// </summary>

        public DateTime? RecordDate { get; set; }
        /// <summary>
        /// 透析模式
        /// </summary>
        public string DialysisType { get; set; }
    }


     
    public class LaboratoryItemSetting
    {
        /// <summary>
        /// 检验项目分类ID
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 检验代号
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 组合项名称
        /// </summary>
        public string CombineName { get; set; }

        /// <summary>
        /// 参考值
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// 正常值上限
        /// </summary>
        public string HighValue { get; set; }

        /// <summary>
        /// 正常值下限
        /// </summary>
        public string LowValue { get; set; }

        /// <summary>
        /// 值检验标准
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// 是否可删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 编号：主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
    }


    public class LaboratoryItemRecord
    {
        /// <summary>
        /// 患者ID
        /// </summary>
        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        /// <summary>
        /// 检验项目ID
        /// </summary>
        public string InspectionItemId { get; set; }

        [ForeignKey("InspectionItemId")]
        public LaboratoryItemSetting itemSetting { get; set; }
        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// 结果值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 检验单号
        /// </summary>
        public string ReportNo { get; set; }

        /// <summary>
        /// 编号：主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int  DataState { get; set; }

        public string CenterId { get; set; }
    }
    public class LaboratoryCategorySetting
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 检验代号
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否可删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 编号：主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }



    }



    public class SensingData
    {
        public string Id { get; set; }
        public string ReportNo { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Region { get; set; }
        public string ProjectName { get; set; }
        public string ItemDetail { get; set; }
        public string Value { get; set; }
        public string Suggestion { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int DataState { get; set; }
    }


    public class SensingDataOutPut : SensingData
    {
        public string CenterName { get; set; }
    }

}
