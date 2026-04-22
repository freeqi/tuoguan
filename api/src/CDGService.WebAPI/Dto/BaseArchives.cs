using CDGService.Data.Datas;
using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    #region Input
    /// <summary>
    /// 库房目录
    /// </summary>
    public class WarehouseCatalogInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
    }

    /// <summary>
    /// 供应商 
    /// </summary>
    public class SupplierInPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string SupCode { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 主营业务
        /// </summary>
        public string MainBusiness { get; set; }
        /// <summary>
        /// 主营类别
        /// </summary>
        public string[] MainCategories { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Image1 { get; set; }

        public string Image2 { get; set; }

    }

    public class SupplierSearchInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public int[] MainCategories { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }

    }


    public class MedicalItemRecordSearchInput
    {
        public string Id { get; set; }
        public int MedicalItemType { get; set; }
        public string CenterId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 库房目录ID
        /// </summary>
        public string WareHouseId { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 品牌/型号
        /// </summary>

        public string Brand { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 价格异常
        /// </summary>
        public bool IsAbnormal { get; set; } = false;

        public string nationItemCode { get; set; }

        public int DataState { get; set; } = 1;


    }

    public class MedicalItemRecordInput
    {

        public bool IsSellFree { get; set; } = false;
        /// <summary>
        /// 是否可划价
        /// </summary>
        public int? MayCharges { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public MedicalItemTypeEnum medicalItemTypeEnum { get; set; }

        /// <summary>
        /// 物品类别ID（显示为类别目录名称）
        /// </summary>
        public string WareHouseId { get; set; }
        /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：药品类型编码D开头，耗材类编码U开头（系统内唯一）
        /// </summary>
        public string MedicalItemCode { get; set; }
        /// <summary>
        /// 医疗物品编码-人工编写 
        /// </summary>
        public string MedicalItemWorkCode { get; set; }

        /// <summary>
        /// 医保中心编码
        /// </summary>
       // public string HiCenterCode { get; set; }
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }
        /// <summary>
        /// 批准文号 2020年10月14日11:14:59加
        /// </summary>
        public string ApprovalNum { get; set; }
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
      //  public int MedicalItemType { get; set; }
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
        public object Forms { get; set; }

        /// <summary>
        /// 剂量单位
        /// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        /// </summary>
        public string DoseUnit { get; set; }
        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string MinDose { get; set; }
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 包装拆最小规格量	值100
        /// </summary>
        public int? SpecificationsQuantity { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }

        /// <summary>
        /// 采购包装
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }
        ///// <summary>
        ///// 计量单位
        ///// 来自字典数据 指门诊配药或病区摆药时所用单位，即最小包装单位，也是所有药品相关数量所用的单位，如“粒”，“片”，“包”，“支”，“丸”等；也称为发药单位、小单位 
        ///// </summary>
        //public int MeasurementUnit { get; set; }
        ///// <summary>
        ///// 剂量单位
        ///// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        ///// </summary>
        //public int DoseUnit { get; set; }
        /// <summary>
        ///拆分规则
        /// 包装规格（计价规格）：是计价单位和计量单位之间换算的关系值。如100
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
        /// 自然月可用量
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
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }


        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 可否采购
        /// </summary>
        public int? ApplyState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public MedicalDrugExtensionInput MedicalDrugExtension { get; set; }
        /// <summary>
        /// 2021年1月12日09:29:18 添加  是否为管路
        /// </summary>
        public bool? IsMedCanal { get; set; }
        public int? DialyzerType { get; set; }

        /// <summary>
        /// 优先出库供应商
        /// </summary>
        public string PrioritySupplier { get; set; }

        /// <summary>
        /// 浓度
        /// </summary>
        public string Concentration { get; set; }

    }


    public class MedicalDrugExtensionInput
    {


        public string Id { get; set; }
        /// <summary>
        /// 物品档案id
        /// </summary>
        public string MedicalId { get; set; }
        /// <summary>
        /// 透析中心ID
        /// </summary>
        public string CenterId { get; set; }

        /// <summary>
        /// 采购价
        /// </summary>
        public decimal PurchasingPrice { get; set; }

        /// <summary>
        /// 协议价
        /// 
        /// </summary>
        public decimal AgreementPrice { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal RetailPrice { get; set; }
        /// <summary>
        /// 参考价
        /// 
        /// </summary>
        public decimal ReferencePrice { get; set; }

        /// <summary>
        /// 政府指导价
        /// </summary>
        public decimal GocGuidePrice { get; set; }

        /// <summary>
        /// 社保指导价
        /// </summary>
        public decimal SocialSecurityPrice { get; set; }
        /// <summary>
        /// 是否为当前使用
        /// </summary>
        public bool IsCurrentUse { get; set; }
        /// <summary>
        /// 1 启用2禁用
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }
        public decimal? Coefficient { get; set; }

    }

    public class DosageFormInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 剂型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }


    /// <summary>
    /// 物品基础单位
    /// </summary>
    public class MedicalUnitInput
    {
        public string Id { get; set; }

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
        public string Remark { get; set; }


    }

    public class UnitQueryInput
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public UnitEnum unitType { get; set; }

        public int DataState { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }

    }
    public class UnitActiveInPut
    {
        /// <summary>
        /// ID集合
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  是否启用 2禁用 1 启用
        /// </summary>
        public int IsActive { get; set; }

    }




    public class UseWayInput
    {
        public string Id { get; set; }
        /// <summary>
        /// 代码值
        /// </summary>
        public string WayCode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string WayDescribe { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WayEnum WayType { get; set; }

    }

    public class UseWayQueryInput
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public WayEnum WayType { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }

    }

    /// <summary>
    /// 医保对照条件
    /// </summary>
    public class SIContrast
    {
        public Si_YpmlQueryInput si_YpmlQueryInput { get; set; }

        public Si_YpmlQueryInput MedicalItemInput { get; set; }
    }

    /// <summary>
    /// 医保对照查询条件
    /// </summary>
    public class Si_YpmlQueryInput
    {
        /// <summary>
        /// 药品名称
        /// </summary>
        public string YpName { get; set; }
        public string medicalItemName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>

        public string Brand { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Packaging { get; set; }

        public int level { get; set; }

        /// <summary>
        /// 1 药品 2 其它
        /// </summary>
        public int ItemType { get; set; }
        /// <summary>
        /// 是否只筛选未对照
        /// </summary>
        public bool? isContrast { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }

        public string gjypdm { get; set; }
        public string nationItemCode { get; set; }
        public string gjxmdm { get; set; }

    }

    public class Si_ZLXMQueryInput
    {
        public string BDItemName { get; set; }
        public string ZXItemName { get; set; }

        public string Manufacturer { get; set; }
        public string bz { get; set; }


        public string gjxmdm { get; set; }
        public string nationItemCode { get; set; }
    }

    #endregion

    #region OutPut


    public class SysRegionOutput
    {
        public string Id { get; set; }

        public string RegionName { get; set; }
        public string title { get { return RegionName; } set { } }
        /// <summary>
        /// 地区行政编码
        /// </summary>
        public string RegionCode { get; set; }

        public string RegionParentId { get; set; }
        public List<SysRegionOutput> children { get; set; } = new List<SysRegionOutput>();
    }




    public class WarehouseCatalogOutPut
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string title { get { return Name; } set { } }
        public bool expand { get; set; } = false;
        public string Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int? CataIndex { get; set; }
        public List<WarehouseCatalogOutPut> children { get; set; } = new List<WarehouseCatalogOutPut>();

    }
    public class CenterWarehouseCatalogOutPut
    {


        public string Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }


    /// <summary>
    /// 供应商 
    /// </summary>
    public class SupplierOutPut
    {
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string SupCode { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 主营业务
        /// </summary>
        public string MainBusiness { get; set; }
        /// <summary>
        /// 主营类别
        /// </summary>
        public string MainCategories { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public string Image1 { get; set; }

        public string Image2 { get; set; }

    }

    /// <summary>
    /// 药品档案
    /// </summary>
    public class MedicalItemRecordOutPut
    {
        public string NationItemCode { get; set; }

        public bool IsSellFree { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        /// <summary>
        /// 物品类别ID（显示为类别目录名称）
        /// </summary>
        public List<string> WareHouseId { get; set; } = new List<string>();
        /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：药品类型编码D开头，耗材类编码U开头（系统内唯一）
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
        /// <summary>
        /// 等级
        /// </summary>
        public int? HiLevel { get; set; }
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }

        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }
        /// <summary>
        /// 批准文号 2020年10月14日11:14:59加
        /// </summary>
        public string ApprovalNum { get; set; }
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
        /// 剂型[]
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public List<string> Forms { get; set; } = new List<string>();
        /// <summary>
        /// 剂型
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public string Form { get; set; }
        /// <summary>
        /// 中文剂型
        /// </summary>
        public string FormValue { get; set; }
        ///// <summary>
        /// 剂量单位
        /// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        /// </summary>
        public string DoseUnit { get; set; }
        /// <summary>
        /// 中文剂量单位
        /// </summary>
        public string DoseUnitValue { get; set; }

        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// 规格
        /// </summary>
        public string MinDose { get; set; }
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string Specifications { get; set; }
        public string SpecificationsValue { get; set; }


        /// <summary>
        /// 包装拆最小规格量	值100
        /// </summary>
        public int? SpecificationsQuantity { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }
        public string PackageUnitValue { get; set; }
        /// <summary>
        /// 采购包装
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }

        public string ProcurementUnitValue { get; set; }
        ///// <summary>
        ///// 计量单位
        ///// 来自字典数据 指门诊配药或病区摆药时所用单位，即最小包装单位，也是所有药品相关数量所用的单位，如“粒”，“片”，“包”，“支”，“丸”等；也称为发药单位、小单位 
        ///// </summary>
        //public int MeasurementUnit { get; set; }
        ///// <summary>
        ///// 剂量单位
        ///// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        ///// </summary>
        //public int DoseUnit { get; set; }
        /// <summary>
        ///拆分规则
        /// 包装规格（计价规格）：是计价单位和计量单位之间换算的关系值。如100
        /// </summary>
        public int? PackageSpecifications { get; set; }

        /// <summary>
        /// 是否可拆分
        /// </summary>
        public int IsSplited { get; set; }

        /// <summary>
        /// 是否可退货
        /// </summary>
        public int IsSalesReturn { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int MonthDosage { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int? YearDosage { get; set; }
        /// <summary>
        /// 自然月人为损耗量
        /// </summary>
        public int ArtificialLoss { get; set; }
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
        public int MinInventory { get; set; }
        /// <summary>
        /// 费用类别 治疗费、检验费、检查费、其他
        /// </summary>
        public string FeeTypeId { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }

        public string supplierName { get; set; }
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
        /// <summary>
        /// 2021年1月12日09:29:18 添加  是否为管路
        /// </summary>
        public int IsMedCanal { get; set; }

        /// <summary>
        /// 2021年2月3日09:36:03 0/null:无 1:低通 2:高通 3:血滤 
        /// </summary>
        public int? DialyzerType { get; set; }
        /// <summary>
        /// 历史价格
        /// </summary>
        public MedicalDrugExtensionOutput MedicalDrugExtension { get; set; }


        public string chrgitm_lv { get; set; }
        public decimal? hilist_pric_uplmt_amt { get; set; }
        public decimal? PurchasingPrice { get; set; }

        /// <summary>
        /// 优先出库供应商
        /// </summary>
        public string PrioritySupplier { get; set; }

        public int? MayCharges { get; set; }
        /// <summary>
        /// 浓度
        /// </summary>
        public string Concentration { get; set; }


        /// <summary>
        /// 最近使用
        /// </summary>
        public int? MedFrequency { get; set; }
 
        /// <summary>
        /// 库存
        /// </summary>
        public decimal? MedInventory { get; set; }
    }

    /// <summary>
    /// 药品档案
    /// </summary>
    public class YBMedicalItemRecordOutPut
    {
        public string NationItemCode { get; set; }

        public string Id { get; set; }
        public string ParentId { get; set; }
        /// <summary>
        /// 物品类别ID（显示为类别目录名称）
        /// </summary>
        public List<string> WareHouseId { get; set; } = new List<string>();
        /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：药品类型编码D开头，耗材类编码U开头（系统内唯一）
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
        /// <summary>
        /// 等级
        /// </summary>
        public int? HiLevel { get; set; }
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }

        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }
        /// <summary>
        /// 批准文号 2020年10月14日11:14:59加
        /// </summary>
        public string ApprovalNum { get; set; }
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
        /// 剂型[]
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public List<string> Forms { get; set; } = new List<string>();
        /// <summary>
        /// 剂型
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public string Form { get; set; }
        /// <summary>
        /// 中文剂型
        /// </summary>
        public string FormValue { get; set; }
        ///// <summary>
        /// 剂量单位
        /// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        /// </summary>
        public string DoseUnit { get; set; }
        /// <summary>
        /// 中文剂量单位
        /// </summary>
        public string DoseUnitValue { get; set; }

        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// 规格
        /// </summary>
        public string MinDose { get; set; }
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string Specifications { get; set; }
        public string SpecificationsValue { get; set; }


        /// <summary>
        /// 包装拆最小规格量	值100
        /// </summary>
        public int? SpecificationsQuantity { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }
        public string PackageUnitValue { get; set; }
        /// <summary>
        /// 采购包装
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }

        public string ProcurementUnitValue { get; set; }
        ///// <summary>
        ///// 计量单位
        ///// 来自字典数据 指门诊配药或病区摆药时所用单位，即最小包装单位，也是所有药品相关数量所用的单位，如“粒”，“片”，“包”，“支”，“丸”等；也称为发药单位、小单位 
        ///// </summary>
        //public int MeasurementUnit { get; set; }
        ///// <summary>
        ///// 剂量单位
        ///// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        ///// </summary>
        //public int DoseUnit { get; set; }
        /// <summary>
        ///拆分规则
        /// 包装规格（计价规格）：是计价单位和计量单位之间换算的关系值。如100
        /// </summary>
        public int? PackageSpecifications { get; set; }

        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int MonthDosage { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int? YearDosage { get; set; }
        /// <summary>
        /// 自然月人为损耗量
        /// </summary>
        public int ArtificialLoss { get; set; }
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
        public int MinInventory { get; set; }
        /// <summary>
        /// 费用类别 治疗费、检验费、检查费、其他
        /// </summary>
        public string FeeTypeId { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }

        public string supplierName { get; set; }
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
        /// <summary>
        /// 2021年1月12日09:29:18 添加  是否为管路
        /// </summary>
        public bool? IsMedCanal { get; set; }

        /// <summary>
        /// 2021年2月3日09:36:03 0/null:无 1:低通 2:高通 3:血滤 
        /// </summary>
        public int? DialyzerType { get; set; }


        public string chrgitm_lv { get; set; }
        public decimal? hilist_pric_uplmt_amt { get; set; }
        public decimal? PurchasingPrice { get; set; }

    }
    /// <summary>
    /// 药品档案
    /// </summary>
    public class CenterMedicalItemRecordOutPut
    {

        public string Id { get; set; }
        /// <summary>
        ///类别ID
        /// </summary>
        public string WareHouseIds { get; set; }
        /// <summary>
        /// 物品类别ID包括父ID（显示为类别目录名称）
        /// </summary>
        public string WareHouseParentIds { get; set; }

        /// <summary>
        /// 医疗物品编码
        /// 系统自动生成：药品类型编码D开头，耗材类编码U开头（系统内唯一）
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
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }
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
        /// 剂型[] 包括父ID
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public List<string> Forms { get; set; } = new List<string>();
        /// <summary>
        /// 剂型
        /// 来自于字典数据  针剂、片剂等  
        /// </summary>
        public string Form { get; set; }
        ///// <summary>
        /// 剂量单位
        /// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        /// </summary>
        public string DoseUnit { get; set; }
        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// 规格
        /// </summary>
        public string MinDose { get; set; }
        /// <summary>
        /// 规格单位
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string Specifications { get; set; }

        /// <summary>
        /// 包装拆最小规格量	值100
        /// </summary>
        public int? SpecificationsQuantity { get; set; }

        /// <summary>
        /// 包装规格
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }

        /// <summary>
        /// 采购包装
        /// </summary>
        public string ProcurementPackage { get; set; }
        /// <summary>
        /// 采购单位
        /// </summary>
        public string ProcurementUnit { get; set; }
        ///// <summary>
        ///// 计量单位
        ///// 来自字典数据 指门诊配药或病区摆药时所用单位，即最小包装单位，也是所有药品相关数量所用的单位，如“粒”，“片”，“包”，“支”，“丸”等；也称为发药单位、小单位 
        ///// </summary>
        //public int MeasurementUnit { get; set; }
        ///// <summary>
        ///// 剂量单位
        ///// 来自于字典数据 指医生开处方时所用的最小单位，一般为“ml”，“mg”，“g” 
        ///// </summary>
        //public int DoseUnit { get; set; }
        /// <summary>
        ///拆分规则
        /// 包装规格（计价规格）：是计价单位和计量单位之间换算的关系值。如100
        /// </summary>
        public int? PackageSpecifications { get; set; }

        /// <summary>
        /// 是否可拆分
        /// </summary>
        public int IsSplited { get; set; }

        /// <summary>
        /// 是否可退货
        /// </summary>
        public int IsSalesReturn { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int MonthDosage { get; set; }
        /// <summary>
        /// 自然月可用量
        /// </summary>
        public int? YearDosage { get; set; }
        /// <summary>
        /// 自然月人为损耗量
        /// </summary>
        public int ArtificialLoss { get; set; }
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
        public int MinInventory { get; set; }
        /// <summary>
        /// 费用类别 治疗费、检验费、检查费、其他
        /// </summary>
        public string FeeTypeId { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId { get; set; }

        public string supplierName { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 可否采购
        /// </summary>
        public int ApplyState { get; set; }
        /// <summary>
        /// 历史价格
        /// </summary>
        public MedicalDrugExtensionOutput MedicalDrugExtension { get; set; }
    }

    /// <summary>
    /// 档案导出EXCEL模型
    /// </summary>
    public class ExportMedicalItem
    {
        /// <summary>
        /// 类别（口服、针剂药）
        /// </summary>
        public string CatalogueItem { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string MedicalItemCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 剂型
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// <summary>
        /// 中文单位
        /// </summary>
        public string SpeUnitCHS { get; set; }

        /// <summary>
        /// 英文单位
        /// </summary>
        public string SpeUnitUS { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackSpeUnitCHS { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 规格单位
        /// </summary>
        public string SpecSpeUnitCHS { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 件比
        /// </summary>
        public string MedicalThan { get; set; }

    }


    public class MedicalDrugExtensionOutput
    {
        public string Id { get; set; }

        public string MedicalId { get; set; }

        public string medicalItemRecordName { get; set; }
        public string CenterId { get; set; }
        public string CenterName { get; set; }

        /// <summary>
        /// 采购价
        /// </summary>
        public decimal PurchasingPrice { get; set; } = -1;

        /// <summary>
        /// 协议价
        /// 
        /// </summary>
        public decimal AgreementPrice { get; set; } = -1;
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal RetailPrice { get; set; } = -1;
        /// <summary>
        /// 参考价
        /// 
        /// </summary>
        public decimal ReferencePrice { get; set; } = -1;

        /// <summary>
        /// 政府指导价
        /// </summary>
        public decimal GocGuidePrice { get; set; } = -1;

        /// <summary>
        /// 社保指导价
        /// </summary>
        public decimal SocialSecurityPrice { get; set; } = -1;
        /// <summary>
        /// 是否为当前使用
        /// </summary>
        public int IsCurrentUse { get; set; }
        public int DataState { get; set; }
        /// <summary>
        /// 建档原因及目的
        /// </summary>
        public string Remark { get; set; }

        public DateTime FounderDate { get; set; }

        public decimal? coefficient { get; set; }

    }


    public class DosageFormOutput
    {
        public string Id { get; set; }
        /// <summary>
        /// 剂型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 剂型名称
        /// </summary>
        public string title { get { return TypeName; } set { } }
        public bool expand { get; set; } = false;
        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public List<DosageFormOutput> children { get; set; } = new List<DosageFormOutput>();


    }
    /// <summary>
    /// 物品基础单位
    /// </summary>
    public class MedicalUnitOutput
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
        public int DataState { get; set; }
        public string Remark { get; set; }
        // public string FounderName { get; set; }


    }

    public class UseWayOutput
    {
        public string Id { get; set; }
        /// <summary>
        /// 代码值
        /// </summary>
        public string WayCode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string WayDescribe { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WayEnum WayType { get; set; }
        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }

    }

    public class DosageFormCenterOutput
    {
        public string Id { get; set; }
        /// <summary>
        /// 剂型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public int DataState { get; set; }



    }


    public class yibao
    {
        public MedicalItemRecordOutPut[] medical { get; set; }
        public SI_YPMLS[] SIypmls { get; set; }
    }

    public class Newyibao
    {
        public MedicalItemOutPut[] medical { get; set; }
        public Si_YPOutPut[] SIypmls { get; set; }

    }

    public class ZLXM
    {
        public MedicalItemRecordOutPut[] medical { get; set; }

        public SI_ZLXMS[] sI_ZLXMS { get; set; }
    }

    public class HC_OutPut
    {
        public SI_HCOutPut[] sI_HCOutPuts { get; set; }
        public MedicalItemOutPut[] medical { get; set; }

    }
    public class HC_FWXM
    {
        public SI_FWXMOutPut[] sI_HCOutPuts { get; set; }
        public MedicalItemOutPut[] medical { get; set; }

    }

    #endregion

    public class yibaoNew
    {
        public MedicalItemOutPut[] medical { get; set; }
        public Si_YPOutPut[] SIypmls { get; set; }
    }


    public class MedicalItemOutPut
    {
        public string NationItemCode { get; set; }

        public string Id { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int? HiLevel { get; set; }
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 批准文号 2020年10月14日11:14:59加
        /// </summary>
        public string ApprovalNum { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 医疗物品l类型
        /// 1药品类型，2耗材类型
        /// </summary>
        public int MedicalItemType { get; set; }

        /// <summary>
        /// 品牌/型号
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 中文剂型
        /// </summary>
        public string FormValue { get; set; }

        /// <summary>
        /// 中文剂量单位
        /// </summary>
        public string DoseUnitValue { get; set; }

        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }

        /// <summary>
        /// 规格 
        /// 最小剂量+剂量单位*包装规格+计量单位/包装单位（显示为2g*100片/盒，规格自动生成，可手工调整）
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 规格 同Specifications
        /// </summary>
        public string MinDose { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string PackageUnit { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int DataState { get; set; }

        /// <summary>
        /// 医保等级
        /// </summary>
        public string chrgitm_lv { get; set; }
        /// <summary>
        /// 医保中心价
        /// </summary>
        public decimal? hilist_pric_uplmt_amt { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? PurchasingPrice { get; set; }



    }



    public class YBDZMedicalItem : MedMatchCode
    {
        /// <summary>
        /// 医疗物品通用名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 品牌/型号
        /// </summary>
        public string Brand { get; set; }

        public string dosageFormName { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specifications { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 最小剂量
        /// </summary>
        public decimal? DoseMin { get; set; }
        /// <summary>
        /// 剂量单位
        /// </summary>
        public string DoseUnitName { get; set; }
        /// <summary>
        /// 销售价
        /// </summary>
        public decimal? RetailPrice { get; set; }
        /// <summary>
        /// 医保中心价
        /// </summary>
        public decimal? SocialSecurityPrice { get; set; }
        /// <summary>
        /// 国家目录代码
        /// </summary>
        public string NationItemCode { get; set; }
        /// <summary>
        /// 医保等级
        /// </summary>
        public string HiLevel { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 医保中心项目
        /// </summary>
        public string tym { get; set; }
        /// <summary>
        /// 医保-商品名称
        /// </summary>
        public string spm { get; set; }
        /// <summary>
        /// 医保-限价
        /// </summary>
        public decimal? ylbzdj { get; set; }
        /// <summary>
        /// 医保-含量
        /// </summary>
        public string hl { get; set; }
        /// <summary>
        /// 医保-包装数量
        /// </summary>
        public string bzsl { get; set; }
        /// <summary>
        /// 医保包装单位
        /// </summary>
        public string bzdw { get; set; }
        /// <summary>
        /// 医保-最小制剂单位
        /// </summary>
        public string zxzjdw { get; set; }
        /// <summary>
        /// 医保-厂家
        /// </summary>
        public string ycmc { get; set; }
        /// <summary>
        /// 医保-国家码
        /// </summary>
        public string gjypdm { get; set; }
        /// <summary>
        /// 医保-等级
        /// </summary>
        public string ylfydj { get; set; }

        public decimal? purchasingPrice { get; set; }


    }

    public class SiTitleData
    {
        public string tym { get; set; }
        public string spm { get; set; }
        public string ylbzdj { get; set; }

        public string hl { get; set; }
        public string bzsl { get; set; }

        public string bzdw { get; set; }
        public string zxzjdw { get; set; }
        public string ycmc { get; set; }

        public string gjypdm { get; set; }

        public string ylfydj { get; set; }
    }





    /// <summary>
    /// 医保药品输出
    /// </summary>
    public class Si_YPOutPut
    {
        /*
         ID	tym	spm	YPTYM	jx	bzsl	bzdw	ZXZJDW	hl	ycmc	gjypdm	ylzbj	ylzfbl	ylfydj	lmttype	protype	ZHB	XZSYBZ	XZSYFW	PZWH
129016	肝素钠注射液	无	肝素	注射剂(注射液)	10	盒	支	2ml:1000单位	成都市海通药业有限公司	XB01ABG047B002050102068	NULL	NULL	NULL	NULL	NULL	0	null	null	国药准字H51021210
             */
        public int id { get; set; }
        /// <summary>
        /// 药品名
        /// </summary>
        public string tym { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string spm { get; set; }
        /// <summary>
        /// 通用名
        /// </summary>
        public string YPTYM { get; set; }
        /// <summary>
        /// 剂型
        /// </summary>
        public string jx { get; set; }
        /// <summary>
        /// 最小包装数量
        /// </summary>
        public string bzsl { get; set; }
        /// <summary>
        /// 包装单位
        /// </summary>
        public string bzdw { get; set; }
        /// <summary>
        /// 最小制剂单位名称
        /// </summary>
        public string ZXZJDW { get; set; }
        /// <summary>
        /// 含量
        /// </summary>
        public string hl { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string ycmc { get; set; }
        /// <summary>
        /// 国家编码
        /// </summary>
        public string gjypdm { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal? ylbzdj { get; set; }
        /// <summary>
        /// 自费比例
        /// </summary>
        public decimal? ylzfbl { get; set; }
        /// <summary>
        /// 目录等级
        /// </summary>
        public string ylfydj { get; set; }
        /// <summary>
        /// 医院等级
        /// </summary>
        public string lmttype { get; set; }
        public string protype { get; set; }
        public string ZHB { get; set; }
        public string XZSYBZ { get; set; }
        public string XZSYFW { get; set; }
        public string BZCZ { get; set; }
        public string BZCZMC { get; set; }
        /// <summary>
        /// 批准文号
        /// </summary>
        public string PZWH { get; set; }

        public string bgsj { get; set; }
        public string bz { get; set; }
    }
    /// <summary>
    /// 医保耗材输出
    /// </summary>
    public class SI_HCOutPut
    {
        public int Id { get; set; }
        /// <summary>
        /// 支付比例
        /// </summary>
        public decimal? txbl { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal? ylzdj { get; set; }
        /// <summary>
        /// 收费项目等级 
        /// </summary>
        public string ylfydj { get; set; }
        /// <summary>
        /// 医院等级
        /// </summary>
        public string lmttype { get; set; }
        public string xmmc { get; set; }

        public string gjxmdm { get; set; }
        public string gjxmfl { get; set; }
        public string gg { get; set; }
        public string GGXH { get; set; }
        public string bz { get; set; }
        public string bgsj { get; set; }

        public string hcfl { get; set; }

        public string memo { get; set; }
    }
    /// <summary>
    /// 医保诊疗项目输出
    /// </summary>
    public class SI_FWXMOutPut
    {
        public int Id { get; set; }
        /// <summary>
        /// 支付比例
        /// </summary>
        public decimal? txbl { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal? ylbzj { get; set; }
        /// <summary>
        /// 收费项目等级 
        /// </summary>
        public string ylfydj { get; set; }
        /// <summary>
        /// 计价单位
        /// </summary>
        public string jjdw { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string xmmc { get; set; }
        /// <summary>
        /// 国家项目代码
        /// </summary>
        public string gjxmdm { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string bz { get; set; }
        /// <summary>
        /// 医院等级
        /// </summary>
        public string lmttype { get; set; }

        /// <summary>
        /// 诊疗项目内涵
        /// </summary>
        public string zlxmhl { get; set; }
        public string bgsj { get; set; }
    }





    public class MRelevancyOutPut
    {
        public string Id { get; set; }
        public string MainMedId { get; set; }

        public string ChildMedId { get; set; }

        public int DataState { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }

        public RelevancyMedModel MainMedModel { get; set; }

        public List<RelevancyMedModel> ChildMedModel { get; set; }
    }


    public class RelevancyMedModel
    {
        public string Id { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string MedicalItemCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string MedicalItemName { get; set; }
        public string MedicalItemId { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
    }



    /// <summary>
    /// 医保月核对目录信息
    /// </summary>
    public class MedHisCatalogOutPut
    {
        //MedCatalog,MedicalItemName,MedicalItemCode,Packaging,MedUnit,ApprovalNum,Manufacturer,NationItemCode,PurchasingPrice,RetailPrice,ylfydj

        /// <summary>
        /// 目录
        /// </summary>
        public string MedCatalog { get; set; }

        public string MedicalId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string MedicalItemName { get; set; }

        /// <summary>
        /// 审核状态 NULL 未审核 1 审核通过 2 未通过
        /// </summary>
        public int? AuditState { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string MedicalItemCode { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string MedUnit { get; set; }
        /// <summary>
        /// 批准文号
        /// </summary>
        public string ApprovalNum { get; set; }
        public string Manufacturer { get; set; }
        public string NationItemCode { get; set; }
        /// <summary>
        /// 采购价
        /// </summary>
        public string PurchasingPrice { get; set; }
        /// <summary>
        /// 销售价/限价
        /// </summary>
        public string RetailPrice { get; set; }
        /// <summary>
        /// 目录等级
        /// </summary>
        public int? ylfydj { get; set; }



    }




    public class MedFrequencyOutPut
    {
        public string Id { get; set; }
        public string MedicalItemId { get; set; }

        public string MedicalItemName { get; set; }

        public DateTime? FounderDate { get; set; }

    }


}
