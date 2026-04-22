using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 患者基本信息
    /// </summary>
    public class Patient
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
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 医保卡号
        /// </summary>
        public string SICardNum { get; set; }
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
        public string CenterId { get; set; }
        /*在院信息*/
        /// <summary>
        /// 所在机构
        /// </summary>
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis Dialysis { get; set; }

        /// <summary>
        /// 在院状态
        /// </summary>
        public string HospitalState { get; set; }
        [ForeignKey(nameof(HospitalState))]
        public virtual SystemDictionary SHospitalState { get; set; }
        /// <summary>
        /// 接诊日期
        /// </summary>
        public DateTime? ReceiveDate { get; set; }
        /// <summary>
        /// 接诊医生
        /// </summary>
        public string ReceiveDoctor { get; set; }
        /// <summary>
        /// 首次治疗日期
        /// </summary>
        public DateTime? FirsthHematodialysisDate { get; set; }
        /// <summary>
        /// 医保类别
        /// </summary>
        public string SIInsuredType { get; set; }
        [ForeignKey(nameof(SIInsuredType))]
        public virtual SystemDictionary SSIInsuredType { get; set; }
        /// <summary>
        /// RH血型
        /// </summary>
        public string RHBloodType { get; set; }
        [ForeignKey(nameof(RHBloodType))]
        public virtual SystemDictionary SRHBloodType { get; set; }

        /// <summary>
        /// 血源性疾病
        /// </summary>
        public string BloodBorneDisease { get; set; }
        [ForeignKey(nameof(BloodBorneDisease))]
        public virtual SystemDictionary SBloodBorneDisease { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string Marital { get; set; }
        [ForeignKey(nameof(Marital))]
        public virtual SystemDictionary SMarital { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public string EducationBackground { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        [ForeignKey(nameof(EducationBackground))]
        public virtual SystemDictionary SEducationBackground { get; set; }


        /// <summary>
        /// 血型
        /// </summary>
        public string ABOBloodType { get; set; }
        [ForeignKey(nameof(ABOBloodType))]
        public virtual SystemDictionary SABOBloodType { get; set; }
        /// <summary>
        /// <summary>
        /// 治疗编号
        /// </summary>
        public string PatientNo { get; set; }
        /// <summary>
        /// 主管医生
        /// </summary>
        public string ChargeDoctor { get; set; }
        [ForeignKey(nameof(ChargeDoctor))]
        public virtual Employee ChargeDoctorEmployee { get; set; }


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
        /// 职业
        /// </summary>
        public string Professional { get; set; }
        /// <summary>
        /// <summary>
        /// 工作单位
        /// </summary>
        public string WorkUnit { get; set; }

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
        /// <summary>
        /// 离院时间
        /// </summary>
        public DateTime? LeaveDate { get; set; }
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
        public bool? IsDelete { get; set; }
        public string PatientFileNo { get; set; }
        /// <summary>
        /// 转归记录
        /// </summary>
        public virtual List<TransferRecord> TransferRecords { get; set; }


        public string Psn_no { get; set; }
    }


    public class TransferRecord
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient patient { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferReason { get; set; }
        public string TransferType { get; set; }
        public string InHospital { get; set; }
        public string PrognosisOutcome { get; set; }
        public DateTime? InOutDate { get; set; }
        public string DoctorId { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
      
        public string CenterId { get; set; }

        public DateTime? CollectData { get; set; } 
    }
}
