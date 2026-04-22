using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{

    public class PatientOutPut
    {
        /*基本信息*/
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 民族
        /// </summary> 
        public string Nation { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardNum { get; set; }

        /// <summary>
        /// 户籍地址  
        /// </summary>
        public string CensusRegister { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CenterDialysisId { get; set; }
        /*在院信息*/
        /// <summary>
        /// 所在机构
        /// </summary>
       // [ForeignKey(nameof(CenterDialysisId))]
        public string Dialysis { get; set; }

        /// <summary>
        /// 在院状态
        /// </summary>
        //  public string HospitalState { get; set; }
        // [ForeignKey(nameof(HospitalState))]
        public string SHospitalState { get; set; }
        /// <summary>
        /// 接诊日期
        /// </summary>
        public string ReceiveDate { get; set; }
        /// <summary>
        /// 接诊医生
        /// </summary>
        public string ReceiveDoctor { get; set; }
        /// <summary>
        /// 首次治疗日期
        /// </summary>
        public string FirsthHematodialysisDate { get; set; }
        /// <summary>
        /// 医保类别
        /// </summary>
        public string SIInsuredType { get; set; }
        //[ForeignKey(nameof(SIInsuredType))]
        public string SSIInsuredType { get; set; }
        /// <summary>
        /// RH血型
        /// </summary>
        public string RHBloodType { get; set; }
        // [ForeignKey(nameof(RHBloodType))]
        public string SRHBloodType { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public string ABOBloodType { get; set; }

        public string SABOBloodType { get; set; }
        /// <summary>
        /// 血源性疾病
        /// </summary>
        public string BloodBorneDisease { get; set; }
        //  [ForeignKey(nameof(BloodBorneDisease))]
        public string SBloodBorneDisease { get; set; }
        /// <summary>
        /// <summary>
        /// 治疗编号
        /// </summary>
        public string PatientNo { get; set; }
        /// <summary>
        /// 主管医生
        /// </summary>
        public string ChargeDoctor { get; set; }
        /*联系方式*/
        /// <summary>
        /// 联系地址
        /// </summary>
        public string ContactAddress { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string WorkUnit { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public string Professional { get; set; }
        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string EmergencyContact1 { get; set; }
        /// <summary>
        /// 紧急联系电话
        /// </summary>
        public string EmergencyContactNumber1 { get; set; }
        /// <summary>
        /// 关系
        /// </summary>
        public string Relationship1 { get; set; }

        public string EducationBackground { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>

        public string SEducationBackground { get; set; }
        public string Marital { get; set; }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string SMarital { get; set; }
        /// <summary>
        /// 医保卡号
        /// </summary>
        public string SICardNum { get; set; }

    }

    public class PatientQueryInPut
    {
        public string id { get; set; }
        public string Name { get; set; }

        public string CenterId { get; set; }

        public string HospitalStateId { get; set; }

        public int PageNum { get; set; }
        public int PageSize { get; set; }

    }

    public class DialysisAdequacyQueryInPut
    {
        public string CenterId { get; set; }
        public string PatientId { get; set; }
        /// <summary>
        /// 透析器型号
        /// </summary>
        public string Dialyzer { get; set; }
        /// <summary>
        /// 透析模式
        /// </summary>
        public string DialysisType { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>

        public DateTime? BeginRecordDate { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>

        public DateTime? EndRecordDate { get; set; }
    }


    public class PatientInspQueryInPut
    {
        public string patientId { get; set; }
        public string categoryId { get; set; }

        public string CenterId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        //public int PageNum { get; set; }
        //public int PageSize { get; set; }

    }

    public class SensingDataQueryInPut
    {
        
        public string ProjectName { get; set; }

        public string CenterId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; } 

    }
    public class SensingDataProjectName
    {

        public string ProjectName { get; set; }

       

    }

}
