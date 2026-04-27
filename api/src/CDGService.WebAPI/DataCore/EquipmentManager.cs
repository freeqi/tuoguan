﻿﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Utils;
using CDGService.Data.Enums;
using CDGService.Data;
using Microsoft.Extensions.Options;

namespace CDGService.WebAPI.DataCore
{
    public class EquipmentManager
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _getUserInfo;
        private readonly DictionaryCode _dictionaryCode;
        private readonly IMapper _mapper;
        public EquipmentManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IOptions<DictionaryCode> dictionaryCode, IMapper mapper)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _dictionaryCode = dictionaryCode.Value;
            _mapper = mapper;
        }
        private IRepository<EquipmentInfo> EquipmentStore => _unitOfWork.GetStore<EquipmentInfo>();
        private IRepository<SystemDictionary> DictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<EquipmentType> EquipmentTypeStore => _unitOfWork.GetStore<EquipmentType>();
        private IRepository<EquipmentSupplier> EquipmentSupplierStore => _unitOfWork.GetStore<EquipmentSupplier>();
        private IRepository<EquipmentModel> EquipmentModelStore => _unitOfWork.GetStore<EquipmentModel>();
        private IRepository<EquipmentManufacturers> EquipmentManufacturersStore => _unitOfWork.GetStore<EquipmentManufacturers>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        /// <summary>
        /// 设备列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PageData<EquipmentOutPut[]>> GetEquipmentQueryableAsync(EquipmentInPut input = null)
        {
            return Task.Run(async () =>
            {
                EquipmentOutPut[] result = null;
                int count = 0;
                Expression<Func<EquipmentInfo, bool>> predicate = t => t.Id != "";
                if (input.CenterId != "" && input.CenterId != "0")
                    predicate = predicate.And(t => t.CenterId == input.CenterId);
                if (input.Name + "" != "")
                {
                    var data = DictionaryStore.GetFirstOrDefault(t => t.Name.Contains(input.Name));
                    if (data != null)
                    {
                        predicate = predicate.And(t => t.Name == data.Id);
                    }
                    else
                        predicate = predicate.And(t => t.Name == "");
                }
                if (input.id + "" != "")
                    predicate = predicate.And(t => t.Id == input.id);
                if (input.EqType + "" != "0" && input.EqType + "" != "")
                {
                    if (input.EqType == "010f81319f3b41b6a9a30216d735dc46") input.EqType = "621a6df986634468adfe7ec5a24ad6e0";
                    if (input.EqType == "6f65d7538e0844c2bb9f8c92e650b493") input.EqType = "285b4938d09d425d9bf2f5b0c6c657c1";
                    if (input.EqType == "eb728c7a549b4d4e8e60c2bfe1e85d1f") input.EqType = "39946bb699cd4c29a45c714d2985d1e1";
                    if (input.EqType == "8ca204264d374d739503c6b414c95d7a") input.EqType = "1ca9ff2b907b4b69af2ef7d04a1a3b78";
                    predicate = predicate.And(t => t.EquipType == input.EqType);
                }
                if (input.EqState > 0)
                    predicate = predicate.And(t => t.EquipmentState == input.EqState.ToString().Substring(1));
                try
                {
                    var Sum = EquipmentStore.Entities.Include(t => t.Dialysis).Include(t => t.SBloodBorneDisease).Include(t => t.SName).Include(t => t.SEquipType).Include(t => t.DicTreatmentRegion).Where(predicate);
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {

                        var persons = await PaginatedList<EquipmentInfo>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        result = _mapper.Map<EquipmentOutPut[]>(persons);

                        count = Sum.Count();
                    }
                    else
                    {
                        var datas = await Sum.ToArrayAsync();
                        result = _mapper.Map<EquipmentOutPut[]>(datas);
                        count = result.Length;

                    }
                }
                catch (Exception ex)
                { 
                    throw new Exception(ex.Message, ex);
                }

                return new PageData<EquipmentOutPut[]>(result, count);
            });
        }

        //统计 型号统计  设备状态  型号分布 机器使用年限

        //型号统计  

        public Task<EmplyeeByStaBisOutPut[]> GetEquiByModelQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.ModelTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();
                    var EquiData = await EquipmentStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    int count = 0;
                    foreach (var item in JobTitleData)
                    {
                        if (CenterId != "" && CenterId != "0")
                            count = EquiData.Where(t => String.Equals(t.Model.Trim(), item.Name, StringComparison.CurrentCultureIgnoreCase) && t.CenterId == CenterId).Count();
                        else
                            count = EquiData.Where(t => String.Equals(t.Model.Trim(), item.Name, StringComparison.CurrentCultureIgnoreCase)).Count();

                        result.Add(new EmplyeeByStaBisOutPut() { name = item.Name, value = count });
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });


        }

        //型号分布
        public Task<LineOutPut> GetEquiByModelDistributionQueryableAsync()
        {

            // data['康美','弹子石','沙坪坝',……] 
            return Task.Run(async () =>
            {
                try
                {
                    bool IsDel = false;
                    var modelData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == _dictionaryCode.ModelTypeId).OrderBy(t => t.ShowSortNo).ToListAsync();

                    var centerData = await CenterDialysisStore.Entities.Where(t => t.IsDelete == IsDel).ToListAsync();
                    LineOutPut result = new LineOutPut();
                    result.LegendData = modelData.Select(t => t.Name).ToList();
                    result.data = centerData.Select(t => t.ShortName).ToList();
                    var EquiData = await EquipmentStore.Entities.Where(t => t.EquipType == _dictionaryCode.EqTypeId).ToListAsync();
                    foreach (var model in modelData)
                    {
                        serise serise = new serise();
                        serise.name = model.Name;
                        foreach (var item in centerData)
                        {
                            // 
                            //var DA=  EquiData.GroupBy(T => T.Model);
                            int count = EquiData.Where(t => String.Equals(t.Model.Trim(), model.Name, StringComparison.CurrentCultureIgnoreCase) && t.CenterId == item.Id).Count();
                            serise.data.Add(count);
                        }
                        result.serises.Add(serise);
                    }
                    return result;
                }
                catch (Exception exp)
                {
                    throw new Exception("未获取到数据", exp);
                }
            });
        }

        //设备状态
        public Task<EmplyeeByStaBisOutPut[]> GetEquiByStateQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                // bool IsDel = false;
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {
                    // 
                    //  var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == (int)DicType.EquiModel).OrderBy(t => t.ShowSortNo).ToListAsync();
                    var EquiData = await EquipmentStore.Entities.ToListAsync();
                    int count = 0;
                    foreach (EquiState equiState in Enum.GetValues(typeof(EquiState)))
                    {
                        if (CenterId != "" && CenterId != "0")
                            count = EquiData.Where(t => t.EquipmentState == equiState.ToString().Substring(1) && t.CenterId == CenterId).Count();     
                        else
                            count = EquiData.Where(t => t.EquipmentState == equiState.ToString().Substring(1)).Count();

                        result.Add(new EmplyeeByStaBisOutPut() { name = equiState.ToChinese(), value = count });
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });

        }

        //使用年限 EquiUseYear

        public Task<EmplyeeByStaBisOutPut[]> GetEquiByUseYearQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {

                // bool IsDel = false;  
                List<EmplyeeByStaBisOutPut> result = new List<EmplyeeByStaBisOutPut>();

                try
                {

                    //   var JobTitleData = await DictionaryStore.Entities.Where(t => t.IsDelete == IsDel && t.TypeId == (int)DicType.EquiModel).OrderBy(t => t.ShowSortNo).ToListAsync();
                    var EquiData = await EquipmentStore.Entities.Where(t => t.DataState == 1).ToListAsync();
                    int count = 0;
                    foreach (EquiUseYear UseYear in Enum.GetValues(typeof(EquiUseYear)))
                    {
                        if (UseYear != EquiUseYear.Seven)
                        {
                            if (CenterId != "" && CenterId != "0")
                                count = EquiData.Where(t => t.CenterId == CenterId && t.PurchaseDate > DateTime.Now.AddYears(-(int)UseYear) && t.PurchaseDate <= DateTime.Now.AddYears(-(int)UseYear + 1)).Count();
                            else
                                count = EquiData.Where(t => t.PurchaseDate > DateTime.Now.AddYears(-(int)UseYear) && t.PurchaseDate <= DateTime.Now.AddYears(-(int)UseYear + 1)).Count();

                        }
                        else
                        {
                            if (CenterId + "" != "")
                                count = EquiData.Where(t => t.CenterId == CenterId && t.PurchaseDate < DateTime.Now.AddYears(-6)).Count();
                            else
                                count = EquiData.Where(t => t.PurchaseDate < DateTime.Now.AddYears(-6)).Count();

                        }


                        result.Add(new EmplyeeByStaBisOutPut() { name = UseYear.ToChinese(), value = count });
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }

                return result.ToArray();
            });

        }

        /// <summary>
        /// 添加设备
        /// </summary>
        public async Task<bool> AddEquipmentAsync(EquipmentInfo equipment)
        {
            equipment.Id = Guid.NewGuid().ToString();
            equipment.FounderDate = DateTime.Now;
            await EquipmentStore.AddAsync(equipment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        public async Task<bool> UpdateEquipmentAsync(EquipmentInfo equipment)
        {
            var existingEquipment = await EquipmentStore.GetByIdAsync(equipment.Id);
            if (existingEquipment == null) return false;

            existingEquipment.Name = equipment.Name;
            existingEquipment.EquipType = equipment.EquipType;
            existingEquipment.Model = equipment.Model;
            existingEquipment.CenterId = equipment.CenterId;
            existingEquipment.SerialNumber = equipment.SerialNumber;
            existingEquipment.TreatmentRegion = equipment.TreatmentRegion;
            existingEquipment.EquipmentState = equipment.EquipmentState;
            existingEquipment.IPAddress = equipment.IPAddress;
            existingEquipment.BedNo = equipment.BedNo;
            existingEquipment.BloodBorneDisease = equipment.BloodBorneDisease;
            existingEquipment.TreatmentModels = equipment.TreatmentModels;
            existingEquipment.EngineerName = equipment.EngineerName;
            existingEquipment.EngineerPhone = equipment.EngineerPhone;
            existingEquipment.PurchaseDate = equipment.PurchaseDate;
            existingEquipment.Supplier = equipment.Supplier;
            existingEquipment.SupplierTelphone = equipment.SupplierTelphone;
            existingEquipment.PurchaseMoney = equipment.PurchaseMoney;
            existingEquipment.Producer = equipment.Producer;
            existingEquipment.ProducerTelphone = equipment.ProducerTelphone;
            existingEquipment.ProduceDate = equipment.ProduceDate;
            existingEquipment.MaintenanceDate = equipment.MaintenanceDate;
            existingEquipment.DataState = equipment.DataState;
            existingEquipment.Modifier = equipment.Modifier;
            existingEquipment.ModifierDate = DateTime.Now;

            EquipmentStore.Update(existingEquipment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        public async Task<bool> DeleteEquipmentAsync(string id)
        {
            var equipment = await EquipmentStore.GetByIdAsync(id);
            if (equipment == null) return false;

            equipment.IsDelete = true;
            EquipmentStore.Update(equipment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        public async Task<EquipmentInfo> GetEquipmentByIdAsync(string id)
        {
            return await EquipmentStore.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取设备类型列表
        /// </summary>
        public async Task<List<EquipmentType>> GetEquipmentTypesAsync()
        {
            return await EquipmentTypeStore.GetAllAsync();
        }

        /// <summary>
        /// 获取设备供应商列表
        /// </summary>
        public async Task<List<EquipmentSupplier>> GetEquipmentSuppliersAsync()
        {
            return await EquipmentSupplierStore.GetAllAsync();
        }

        /// <summary>
        /// 获取设备制造商列表
        /// </summary>
        public async Task<List<EquipmentManufacturers>> GetEquipmentManufacturersAsync()
        {
            return await EquipmentManufacturersStore.GetAllAsync();
        }

        /// <summary>
        /// 获取设备型号列表
        /// </summary>
        public async Task<List<EquipmentModel>> GetEquipmentModelsAsync()
        {
            return await EquipmentModelStore.GetAllAsync();
        }

        /// <summary>
        /// 根据设备类型获取设备型号列表
        /// </summary>
        public async Task<List<EquipmentModel>> GetEquipmentModelsByTypeIdAsync(string typeId)
        {
            return await EquipmentModelStore.GetAllAsync(m => m.TypeId == typeId);
        }

        /// <summary>
        /// 根据制造商获取设备型号列表
        /// </summary>
        public async Task<List<EquipmentModel>> GetEquipmentModelsByManufacturerIdAsync(string manufacturerId)
        {
            return await EquipmentModelStore.GetAllAsync(m => m.ManufacturersId == manufacturerId);
        }

    }
}
