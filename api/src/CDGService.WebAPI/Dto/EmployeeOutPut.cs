using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class EmployeeOutPut
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
        public string Birthday { get; set; }
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
        public string department { get; set; }
        public string DepId { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }
        public string positionId { get; set; }

        /// <summary>
        /// 在职状态ID
        /// </summary>
        public string WorkingState { get; set; }

        /// <summary>
        /// 在职状态 
        /// </summary>
        public string StrWorkingState { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public string hiredate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        public string positiveDate { get; set; }
        /// <summary>
        /// 合同签订日期
        /// </summary>
        public string signDate { get; set; }
        /// <summary>
        /// 合同到期日
        /// </summary>
        public string expireDate { get; set; }

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
        public string Education { get; set; }
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
        /// 专业
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public string GraduationDate { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        public string JobTitle { get; set; }
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

        /// <summary>
        /// 工作经历 
        /// </summary>
        public List<string> ExperienceJobs { get; set; } = new List<string>();

        /// <summary>
        /// 调动记录
        /// </summary>
        public List<string> EmployeeTransfer { get; set; } = new List<string>();

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
        ///// <summary>
        ///// 数据状态
        ///// </summary>
        public int DataState { get; set; }

        public string CenterDialysisId { get; set; }
        /// <summary>
        /// 所在中心
        /// </summary>
        public string CenterDialysisName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifierDate { get; set; }
        /// <summary> 
        /// 医师代码  
        /// </summary>
        public string NationDoctCode { get; set; }
    }

    public class EmployeeTransferOutPut
    {
        public string Id { get; set; }

        public string strEmployeeTransfer { get; set; }
    }

    /// <summary>
    /// 根据机构统计员工输出实体
    /// </summary>
    public class EmplyeeByDiaOutPut
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string DialysisId { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string DialysisName { get; set; }
        /// <summary>
        /// 人员总数
        /// </summary>
        public int EmlyeeCount { get; set; }
        /// <summary>
        /// 患者总数
        /// </summary>
        public int PatientCount { get; set; }

        /// <summary>
        /// 设备总数
        /// </summary>
        public int EquipCount { get; set; }
    }

    public class EmplyeeByStaBisOutPut
    {
        /// <summary>
        /// count
        /// </summary>
        public double value { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
    }

    public class LineOutPut
    {
        /// <summary>
        /// xAxis
        /// </summary>
        public List<string> data { get; set; } = new List<string>();
        /// <summary>
        /// Legend
        /// </summary>
        public List<string> LegendData { get; set; } = new List<string>();
        /// <summary>
        /// serisesData
        /// </summary>
        public List<serise> serises { get; set; } = new List<serise>();
    }
    public class serise
    {
        public string name { get; set; }
        public List<double> data { get; set; } = new List<double>();
    }


    #region 预调动输入输出
    public class PerQuerInput
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName { get; set; }
        public int pageSize { get; set; }
        public int PageNum { get; set; }
    }

    public class PerEmpTransferOutPut
    {
        public string Id { get; set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public string EmpId { get; set; }
        public string employeeName { get; set; }
        /// <summary>
        /// 原部门
        /// </summary>
        public string OriginalDep { get; set; }
        /// <summary>
        /// 原职位
        /// </summary>
        public string OriginalPosition { get; set; }
        /// <summary>
        /// 原机构
        /// </summary>
        public string OriginalCenter { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string PresentDepId { get; set; }
        public string PresentDep { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string PresentPositionId { get; set; }
        public string PresentPosition { get; set; }
        /// <summary>
        /// 机构
        /// </summary>
        public string PresentCenterId { get; set; }

        public string PresentCenter { get; set; }
        /// <summary>
        /// 预调动时间
        /// </summary>
        public DateTime? TransferDate { get; set; }

        /// <summary>
        /// 调回时间
        /// </summary>
        public DateTime? BackDate { get; set; }
        /// <summary>
        /// 调动状态 1有效 2 撤回（无效）3 删除
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 是否已执行调动
        /// </summary>
        public bool IsTransfer { get; set; }
    }



    public class PerEmpTransferInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 人员ID
        /// </summary>
        public string EmpId { get; set; }

        /// <summary>
        /// 调动部门
        /// </summary>
        public string PresentDepId { get; set; }
        /// <summary>
        /// 调动职位
        /// </summary>
        public string PresentPositionId { get; set; }
        /// <summary>
        /// 调动机构
        /// </summary>
        public string PresentCenterId { get; set; }

        /// <summary>
        /// 预调动时间
        /// </summary>
        public DateTime TransferDate { get; set; }

        /// <summary>
        /// 调回时间
        /// </summary>
        public DateTime BackDate { get; set; }
    }


    #endregion 




}
