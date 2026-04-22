using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    /// <summary>
    /// 字典类型ID
    /// </summary>
    public class DictionaryCode
    {
        /// <summary>
        /// 性别
        /// </summary>
        public string SexTypeId { get; set; } //性别
        public string CenterSexTypeId { get; set; }
        /// <summary>
        /// 医保
        /// </summary>
        public string SIInsuredTypeID { get; set; } //医保

        /// <summary>
        /// 职工医保ID
        /// </summary>
        public string SIInsuredID { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DepTypeId { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        public string JobTitleTypeId { get; set; } //职称
        /// <summary>
        /// 在职情况
        /// </summary>
        public string WorkStateTypeId { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string EduTypeId { get; set; } //学历
        /// <summary>
        /// 职业
        /// </summary>
        public string ProfessionalTypeId { get; set; } //职业

        public string HospitalStateTypeId { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string ModelTypeId { get; set; } //设备 型号
        /// <summary>
        /// 透析 设备ID
        /// </summary>
        public string EqTypeId { get; set; }
        /// <summary>
        /// 血源性疾病
        /// </summary>
        public string BloodBorneDiseaseTypeId { get; set; } //血源性疾病 

        /// <summary>
        /// 药品
        /// </summary>

        public string DrugArchivesID { get; set; }
        /// <summary>
        /// 卫生耗材
        /// </summary>
        public string HealthSuppliesID { get; set; }
        /// <summary>
        /// 固定资产
        /// </summary>
        public string FixedAssetsID { get; set; }
        /// <summary>
        /// 低值易耗品
        /// </summary>
        public string LowValueID { get; set; }
        /// <summary>
        /// 诊疗项目
        /// </summary>
        public string TreatmentItmeID { get; set; }

        /// <summary>
        /// 费用类别
        /// </summary>
        public string FeeTypeId { get; set; }
        /// <summary>
        /// 患者在院ID
        /// </summary>
        public string HStateId { get; set; }

    }
}
