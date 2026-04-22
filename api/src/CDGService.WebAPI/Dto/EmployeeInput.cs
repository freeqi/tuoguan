using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class EmployeeInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 姓名 - 非空唯一
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别 - 非空
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 名族
        /// </summary>
        public string Ethnic { get; set; }
        /// <summary>
        /// 身份证号码 - 非空
        /// </summary>
        public string Idcard { get; set; }
        /// <summary>
        /// 出生日期 - 非空
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 联系方式 - 非空
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 户口所在地 - 非空
        /// </summary>
        public string HjAddress { get; set; }
        /// <summary>
        /// 紧急电话
        /// </summary>
        public string EmergencyPhone { get; set; }
        /// <summary>
        /// 现居住地 - 非空
        /// </summary>
        public string ContactAddress { get; set; }
        /// <summary>
        /// 部门 - 非空
        /// </summary>
        public string DepId { get; set; }
        public string WorkingState { get; set; }
        /// <summary>
        /// 职位 - 非空
        /// </summary>
        public string positionId { get; set; }
        /// <summary>
        /// 入职时间 - 非空
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
        /// 户口性质 - 非空
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
        /// 学历 - 非空
        /// </summary>
        public string EduId { get; set; }

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
        /// 专业 - 非空
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

        ///// <summary>
        ///// 创建人
        ///// </summary>
        //public string Founder { get; set; }
        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //public DateTime FounderDate { get; set; }
        ///// <summary>
        ///// 修改人
        ///// </summary>
        //public string Modifier { get; set; }
        ///// <summary>
        ///// 修改时间
        ///// </summary>
        //public DateTime ModifierDate { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        public string CenterDialysisId { get; set; }
        /// <summary>
        /// 医师代码
        /// </summary>
        public string NationDoctCode { get; set; }
    }

    public class EmployeeQueryInput
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string DialysisId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName { get; set; }
        public int pageSize { get; set; }
        public int PageNum { get; set; }

        /// <summary>
        /// 只筛选医生护士 1查医生 2查护士 3查医护 0查全部
        /// </summary>
        public int  IsQureydoctor { get; set; }
    }
}
