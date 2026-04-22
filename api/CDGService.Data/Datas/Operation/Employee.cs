using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class Employee : ISoftDelete
    {

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
        /// 名族
        /// </summary>
        public string Ethnic { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Idcard { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 户口所在地
        /// </summary>
        public string HjAddress { get; set; }
        /// <summary>
        /// 紧急电话
        /// </summary>
        public string EmergencyPhone { get; set; }
        /// <summary>
        /// 现居住地
        /// </summary>
        public string ContactAddress { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DepId { get; set; }
        [ForeignKey(nameof(DepId))]
        public virtual SystemDictionary Sdepartment { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string positionId { get; set; }
        [ForeignKey(nameof(positionId))]
        public virtual SystemDictionary Sposition { get; set; }
        /// <summary>
        /// 在职状态
        /// </summary>
        public string WorkingState { get; set; }
        /// <summary>
        /// 在职状态
        /// </summary>
        [ForeignKey(nameof(WorkingState))]
        public virtual SystemDictionary DicWorkingState { get; set; }
        /// <summary>
        /// 所在科室
        /// </summary>
        public string Department { get; set; }



        /// <summary>
        /// 所在科室
        /// </summary>
        [ForeignKey(nameof(Department))]
        public virtual SystemDictionary DicDepartment { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? hiredate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        public DateTime? positiveDate { get; set; }
        /// <summary>
        /// 合同签订日期
        /// </summary>
        public DateTime? signDate { get; set; }
        /// <summary>
        /// 合同到期日
        /// </summary>
        public DateTime? expireDate { get; set; }

        /// <summary>
        /// 外派合同情况
        /// </summary>
        public string AssignmentStatus { get; set; }
        /// <summary>
        /// 户口性质
        /// </summary>
        public string HouseholdRegister { get; set; }
        /// <summary>
        /// 劳动合同关系
        /// </summary>
        public string LaborContract { get; set; }
        /// <summary>
        /// 举荐人
        /// </summary>
        public string Recommendations { get; set; }
        /// <summary>
        /// 举荐人电话
        /// </summary>
        public string RecommendationsPhone { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string EduId { get; set; }
        [ForeignKey(nameof(EduId))]
        public virtual SystemDictionary SEducation { get; set; }
        /// <summary>
        /// 毕业院校（起点）
        /// </summary>
        public string startSchool { get; set; }
        /// <summary>
        /// 毕业院校（最高）
        /// </summary>
        public string highestSchool { get; set; }
        /// <summary>
        /// 毕业院校（现有毕业证）
        /// </summary>
        public string GraduateSchool { get; set; }
        /// <summary>
        /// 最高学历
        /// </summary>
        public string EducationBackground { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? GraduationDate { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        public string JobTitleId { get; set; }
        [ForeignKey(nameof(JobTitleId))]
        public virtual SystemDictionary SJobTitle { get; set; }
        /// <summary>
        /// 血透经验年限
        /// </summary>
        public decimal? JobYear { get; set; }
        /// <summary>
        /// 工作经历1
        /// </summary>
        public string ExperienceJob { get; set; }
        /// <summary>
        /// 工作经历2
        /// </summary>
        public string ExperienceJobTwo { get; set; }

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
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 所在机构
        /// </summary>
        public string CenterDialysisId { get; set; }
        [ForeignKey(nameof(CenterDialysisId))]
        public virtual CenterDialysis CenterDialysis { get; set; }

        public virtual List<EmployeeTransfer> employeeTransfers { get; set; }
        /// <summary>
        /// 医师代码
        /// </summary>
        public string NationDoctCode { get; set; }
    }

    /// <summary>
    /// 员工机构、职位调动记录表
    /// </summary>
    public class EmployeeTransfer
    {
        public string Id { get; set; }
        public string EmpId { get; set; }
        [ForeignKey(nameof(EmpId))]
        public virtual Employee employee { get; set; }
        public string OriginalCenterId { get; set; }
        [ForeignKey(nameof(OriginalCenterId))]
        public virtual CenterDialysis orcenterDialysis { get; set; }
        public string NowCenterId { get; set; }
        [ForeignKey(nameof(NowCenterId))]
        public virtual CenterDialysis nowcenterDialysis { get; set; }
        public string OriginalPositionId { get; set; }
        [ForeignKey(nameof(OriginalPositionId))]
        public SystemDictionary OriginalPosition { get; set; }
        public string NowPositionId { get; set; }

        [ForeignKey(nameof(NowPositionId))]
        public SystemDictionary NowPosition { get; set; }
        public int? DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Remark { get; set; }
       

    }
}
