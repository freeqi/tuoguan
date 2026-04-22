using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 药品档案
    /// </summary>
    public class MedicalItemRecord : ISoftDelete
    {

        public string Id { get; set; }

        /// <summary>
        /// 是否销售免费   true = 售价可为0  
        /// </summary>
        public bool IsSellFree { get; set; }

        public string ParentId { get; set; }
        /// <summary>
        /// 物品类别ID（显示为类别目录名称）
        /// </summary>
        public string WareHouseIds { get; set; }
        [ForeignKey("WareHouseIds")]
        public virtual WarehouseCatalog WarehouseCatalog { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string WareHouseParentIds { get; set; }
        /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：
        /// 药品类型编码D开头，耗材类编码U开头（系统内唯一）
        /// 固定资产G 低值L  诊疗 T
        /// </summary>
        public string MedicalItemCode { get; set; }

        /// <summary>
        /// 医疗物品编码-人工编写 
        /// </summary>
        public string MedicalItemWorkCode { get; set; }

        /// <summary>
        /// 医保中心编码
        /// </summary>
        public string HiCenterCode { get; set; }


        public int? HiLevel { get; set; }

        public string HiCenterName { get; set; }
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }

        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName { get; set; }
        /// <summary>
        /// 医疗物品l类型
        /// 1药品类型，2耗材类型
        /// </summary>
        public int MedicalItemType { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 品牌/型号
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 助记码
        /// 自动生成
        /// </summary>
        public string Mnemonic { get; set; }
        /// <summary>
        /// 剂型
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public string Form { get; set; }
        [ForeignKey("Form")]
        public virtual DosageForm dosageForm { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 包装拆最小规格量	值100
        /// </summary>
        public int? SpecificationsQuantity { get; set; }
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string SpecificationsUnit { get; set; }
        [ForeignKey("SpecificationsUnit")]
        public virtual MedicalUnit SpecificationsUnits { get; set; }
        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }
        [ForeignKey("PackageUnit")]
        public virtual MedicalUnit PackageUnits { get; set; }
        /// <summary>
        /// 采购包装
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }
        [ForeignKey("ProcurementUnit")]
        public virtual MedicalUnit ProcurementUnits { get; set; }
        ///// <summary>
        ///// 计量单位
        ///// 来自字典数据 指门诊配药或病区摆药时所用单位，即最小包装单位，也是所有药品相关数量所用的单位，如“粒”，“片”，“包”，“支”，“丸”等；也称为发药单位、小单位 
        ///// </summary>
        //public int MeasurementUnit { get; set; }
        ///// <summary>
        /// 剂量单位
        /// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        /// </summary>
        public string DoseUnit { get; set; }
        [ForeignKey("DoseUnit")]
        public virtual MedicalUnit doseUnits { get; set; }
        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// <summary>
        ///采购拆分规则 --暂不使用，默认为1
        ///  
        /// </summary>
        public int? PackageSpecifications { get; set; }

        /// <summary>
        /// 是否可拆分
        /// </summary>
        public bool IsSplited { get; set; }

        /// <summary>
        /// 是否可退货
        /// </summary>
        public bool IsSalesReturn { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int? MonthDosage { get; set; }
        /// <summary>
        /// 自然年可用量
        /// </summary>
        public int? YearDosage { get; set; }
        /// <summary>
        /// 自然月人为损耗量
        /// </summary>
        public int? ArtificialLoss { get; set; }
        /// <summary>
        /// 适应症
        /// </summary>
        public string Iindications { get; set; }
        /// <summary>
        /// 用法用量
        /// </summary>
        public string Usage { get; set; }
        /// <summary>
        /// 存储条件
        /// </summary>
        public string StorageConditions { get; set; }

        /// <summary>
        /// 首批进货人员
        /// </summary>
        public string FirstStock { get; set; }
        ///// <summary>
        ///// 当前成本价
        ///// </summary>
        //public decimal CostPrice { get; set; }
        ///// <summary>
        ///// 当前销售价
        ///// </summary>
        //public decimal SalePrice { get; set; }
        /// <summary>
        /// 最小库存
        /// </summary>
        public int? MinInventory { get; set; }
        /// <summary>
        /// 费用类别 治疗费、检验费、检查费、其他
        /// </summary>
        public string FeeTypeId { get; set; }
        [ForeignKey("FeeTypeId")]
        public virtual SystemDictionary FeeType { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier supplier { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 可否采购
        /// </summary>
        public int ApplyState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }
        public bool IsDelete { get; set; }

        public virtual List<MedicalDrugExtension> MedicalDrugExtensions { get; set; } = new List<MedicalDrugExtension>();

        public virtual SI_CheckPriceInfo SI_CheckPriceInfoS { get; set; }

        /// <summary>
        /// 批准文号 2020年10月14日11:14:59加
        /// </summary>
        public string ApprovalNum { get; set; }

        /// <summary>
        /// 2021年1月12日09:29:18 添加  是否为管路
        /// </summary>
        public bool? IsMedCanal { get; set; }

        /// <summary>
        /// 2021年2月3日09:36:03 0/null:无 1:低通 2:高通 3:血滤 
        /// </summary>
        public int? DialyzerType { get; set; }

        /// <summary>
        /// 国家目录代码
        /// </summary>
        public string NationItemCode { get; set; }

        /// <summary>
        /// 优先出库供应商
        /// </summary>
        public string PrioritySupplier { get; set; }
        /// <summary>
        /// 是否为中心端自己添加
        /// </summary>
        public int? IsCenterAdd { get; set; }
        public string CenterId { get; set; }

        /// <summary>
        /// 普通耗材可否划价
        /// </summary>
        public int? MayCharges { get; set; }
        ///// <summary>
        ///// 是否为透析液
        ///// </summary>
        //public int? IsMedDialysate { get; set; }
        /// <summary>
        /// 浓度
        /// </summary>
        public string Concentration { get; set; }
    }

    /// <summary>
    /// 档案关联表
    /// </summary>
    public class MedicalRelevancy
    {
        public string Id { get; set; }
        public string MainMedId { get; set; }
        [ForeignKey("MainMedId")]
        public virtual MedicalItemRecord MainMed { get; set; }
        public string ChildMedId { get; set; }
        [ForeignKey("ChildMedId")]
        public virtual MedicalItemRecord ChildMed { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }

    }


    /// <summary>
    /// 档案对码审核中间表
    /// </summary>
    public class MedMatchCode
    {
        public string Id { get; set; }
        /// <summary>
        /// 新医保码
        /// </summary>
        public string NewNationItemCode { get; set; }
        /// <summary>
        /// 原医保码
        /// </summary>
        public string OldNationItemCode { get; set; }
        /// <summary>
        /// 档案ID
        /// </summary>
        public string MedId { get; set; }
        /// <summary>
        /// 档案类型 1药品 2耗材 5 服务项目
        /// </summary>
        public int MedType { get; set; }
        /// <summary>
        /// 审核状态 0 未审核 1 审核通过 2 未通过
        /// </summary>
        public int AuditState { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }

    }


    /// <summary>
    /// 档案医保月核对
    /// </summary>

    public class MedMonthAudit
    {

        public string Id { get; set; }
        /// <summary>
        /// 医疗物品ID
        /// </summary>
        public string MedicalId { get; set; }
        /// <summary>
        /// 国家码
        /// </summary>
        public string NationItemCode { get; set; }
        /// <summary>
        /// 添加人 审核人
        /// </summary>
        public string Founder { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        /// <summary>
        /// 数据状态    1 正常 2 无效 3 删除
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 审核状态 0 未审核 1 审核通过 2 未通过  -1 医保目录
        /// </summary>
        public int AuditState { get; set; }

        /// <summary>
        /// 备注、核对意见
        /// </summary>
        public string Remark { get; set; }

    }

}
