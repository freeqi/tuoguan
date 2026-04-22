using CDGService.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Enums
{
    /// <summary>
    /// 知识库分类
    /// </summary>
    public enum DocumentCatalog
    {
        /// <summary>
        /// 知识库
        /// </summary>
        [ChineseEnum("知识库")]
        KnowledgeFlie = 1,
        /// <summary>
        /// 重要文件
        /// </summary>
        [ChineseEnum("重要文件")]
        ImportantFile = 2,
        /// <summary>
        /// 制度库
        /// </summary>
        [ChineseEnum("制度库")]
        InstitutionFile = 3,
        /// <summary>
        /// 资质库
        /// </summary>
        [ChineseEnum("资质库")]
        QualificationFlie = 4,
        /// <summary>
        /// 培训记录
        /// </summary>
        [ChineseEnum("培训记录")]
        CultivateFlie = 5,
        /// <summary>
        /// 其他库
        /// </summary>
        [ChineseEnum("其他库")]
        OtherFlie = 6,

    }

    public enum KnowledgeType
    {
        /// <summary>
        /// 医疗知识
        /// </summary>
        [ChineseEnum("医疗知识")]
        Medical = 1,
        /// <summary>
        /// 企业文化
        /// </summary>
        [ChineseEnum("企业文化")]
        EnterpriseCulture = 2,
        /// <summary>
        /// 企业文化
        /// </summary>
        [ChineseEnum("质量体系")]
        QualitySystem = 3,
        /// <summary>
        /// 管理制度
        /// </summary>
        [ChineseEnum("管理制度")]
        ManagementSystem = 4,
        /// <summary>
        /// 财务制度
        /// </summary>
        [ChineseEnum("财务制度")]
        FinancialSystem = 5,

        /// <summary>
        /// 物质制度
        /// </summary>
        [ChineseEnum("物质制度")]
        MaterialSystem = 6,

        /// <summary>
        /// 资质库1号
        /// </summary>
        [ChineseEnum("资质库1号")]
        ManagementSystem1 = 7,
        /// <summary>
        /// 资质库2号
        /// </summary>
        [ChineseEnum("资质库2号")]
        FinancialSystem2 = 8,

        /// <summary>
        /// 资质库3号
        /// </summary>
        [ChineseEnum("资质库3号")]
        MaterialSystem3 = 9,

    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FielType
    {

        Word = 1,
        excel = 2,
        ppt = 3,
        text = 4,
        jpg = 5,
        pdf = 6,
        Project = 7,
        other = 8
    }

    /// <summary>
    /// 导入模块
    /// </summary>
    public enum ImportCatalog
    {
        /// <summary>
        /// 员工
        /// </summary>
        [ChineseEnum("员工")]
        Emp = 1,
        /// <summary>
        /// 机构
        /// </summary>
        [ChineseEnum("机构")]
        Centher = 2,
        /// <summary>
        /// 供应商
        /// </summary>
        [ChineseEnum("供应商")]
        Spplier = 3,
        /// <summary>
        /// 药品
        /// </summary>
        Drug = 4,
        /// <summary>
        /// 卫生耗材 
        /// </summary>
        [ChineseEnum("卫生耗材")]
        Material = 5,


        /// <summary>
        /// 固定资产
        /// </summary>
        FixedAssets = 6,
        /// <summary>
        /// 低值易耗品
        /// </summary>
        Low = 7,
        /// <summary>
        /// 诊疗项目
        /// </summary>
        Diagnosis = 8

    }


    /// <summary>
    /// 患者病历分类块
    /// </summary>
    public enum PatientRecords
    {
        /// <summary>
        /// 病历首页
        /// </summary>
        [ChineseEnum("病历首页")]
        FirstPage = 1,
        /// <summary>
        /// 专用病历
        /// </summary>
        [ChineseEnum("血液净化专用病历")]
        FirstOutpatientRecord = 2,
        /// <summary>
        /// 门诊病历/透析记录单
        /// </summary>
        [ChineseEnum("门诊病历_透析记录单")]
        DialysisRecord = 3,
        /// <summary>
        /// 透析方案调整    
        /// </summary>
        [ChineseEnum("透析方案调整记录表")]
        DialysisProgram = 4,
        /// <summary>
        /// 阶段小结
        /// </summary>
        [ChineseEnum("阶段小结")]
        StageSummary = 5,
        /// <summary>
        /// 护理评估记录
        /// </summary>
        [ChineseEnum("护理评估记录")]
        NursingAssessmentRecord =6,
        /// <summary>
        /// 健康宣教    
        /// </summary>
        [ChineseEnum("健康宣教")]
        HealthEducationRecords = 7,
        /// <summary>
        /// 营养评估    
        /// </summary>
        [ChineseEnum("营养评估")]
        NutritionAssessmentRecord =8,
       
    }


}
