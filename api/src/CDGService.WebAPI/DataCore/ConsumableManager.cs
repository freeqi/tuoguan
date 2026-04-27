using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.DataCore
{
    public class ConsumableManager
    {
        private readonly IRepository<MedicalItemRecord> _medicalItemRecordRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConsumableManager(
            IRepository<MedicalItemRecord> medicalItemRecordRepository,
            IRepository<Supplier> supplierRepository,
            IUnitOfWork unitOfWork)
        {
            _medicalItemRecordRepository = medicalItemRecordRepository;
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取耗材列表
        /// </summary>
        public async Task<List<ConsumableOutput>> GetConsumablesAsync()
        {
            var consumables = await _medicalItemRecordRepository.GetAllAsync(m => m.MedicalItemType == 2); // 2: 耗材类型
            var result = new List<ConsumableOutput>();

            foreach (var consumable in consumables)
            {
                var supplier = await _supplierRepository.GetByIdAsync(consumable.SupplierId);
                result.Add(new ConsumableOutput
                {
                    Id = consumable.Id,
                    MedicalItemName = consumable.MedicalItemName,
                    MedicalItemCode = consumable.MedicalItemCode,
                    MedicalItemWorkCode = consumable.MedicalItemWorkCode,
                    GoodsName = consumable.GoodsName,
                    AliasName = consumable.AliasName,
                    MedicalItemType = consumable.MedicalItemType,
                    MedicalItemTypeText = consumable.MedicalItemType == 1 ? "药品" : "耗材",
                    EnglishName = consumable.EnglishName,
                    Brand = consumable.Brand,
                    Form = consumable.Form,
                    Specifications = consumable.Specifications,
                    SpecificationsQuantity = consumable.SpecificationsQuantity,
                    SpecificationsUnit = consumable.SpecificationsUnit,
                    Packaging = consumable.Packaging,
                    PackageUnit = consumable.PackageUnit,
                    ProcurementPackage = consumable.ProcurementPackage,
                    ProcurementUnit = consumable.ProcurementUnit,
                    DoseUnit = consumable.DoseUnit,
                    DoseMin = consumable.DoseMin,
                    PackageSpecifications = consumable.PackageSpecifications,
                    IsSplited = consumable.IsSplited,
                    IsSalesReturn = consumable.IsSalesReturn,
                    MonthDosage = consumable.MonthDosage,
                    YearDosage = consumable.YearDosage,
                    ArtificialLoss = consumable.ArtificialLoss,
                    Iindications = consumable.Iindications,
                    Usage = consumable.Usage,
                    StorageConditions = consumable.StorageConditions,
                    FirstStock = consumable.FirstStock,
                    MinInventory = consumable.MinInventory,
                    FeeTypeId = consumable.FeeTypeId,
                    Manufacturer = consumable.Manufacturer,
                    SupplierId = consumable.SupplierId,
                    SupplierName = supplier?.Name,
                    DataState = consumable.DataState,
                    DataStateText = GetDataStateText(consumable.DataState),
                    ApplyState = consumable.ApplyState,
                    ApplyStateText = consumable.ApplyState == 1 ? "可采购" : "不可采购",
                    Remark = consumable.Remark,
                    Founder = consumable.Founder,
                    FounderDate = consumable.FounderDate,
                    Modifier = consumable.Modifier,
                    ModifierDate = consumable.ModifierDate,
                    CenterId = consumable.CenterId
                });
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取耗材
        /// </summary>
        public async Task<ConsumableOutput> GetConsumableByIdAsync(string id)
        {
            var consumable = await _medicalItemRecordRepository.GetByIdAsync(id);
            if (consumable == null) return null;

            var supplier = await _supplierRepository.GetByIdAsync(consumable.SupplierId);
            return new ConsumableOutput
            {
                Id = consumable.Id,
                MedicalItemName = consumable.MedicalItemName,
                MedicalItemCode = consumable.MedicalItemCode,
                MedicalItemWorkCode = consumable.MedicalItemWorkCode,
                GoodsName = consumable.GoodsName,
                AliasName = consumable.AliasName,
                MedicalItemType = consumable.MedicalItemType,
                MedicalItemTypeText = consumable.MedicalItemType == 1 ? "药品" : "耗材",
                EnglishName = consumable.EnglishName,
                Brand = consumable.Brand,
                Form = consumable.Form,
                Specifications = consumable.Specifications,
                SpecificationsQuantity = consumable.SpecificationsQuantity,
                SpecificationsUnit = consumable.SpecificationsUnit,
                Packaging = consumable.Packaging,
                PackageUnit = consumable.PackageUnit,
                ProcurementPackage = consumable.ProcurementPackage,
                ProcurementUnit = consumable.ProcurementUnit,
                DoseUnit = consumable.DoseUnit,
                DoseMin = consumable.DoseMin,
                PackageSpecifications = consumable.PackageSpecifications,
                IsSplited = consumable.IsSplited,
                IsSalesReturn = consumable.IsSalesReturn,
                MonthDosage = consumable.MonthDosage,
                YearDosage = consumable.YearDosage,
                ArtificialLoss = consumable.ArtificialLoss,
                Iindications = consumable.Iindications,
                Usage = consumable.Usage,
                StorageConditions = consumable.StorageConditions,
                FirstStock = consumable.FirstStock,
                MinInventory = consumable.MinInventory,
                FeeTypeId = consumable.FeeTypeId,
                Manufacturer = consumable.Manufacturer,
                SupplierId = consumable.SupplierId,
                SupplierName = supplier?.Name,
                DataState = consumable.DataState,
                DataStateText = GetDataStateText(consumable.DataState),
                ApplyState = consumable.ApplyState,
                ApplyStateText = consumable.ApplyState == 1 ? "可采购" : "不可采购",
                Remark = consumable.Remark,
                Founder = consumable.Founder,
                FounderDate = consumable.FounderDate,
                Modifier = consumable.Modifier,
                ModifierDate = consumable.ModifierDate,
                CenterId = consumable.CenterId
            };
        }

        /// <summary>
        /// 添加耗材
        /// </summary>
        public async Task<bool> AddConsumableAsync(ConsumableInput input)
        {
            var consumable = new MedicalItemRecord
            {
                Id = Guid.NewGuid().ToString(),
                MedicalItemName = input.MedicalItemName,
                MedicalItemCode = input.MedicalItemCode,
                MedicalItemWorkCode = input.MedicalItemWorkCode,
                GoodsName = input.GoodsName,
                AliasName = input.AliasName,
                MedicalItemType = input.MedicalItemType,
                EnglishName = input.EnglishName,
                Brand = input.Brand,
                Form = input.Form,
                Specifications = input.Specifications,
                SpecificationsQuantity = input.SpecificationsQuantity,
                SpecificationsUnit = input.SpecificationsUnit,
                Packaging = input.Packaging,
                PackageUnit = input.PackageUnit,
                ProcurementPackage = input.ProcurementPackage,
                ProcurementUnit = input.ProcurementUnit,
                DoseUnit = input.DoseUnit,
                DoseMin = input.DoseMin,
                PackageSpecifications = input.PackageSpecifications,
                IsSplited = input.IsSplited,
                IsSalesReturn = input.IsSalesReturn,
                MonthDosage = input.MonthDosage,
                YearDosage = input.YearDosage,
                ArtificialLoss = input.ArtificialLoss,
                Iindications = input.Iindications,
                Usage = input.Usage,
                StorageConditions = input.StorageConditions,
                FirstStock = input.FirstStock,
                MinInventory = input.MinInventory,
                FeeTypeId = input.FeeTypeId,
                Manufacturer = input.Manufacturer,
                SupplierId = input.SupplierId,
                DataState = input.DataState,
                ApplyState = input.ApplyState,
                Remark = input.Remark,
                Founder = input.Founder,
                FounderDate = DateTime.Now,
                Modifier = input.Modifier,
                ModifierDate = DateTime.Now,
                CenterId = input.CenterId,
                IsDelete = false
            };

            await _medicalItemRecordRepository.AddAsync(consumable);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新耗材
        /// </summary>
        public async Task<bool> UpdateConsumableAsync(ConsumableInput input)
        {
            var consumable = await _medicalItemRecordRepository.GetByIdAsync(input.Id);
            if (consumable == null) return false;

            consumable.MedicalItemName = input.MedicalItemName;
            consumable.MedicalItemCode = input.MedicalItemCode;
            consumable.MedicalItemWorkCode = input.MedicalItemWorkCode;
            consumable.GoodsName = input.GoodsName;
            consumable.AliasName = input.AliasName;
            consumable.MedicalItemType = input.MedicalItemType;
            consumable.EnglishName = input.EnglishName;
            consumable.Brand = input.Brand;
            consumable.Form = input.Form;
            consumable.Specifications = input.Specifications;
            consumable.SpecificationsQuantity = input.SpecificationsQuantity;
            consumable.SpecificationsUnit = input.SpecificationsUnit;
            consumable.Packaging = input.Packaging;
            consumable.PackageUnit = input.PackageUnit;
            consumable.ProcurementPackage = input.ProcurementPackage;
            consumable.ProcurementUnit = input.ProcurementUnit;
            consumable.DoseUnit = input.DoseUnit;
            consumable.DoseMin = input.DoseMin;
            consumable.PackageSpecifications = input.PackageSpecifications;
            consumable.IsSplited = input.IsSplited;
            consumable.IsSalesReturn = input.IsSalesReturn;
            consumable.MonthDosage = input.MonthDosage;
            consumable.YearDosage = input.YearDosage;
            consumable.ArtificialLoss = input.ArtificialLoss;
            consumable.Iindications = input.Iindications;
            consumable.Usage = input.Usage;
            consumable.StorageConditions = input.StorageConditions;
            consumable.FirstStock = input.FirstStock;
            consumable.MinInventory = input.MinInventory;
            consumable.FeeTypeId = input.FeeTypeId;
            consumable.Manufacturer = input.Manufacturer;
            consumable.SupplierId = input.SupplierId;
            consumable.DataState = input.DataState;
            consumable.ApplyState = input.ApplyState;
            consumable.Remark = input.Remark;
            consumable.Modifier = input.Modifier;
            consumable.ModifierDate = DateTime.Now;
            consumable.CenterId = input.CenterId;

            _medicalItemRecordRepository.Update(consumable);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除耗材
        /// </summary>
        public async Task<bool> DeleteConsumableAsync(string id)
        {
            var consumable = await _medicalItemRecordRepository.GetByIdAsync(id);
            if (consumable == null) return false;

            consumable.IsDelete = true;
            _medicalItemRecordRepository.Update(consumable);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取数据状态文本
        /// </summary>
        private string GetDataStateText(int dataState)
        {
            switch (dataState)
            {
                case 1: return "正常";
                case 2: return "无效";
                case 3: return "删除";
                default: return "未知";
            }
        }
    }
}
