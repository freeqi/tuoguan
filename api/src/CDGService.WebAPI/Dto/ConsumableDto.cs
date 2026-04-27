using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    /// <summary>
    /// 耗材输入DTO
    /// </summary>
    public class ConsumableInput
    {
        public string Id { get; set; }
        public string MedicalItemName { get; set; }
        public string MedicalItemCode { get; set; }
        public string MedicalItemWorkCode { get; set; }
        public string GoodsName { get; set; }
        public string AliasName { get; set; }
        public int MedicalItemType { get; set; }
        public string EnglishName { get; set; }
        public string Brand { get; set; }
        public string Form { get; set; }
        public string Specifications { get; set; }
        public int? SpecificationsQuantity { get; set; }
        public string SpecificationsUnit { get; set; }
        public string Packaging { get; set; }
        public string PackageUnit { get; set; }
        public string ProcurementPackage { get; set; }
        public string ProcurementUnit { get; set; }
        public string DoseUnit { get; set; }
        public decimal? DoseMin { get; set; }
        public int? PackageSpecifications { get; set; }
        public bool IsSplited { get; set; }
        public bool IsSalesReturn { get; set; }
        public int? MonthDosage { get; set; }
        public int? YearDosage { get; set; }
        public int? ArtificialLoss { get; set; }
        public string Iindications { get; set; }
        public string Usage { get; set; }
        public string StorageConditions { get; set; }
        public string FirstStock { get; set; }
        public int? MinInventory { get; set; }
        public string FeeTypeId { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierId { get; set; }
        public int DataState { get; set; }
        public int ApplyState { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public string Modifier { get; set; }
        public string CenterId { get; set; }
    }

    /// <summary>
    /// 耗材输出DTO
    /// </summary>
    public class ConsumableOutput
    {
        public string Id { get; set; }
        public string MedicalItemName { get; set; }
        public string MedicalItemCode { get; set; }
        public string MedicalItemWorkCode { get; set; }
        public string GoodsName { get; set; }
        public string AliasName { get; set; }
        public int MedicalItemType { get; set; }
        public string MedicalItemTypeText { get; set; }
        public string EnglishName { get; set; }
        public string Brand { get; set; }
        public string Form { get; set; }
        public string Specifications { get; set; }
        public int? SpecificationsQuantity { get; set; }
        public string SpecificationsUnit { get; set; }
        public string Packaging { get; set; }
        public string PackageUnit { get; set; }
        public string ProcurementPackage { get; set; }
        public string ProcurementUnit { get; set; }
        public string DoseUnit { get; set; }
        public decimal? DoseMin { get; set; }
        public int? PackageSpecifications { get; set; }
        public bool IsSplited { get; set; }
        public bool IsSalesReturn { get; set; }
        public int? MonthDosage { get; set; }
        public int? YearDosage { get; set; }
        public int? ArtificialLoss { get; set; }
        public string Iindications { get; set; }
        public string Usage { get; set; }
        public string StorageConditions { get; set; }
        public string FirstStock { get; set; }
        public int? MinInventory { get; set; }
        public string FeeTypeId { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int DataState { get; set; }
        public string DataStateText { get; set; }
        public int ApplyState { get; set; }
        public string ApplyStateText { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifierDate { get; set; }
        public string CenterId { get; set; }
    }
}
