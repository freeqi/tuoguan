﻿using AutoMapper;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Store;
using CDGService.Utils;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDGService.Data.Helper;
using CDGService.Data;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.JsonPatch.Internal;
using static CDGService.Application.DataCollect.CollentStartup;
using Newtonsoft.Json;
using CDGService.Application.Service.Net;

namespace CDGService.WebAPI.DataCore
{
    public class DataManager
    {
        private readonly IGetUserInfo _getUserInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LogManager _logManager;
        private readonly Data.DocumentSetting _documentSetting;
        private readonly Data.DictionaryCode _dictionaryCode;

        private readonly IDataReceive _dataReceive;
        private List<PurchSubmit> _purchSubmit = new List<PurchSubmit>();
        private List<OrderPricingRole> _orderPricingRole = new List<OrderPricingRole>();
        public DataManager(IUnitOfWork unitOfWork, LogManager logManager, IGetUserInfo getUserInfo, IDataReceive dataReceive, IOptions<Data.DocumentSetting> documentSetting, IOptions<Data.DictionaryCode> dictionaryCode, Microsoft.Extensions.Options.IOptions<List<OrderPricingRole>> orderPricingRol, Microsoft.Extensions.Options.IOptions<List<PurchSubmit>> purchSubmit)
        {

            _getUserInfo = getUserInfo;
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _documentSetting = documentSetting.Value;
            _dictionaryCode = dictionaryCode.Value;
            _orderPricingRole = orderPricingRol.Value;
            _purchSubmit = purchSubmit.Value;
            _dataReceive = dataReceive;
        }
        private IRepository<Log> LogStore => _unitOfWork.GetStore<Log>();

        private IRepository<WarehouseCatalog> WarehouseCatalogStore => _unitOfWork.GetStore<WarehouseCatalog>();

        private IRepository<Supplier> SupplierStore => _unitOfWork.GetStore<Supplier>();
        private IRepository<MedicalItemRecord> MedicalItemRecordStore => _unitOfWork.GetStore<MedicalItemRecord>();

        private IRepository<DosageForm> DosageFormStore => _unitOfWork.GetStore<DosageForm>();
        private IRepository<MedicalUnit> MedicalUnitStore => _unitOfWork.GetStore<MedicalUnit>();

        private IRepository<UseWay> UseWayStore => _unitOfWork.GetStore<UseWay>();

        private IRepository<MedicalDrugExtension> MedicalDrugExtensionStore => _unitOfWork.GetStore<MedicalDrugExtension>();
        private IRepository<SysRegion> SysRegionStore => _unitOfWork.GetStore<SysRegion>();
        private IRepository<SI_YPMLS> SI_YPMLSStore => _unitOfWork.GetStore<SI_YPMLS>();
        private IRepository<SI_ZLXMS> SI_ZLXMStore => _unitOfWork.GetStore<SI_ZLXMS>();

        private IRepository<DictionaryType> DictionaryTypeStore => _unitOfWork.GetStore<DictionaryType>();
        private IRepository<SystemDictionary> SystemDictionaryStore => _unitOfWork.GetStore<SystemDictionary>();
        private IRepository<Employee> EmployeeStore => _unitOfWork.GetStore<Employee>();

        private IRepository<Patient> PatientStore => _unitOfWork.GetStore<Patient>();
        private IRepository<SI_BZML> SI_BZMLStore => _unitOfWork.GetStore<SI_BZML>();

        private IRepository<SI_HC> SI_HCStore => _unitOfWork.GetStore<SI_HC>();
        private IRepository<MedicalRelevancy> MRelevancyStore => _unitOfWork.GetStore<MedicalRelevancy>();
        private IRepository<MedMatchCode> MedMatchCodeStore => _unitOfWork.GetStore<MedMatchCode>();
        private IRepository<MedMonthAudit> MedMonthAuditStore => _unitOfWork.GetStore<MedMonthAudit>();

        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <returns></returns>
        public Task<PageData<LogOutPut[]>> GetLogQueryableAsync(LogInput logInput)
        {
            return Task.Run(async () =>
            {
                LogOutPut[] result = null;
                if (logInput == null)
                {
                    var logs = await LogStore.Entities.Include(x => x.OperatorUser).ThenInclude(x => x.Employee).OrderByDescending(t => t.CreateTime).ToArrayAsync();
                    result = Mapper.Map<LogOutPut[]>(logs);
                    return new PageData<LogOutPut[]>(result, result.Length);
                }
                switch (logInput.Day)
                {
                    case -1:
                        logInput.StartTime = DateTime.Now.Date.AddDays(-1);
                        logInput.EndTime = DateTimeHelper.ToDayLastMinitue(DateTime.Now);
                        break;
                    case 1:
                        logInput.StartTime = DateTime.Now.Date;
                        logInput.EndTime = DateTimeHelper.ToDayLastMinitue(DateTime.Now);
                        break;
                    case 7:
                        logInput.StartTime = DateTime.Now.Date.AddDays(-7);
                        logInput.EndTime = DateTimeHelper.ToDayLastMinitue(DateTime.Now);
                        break;
                    case 30:
                        logInput.StartTime = DateTime.Now.Date.AddMonths(-1);
                        logInput.EndTime = DateTimeHelper.ToDayLastMinitue(DateTime.Now);
                        break;

                    default:
                        break;
                }
                Expression<Func<Log, bool>> predicate = t => t.OperatorId != "a51e842fdb544dfea3c274aee6d5ec0f" && t.LogCode != LogType.UserLogin;//过滤内部人员 和登录信息
                if (logInput.StartTime.HasValue)
                    predicate = predicate.And(t => t.CreateTime >= logInput.StartTime.Value);
                if (logInput.EndTime.HasValue)
                    predicate = predicate.And(t => t.CreateTime <= logInput.EndTime.Value);
                if (logInput.LogCode.HasValue && logInput.LogCode > 0)
                    predicate = predicate.And(t => t.LogCode == logInput.LogCode);

                var datas = LogStore.Entities.Include(x => x.OperatorUser).ThenInclude(x => x.Employee).Where(predicate).OrderByDescending(t => t.CreateTime);




                if (logInput.PageNum > 0 && logInput.PageSize > 0)
                {
                    var Dialysis = await PaginatedList<Log>.CreateAsync(datas, logInput.PageNum, logInput.PageSize);
                    result = Mapper.Map<LogOutPut[]>(Dialysis);
                }
                else
                {
                    result = Mapper.Map<LogOutPut[]>(datas);
                }
                int count = datas.Count();
                // return result;
                return new PageData<LogOutPut[]>(result, count);
            });
        }

        /// <summary>
        /// 获取地区列表 
        /// </summary>
        /// <returns></returns>
        public Task<SysRegionOutput[]> GetSysRegionQueryableAsync()
        {
            return Task.Run(async () =>
            {
                SysRegionOutput[] result = null;
                SysRegionOutput curItem = new SysRegionOutput() { title = "根节点", Id = "0", RegionParentId = "0" };
                try
                {
                    var datas = await SysRegionStore.Entities.ToArrayAsync();
                    result = Mapper.Map<SysRegionOutput[]>(datas);

                    SysRegionLoopToAppendChildren(result, curItem);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return curItem.children.ToArray();
            });
        }
        public void SysRegionLoopToAppendChildren(SysRegionOutput[] catelist, SysRegionOutput children)
        {
            var subItems = catelist.Where(x => x.RegionParentId == children.Id).ToList();
            children.children = new List<SysRegionOutput>();
            // subItems.Sort(t=>t.);
            // var query = from items in subItems orderby items.SortNo select items;
            children.children.AddRange(subItems);
            foreach (var item in subItems)
            {
                SysRegionLoopToAppendChildren(catelist, item);
            }
        }

        #region 物品类别目录


        /// <summary>
        /// 获取菜单列表(树)
        /// </summary>
        public Task<WarehouseCatalogOutPut[]> GetWarehouseCatalogTreeQueryableAsync(int HouseType = 0)
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                WarehouseCatalogOutPut[] result = null;
                string Name = "根节点";
                Expression<Func<WarehouseCatalog, bool>> predicate = t => t.IsDelete == IsDel;
                string Id = "0";
                if (HouseType > 0)
                {
                    switch (HouseType)
                    {
                        case 1:
                            Id = _dictionaryCode.DrugArchivesID;
                            break;
                        case 2:
                            Id = _dictionaryCode.HealthSuppliesID;
                            break;
                        case 3:
                            Id = _dictionaryCode.FixedAssetsID;

                            break;
                        case 4:
                            Id = _dictionaryCode.LowValueID;
                            break;
                        case 5:
                            Id = _dictionaryCode.TreatmentItmeID;
                            break;

                        default:
                            break;
                    }
                }
                WarehouseCatalogOutPut curItem = new WarehouseCatalogOutPut() { Name = Name, Id = Id, ParentId = "" };
                try
                {
                    var datas = await WarehouseCatalogStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<WarehouseCatalogOutPut[]>(datas);

                    LoopToAppendChildren(result, curItem);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return curItem.children.ToArray();

            });
        }

        /// <summary>
        /// 获取菜单列表[]
        /// </summary>
        public Task<WarehouseCatalogOutPut[]> GetWarehouseCatalogQueryableAsync()
        {
            return Task.Run(async () =>
            {
                WarehouseCatalogOutPut[] result = null;

                try
                {
                    var datas = await WarehouseCatalogStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<WarehouseCatalogOutPut[]>(datas);


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;
            });
        }



        /// <summary>
        ///  新增/修改库房目录
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateWarehouseCatalogAsync(WarehouseCatalogInput input)
        {
            return Task.Run(async () =>
            {
                //if (input.Id.HasValue && (input.Id.Value == 1 || input.Id.Value == 2 || input.Id.Value == 3))
                //{
                //    throw new Exception("当前库房目录不可修改！");
                //}

                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.MenuName"));

                bool isRe = await GetNameRepeat(input.Name, input.Id);
                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("库房名称", input.Name));
                try
                {
                    WarehouseCatalogOutPut ParentMenu = null;
                    if (input.ParentId + "" != "")
                    {
                        ParentMenu = await GetUpMenu(input.ParentId + "");
                    }
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    WarehouseCatalog data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await WarehouseCatalogStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;

                        if (ParentMenu == null)
                        {

                            data.ParentId = "0";
                        }
                        else
                        {
                            data.ParentId = ParentMenu.Id;

                        }
                        WarehouseCatalogStore.Update(data);
                    }
                    else
                    {
                        data = Mapper.Map<WarehouseCatalog>(input);
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.Id = Guid.NewGuid().tostring32();
                        data.DataState = 1;

                        bool flag = true;
                        while (flag)
                        {
                            data.HouseSysCode = ValueHelper.TimeNowRandom('H');
                            var Codedata = await WarehouseCatalogStore.GetFirstOrDefaultAsync(t => t.HouseSysCode == data.HouseSysCode);
                            if (Codedata == null)
                                flag = false;
                        }
                        WarehouseCatalogStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "库房目录",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }


        /// <summary>
        /// 删除库房目录
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<bool> DelWarehouseCatalogAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    //if (Id == 1 || Id == 2 || Id == 3)
                    //{
                    //    throw new Exception("该库房目录不可删除！");
                    //}

                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    WarehouseCatalog data = null;

                    data = await WarehouseCatalogStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    if (await GetcMenus(data.Id))
                        throw new Exception("该库房目录下存在子目录，不可删除！");
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.IsDelete = true;
                    data.DataState = 3;
                    WarehouseCatalogStore.Update(data);

                    await
                        _logManager.WriteLogAsync(LogType.DataDelete, "库房目录",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }



        /// <summary>
        /// 获取父级菜单
        /// </summary>
        /// <param name="ParentMenuCode"></param>           
        public Task<WarehouseCatalogOutPut> GetUpMenu(string ParentID)
        {
            return Task.Run(async () =>
            {
                var data = await WarehouseCatalogStore.GetFirstOrDefaultAsync(t => t.Id == ParentID && t.IsDelete == false);
                return Mapper.Map<WarehouseCatalogOutPut>(data);
            });

        }
        /// <summary>
        /// 判断是否有子菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Task<bool> GetcMenus(string id)
        {
            return Task.Run(async () =>
            {
                var data = await WarehouseCatalogStore.GetFirstOrDefaultAsync(t => t.ParentId == id && t.IsDelete == false);
                if (data == null)
                {
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        private Task<bool> GetNameRepeat(string name, string id = "0")
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await WarehouseCatalogStore.Entities.Where(t => t.Name == name && t.IsDelete == false).ToArrayAsync();
                    if (data == null)
                        IsRepeat = false;
                    if (data != null && data.Where(t => t.Id != id).Count() > 0)
                        IsRepeat = true;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return IsRepeat;
            });

        }


        public void LoopToAppendChildren(WarehouseCatalogOutPut[] catelist, WarehouseCatalogOutPut children)
        {
            var subItems = catelist.Where(x => x.ParentId == children.Id).ToList();
            children.children = new List<WarehouseCatalogOutPut>();
            // subItems.Sort(t=>t.);
            var query = from items in subItems orderby items.SortNo select items;
            children.children.AddRange(query);
            foreach (var item in subItems)
            {
                LoopToAppendChildren(catelist, item);
            }
        }


        #endregion

        #region 物品档案

        public Task<PageData<MedicalItemRecordOutPut[]>> GetMedicalItemRecordQueryableAsync(MedicalItemRecordSearchInput input = null)
        {

            return Task.Run(async () =>
            {
                // kk();
                bool IsDel = input.DataState == 3 ? true : false;
                MedicalItemRecordOutPut[] result = null;

                int count = 0;
                try
                {
                    if (input == null)
                    {
                        input = new MedicalItemRecordSearchInput();
                    }

                    var catet = await WarehouseCatalogStore.Entities.Where(t => t.IsDelete == false && t.DataState == 1).ToListAsync();
                    if (input.CenterId + "" == "") input.CenterId = "0";
                    Expression<Func<MedicalItemRecord, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.MedicalItemName.Contains(input.Name) || t.Mnemonic.Contains(input.Name) || t.MedicalItemCode.Contains(input.Name) || t.supplier.Name.Contains(input.Name));
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);

                    if (input.MedicalItemType > 0)
                        predicate = predicate.And(t => t.MedicalItemType == input.MedicalItemType);
                    if (input.WareHouseId + "" != "" && input.WareHouseId + "" != "0")
                    {
                        List<string> child = new List<string>();
                        child.Add(input.WareHouseId);
                        List<string> MainId = new List<string>();
                        MainId.Add(input.WareHouseId);
                        GetchildWareHouse(MainId.ToArray(), child, catet);
                        predicate = predicate.And(t => child.Contains(t.WareHouseIds));
                    }

                    if (input.IsAbnormal)
                    {
                        var data = await MedicalDrugExtensionStore.Entities.Include(t => t.medicalItemRecord).Where(t => t.IsCurrentUse == true && t.RetailPrice > t.SocialSecurityPrice && t.medicalItemRecord.IsDelete == IsDel && t.medicalItemRecord.HiCenterCode + "" != "" && t.medicalItemRecord.MedicalItemType == input.MedicalItemType).Select(t => t.MedicalId).ToListAsync();
                        predicate = predicate.And(t => data.Contains(t.Id));
                    }
                    // predicate = predicate.And(t =>t.MedicalDrugExtensions.Where(k=>k.IsCurrentUse);
                    /*
                     
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
        public string SalePrice { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Packaging { get; set; }
                     */
                    if (input.GoodsName + "" != "")
                        predicate = predicate.And(t => t.GoodsName.Contains(input.Brand));
                    //if (input.Brand + "" != "")
                    //    predicate = predicate.And(t => t.Brand.Contains(input.Brand));
                    if (input.SalePrice > 0)
                        predicate = predicate.And(t => t.MedicalDrugExtensions.Where(d => d.IsCurrentUse).FirstOrDefault().RetailPrice <= input.SalePrice);
                    if (input.Manufacturer + "" != "")
                        predicate = predicate.And(t => t.Manufacturer.Contains(input.Manufacturer));
                    if (input.Packaging + "" != "")
                        predicate = predicate.And(t => t.Packaging.Contains(input.Packaging));
                    if (input.nationItemCode + "" != "")
                        predicate = predicate.And(t => t.NationItemCode.Contains(input.nationItemCode));


                    #region  获取最新采购时间 和库存

                    string QueryOrderSQL = @"SELECT 
    pd.Id,
    a.Id  as  MedicalItemId,
    a.MedicalItemName,
    pd.FounderDate
FROM 
    MedicalItemRecords a
     JOIN 
    PurchaseDetails pd ON a.Id = pd.MedicalItemId
   JOIN (
    SELECT 
        MedicalItemId,
        MAX(FounderDate) AS MaxPurchaseDate
    FROM 
        PurchaseDetails
    GROUP BY 
        MedicalItemId
) maxDates ON pd.MedicalItemId = maxDates.MedicalItemId AND pd.FounderDate = maxDates.MaxPurchaseDate  order by FounderDate  desc";


                    //最新采集时间
                    var purchaseDatas = _unitOfWork.SqlQuery<MedFrequencyOutPut>(QueryOrderSQL).ToList();
                    //库存

                    //     await centerDocking.GetGoodsInStockdInfo();

                    var MedInStock = CenterDockingManger.listStockData;



                    #endregion



                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        var Sum = MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Include(t => t.ProcurementUnits).Where(predicate).OrderBy(t => t.ApplyState);
                        var Dialysis = await PaginatedList<MedicalItemRecord>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        // result = Mapper.Map<CenterDialysisOutPut[]>(Dialysis);
                        count = Sum.Count();

                        result = Mapper.Map<MedicalItemRecordOutPut[]>(Dialysis);
                        int i = 0;


                        foreach (var item in result)
                        {

                            var Frequency = purchaseDatas.Where(t => t.MedicalItemId == item.Id).FirstOrDefault();
                            if (Frequency != null && Frequency.FounderDate != null)
                            {
                                var datetimeNow = DateTime.Now;
                                int monthsDifference = (datetimeNow.Year - Frequency.FounderDate.Value.Year) * 12 + (datetimeNow.Month - Frequency.FounderDate.Value.Month);

                                item.MedFrequency = monthsDifference;
                            }
                            else
                                item.MedFrequency = 99;


                            item.MedInventory = MedInStock.Where(t => t.MedicalItemId == item.Id).Sum(t => t.InQty);
                            //  GetManId(Dialysis[i].WareHouseIds, item.WareHouseId);
                            //  GetFromManId(Dialysis[i].Form, item.Forms);
                            var dd = Dialysis[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == input.CenterId).FirstOrDefault();
                            if (dd == null) dd = Dialysis[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == "0").FirstOrDefault();
                            //  dd.centerDialysis = MedicalDrugExtensionStore.Entities.Include(t => t.centerDialysis).FirstOrDefaultAsync(t => t.Id == dd.Id).Result.centerDialysis;
                            item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                            item.MedicalDrugExtension.CenterName = "";
                            if (input.CenterId != "0" && item.MedicalDrugExtension.CenterId == "0")
                                item.MedicalDrugExtension.CenterName = "统一价";
                            i++;
                        }
                    }
                    else
                    {
                        var datas = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Include(t => t.ProcurementUnits).Where(predicate).Skip(0).Take(30).ToArrayAsync();
                        result = Mapper.Map<MedicalItemRecordOutPut[]>(datas);
                        count = result.Length;
                        int i = 0;
                        foreach (var item in result)
                        {
                            // GetManId(datas[i].WareHouseIds, item.WareHouseId);
                            //  GetFromManId(datas[i].Form, item.Forms);
                            var dd = datas[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == input.CenterId).FirstOrDefault();
                            if (dd == null) dd = datas[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == "0").FirstOrDefault();
                            // dd.centerDialysis = MedicalDrugExtensionStore.Entities.Include(t => t.centerDialysis).FirstOrDefaultAsync(t => t.Id == dd.Id).Result.centerDialysis;
                            item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                            item.MedicalDrugExtension.CenterName = "";
                            if (input.CenterId != "0" && item.MedicalDrugExtension.CenterId == "0")
                                item.MedicalDrugExtension.CenterName = "统一价";
                            i++;
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<MedicalItemRecordOutPut[]>(result, count);
            });
        }


        public Task<PageData<MedicalItemRecordOutPut[]>> GetMedQueryableAsync(int MedType)
        {
            return Task.Run(async () =>
            {
                // kk();
                bool IsDel = false;
                MedicalItemRecordOutPut[] result = null;
                int count = 0;
                var data = await MedicalDrugExtensionStore.Entities.Include(t => t.medicalItemRecord).Where(t => t.IsCurrentUse == true && t.RetailPrice > t.SocialSecurityPrice && t.medicalItemRecord.IsDelete == IsDel && t.medicalItemRecord.HiCenterCode + "" != "" && t.medicalItemRecord.MedicalItemType == MedType).Select(t => t.MedicalId).ToListAsync();

                var medData = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Where(t => data.Contains(t.Id)).ToArrayAsync();
                result = Mapper.Map<MedicalItemRecordOutPut[]>(medData);
                count = result.Length;
                int i = 0;
                foreach (var item in result)
                {
                    // GetManId(datas[i].WareHouseIds, item.WareHouseId);
                    //  GetFromManId(medData[i].Form, item.Forms);
                    var dd = medData[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == "0").FirstOrDefault();
                    item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                    i++;
                }
                return new PageData<MedicalItemRecordOutPut[]>(result, count);
            });
        }


        /// <summary>
        /// 档案EXCEL导出
        /// </summary>
        /// <param name="MedType"></param>
        /// <returns></returns>

        public Task<ExportMedicalItem[]> ExportMedicalItemsAsync(int MedType)
        {
            return Task.Run(async () =>
            {
                List<ExportMedicalItem> exports = new List<ExportMedicalItem>();
                var medData = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.dosageForm).Include(t => t.WarehouseCatalog).Where(t => t.MedicalItemType == MedType && t.DataState == 1 && t.IsDelete == false).OrderBy(t => t.WareHouseIds).ToArrayAsync();

                foreach (var item in medData)
                {
                    exports.Add(new ExportMedicalItem()
                    {
                        Brand = item.Brand,
                        CatalogueItem = item.WarehouseCatalog.Name,
                        DoseMin = item.DoseMin,
                        Manufacturer = item.Manufacturer,
                        MedicalItemCode = item.MedicalItemCode,
                        MedicalItemName = item.MedicalItemName,
                        Packaging = item.Packaging,
                        PackSpeUnitCHS = item.PackageUnits != null ? item.PackageUnits.SpeUnitCHS : "",
                        Specifications = item.Specifications,
                        SpecSpeUnitCHS = item.SpecificationsUnits != null ? item.SpecificationsUnits.SpeUnitCHS : "",
                        SpeUnitCHS = item.doseUnits != null ? item.doseUnits.SpeUnitCHS : "",
                        SpeUnitUS = item.doseUnits != null ? item.doseUnits.SpeUnitUS : "",
                        TypeName = item.dosageForm != null ? item.dosageForm.TypeName : "",
                        MedicalThan = item.MedicalThan,

                    });
                }

                return exports.ToArray();
            });

        }

        private void kk()
        {
            var da = MedicalItemRecordStore.Entities.Where(t => t.IsDelete == false && t.MedicalItemType == 5).ToList();
            var userId = _getUserInfo.GetCurrentUserIdAsync().Result;
            foreach (var item in da)
            {
                MedicalDrugExtension medicalDrugExtension = new MedicalDrugExtension();// Mapper.Map<MedicalDrugExtension>(input);
                medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                medicalDrugExtension.DataState = 1;
                medicalDrugExtension.IsCurrentUse = true;
                medicalDrugExtension.MedicalId = item.Id;
                medicalDrugExtension.RetailPrice = decimal.Parse(item.Specifications.Substring(0, item.Specifications.IndexOf("元")));
                medicalDrugExtension.IsCurrentUse = true;
                medicalDrugExtension.Founder = userId;
                medicalDrugExtension.FounderDate = DateTime.Now;
                medicalDrugExtension.Modifier = userId;
                medicalDrugExtension.ModifierDate = DateTime.Now;
                medicalDrugExtension.CenterId = "0";
                MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                _unitOfWork.SaveChanges();

            }
        }

        public Task<bool> HzToPY()
        {
            return Task.Run(async () =>
            {
                var Data = await MedicalItemRecordStore.GetAllAsync();

                foreach (var item in Data)
                {
                    item.Mnemonic = CDGService.Data.Helper.Pinyin.GetInitials(item.MedicalItemName);

                    MedicalItemRecordStore.Update(item);
                }

                await _unitOfWork.SaveChangesAsync();

                return true;
            });

        }


        public Task<bool> SetSpecificationsQuantity()
        {
            return Task.Run(async () =>
            {
                var Data = await MedicalItemRecordStore.Entities.Where(t => t.MedicalItemType == 4).ToArrayAsync();
                try
                {
                    foreach (var item in Data)
                    {
                        //  item.Mnemonic = CDGService.Data.Helper.Pinyin.GetInitials(item.MedicalItemName);
                        // if (item.SpecificationsQuantity <= 1)
                        {
                            int sum = GetSum(item.Packaging.Trim());

                            item.SpecificationsQuantity = sum;



                            MedicalItemRecordStore.Update(item);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception exp)
                {

                    throw;
                }


                return true;
            });
        }

        private int GetSum(string packaging)
        {
            int sum = 1;


            for (int i = 1; i < packaging.Length; i++)
            {

                var value = packaging.Substring(0, i);
                if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[+-]?\d*$"))
                {
                    sum = Convert.ToInt32(value);

                }
                else
                {
                    break;
                }

            }

            return sum;
        }
        /*
         ,List<WarehouseCatalog> catalogs)
        {
            try
            {
                var data = catalogs.Where(t => t.Id == id).FirstOrDefault();
         */

        private void GetchildWareHouse(string[] Id, List<string> listChild, List<WarehouseCatalog> catalogs)
        {
            try
            {//input.Id.Contains(t.Id)
                var data = catalogs.Where(t => Id.Contains(t.ParentId)).ToList();
                if (data != null && data.Count > 0)
                {
                    listChild.InsertRange(0, data.Select(t => t.Id).ToList());

                    GetchildWareHouse(data.Select(t => t.Id).ToArray(), listChild, catalogs);

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }

        }


        /// <summary>
        ///  新增/修改药品档案
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateMedicalItemRecordAsync(MedicalItemRecordInput input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("物品档案信息"));
                if (string.IsNullOrEmpty(input.MedicalItemName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("医疗物品名称")); if (string.IsNullOrEmpty(input.MedicalItemWorkCode))
                    input.MedicalItemWorkCode = "";
                if (!input.MayCharges.HasValue)
                    throw new Exception("档案是否可划价为必选项");
                var caset = await WarehouseCatalogStore.Entities.Where(t => t.IsDelete == false && t.DataState == 1).ToListAsync();
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    if (input.medicalItemTypeEnum != MedicalItemTypeEnum.T)
                    {

                        var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore
                        var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                        var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                        var subData = _purchSubmit.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                        //if (subData == null)
                        //    throw new Exception("您暂时没有维护物资档案的权限"); 

                    }
                    MedicalItemRecord data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        var Specifications = data.Specifications;
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));

                        if (data.MedicalItemCode != input.MedicalItemCode)
                        {
                            throw new Exception("档案档案编码禁止修改");
                        }
                        EntityHelper.CoptyPropertys(input, data);
                        data.ModifierDate = DateTime.Now;
                        data.Modifier = userId;
                        data.SpecificationsUnit = input.Specifications;
                        data.Specifications = input.MinDose;
                        if (input.Forms != null)
                            data.Form = input.Forms.ToString() == "[]" ? "" : input.Forms.ToString();
                        data.Modifier = userId;
                        data.WareHouseIds = input.WareHouseId;
                        List<string> Main = new List<string>();
                        GetManId(data.WareHouseIds, Main, caset);
                        data.WareHouseParentIds = string.Join(",", Main.ToArray());
                        data.ModifierDate = DateTime.Now;
                        if (input.medicalItemTypeEnum == MedicalItemTypeEnum.G)
                            data.Specifications = input.Brand;
                        if (!input.IsSplited)
                        {
                            data.SpecificationsQuantity = 1;
                            data.ProcurementPackage = data.Packaging;//: data.ProcurementPackage; data.ProcurementPackage == "" ?
                            data.ProcurementUnit = data.PackageUnit;//: data.ProcurementUnit;data.ProcurementUnit == "" ?  
                        }
                        else
                        {

                            data.ProcurementPackage = data.Specifications;// : data.ProcurementPackage;data.ProcurementPackage == "" ? 
                            data.ProcurementUnit = data.SpecificationsUnit;//: data.ProcurementUnit;data.ProcurementUnit == "" ?
                        }
                        if (input.IsMedCanal.HasValue)
                            data.IsMedCanal = input.IsMedCanal;
                        else
                            data.IsMedCanal = false;
                        MedicalItemRecordStore.Update(data);
                        var _MedicalDrugExtensione = await MedicalDrugExtensionStore.Entities.Where(t => t.MedicalId == data.Id && t.IsCurrentUse == true && t.CenterId == "0").ToListAsync();

                        if (_MedicalDrugExtensione != null)
                        {
                            _MedicalDrugExtensione.ForEach((t) =>
                            {
                                t.CenterId = "0";
                                t.IsCurrentUse = false;
                            });

                            MedicalDrugExtensionStore.Update(_MedicalDrugExtensione);
                        }

                        MedicalDrugExtension medicalDrugExtension = Mapper.Map<MedicalDrugExtension>(input.MedicalDrugExtension);
                        medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                        medicalDrugExtension.DataState = 1;
                        medicalDrugExtension.CenterId = "0";
                        medicalDrugExtension.IsCurrentUse = true;
                        medicalDrugExtension.MedicalId = data.Id;
                        medicalDrugExtension.FounderDate = DateTime.Now;
                        medicalDrugExtension.ModifierDate = DateTime.Now;
                        medicalDrugExtension.Founder = userId;
                        medicalDrugExtension.Modifier = userId;
                        // medicalDrugExtension.Coefficient = 2;
                        MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                    }


                    else
                    {

                        data = Mapper.Map<MedicalItemRecord>(input);
                        data.Id = Guid.NewGuid().tostring32();
                        List<string> Main = new List<string>();
                        GetManId(data.WareHouseIds, Main, caset);

                        data.WareHouseParentIds = string.Join(",", Main.ToArray());
                        data.Founder = userId;
                        if (input.Forms != null)
                            data.Form = input.Forms.ToString() == "[]" ? "" : input.Forms.ToString();
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.IsDelete = false;
                        data.ApplyState = 1;
                        //if (!input.DoseMin.HasValue)
                        //{
                        //    data.DoseMin = 1;   

                        //}
                        if (!input.SpecificationsQuantity.HasValue)
                        {
                            data.SpecificationsQuantity = 1;
                        }

                        #region
                        // data.DataState = 1;
                        //string MedicalItemCode = "";
                        //bool repeat = true;
                        //switch (input.medicalItemTypeEnum)
                        //{
                        //    case MedicalItemTypeEnum.D:
                        //        data.MedicalItemType = 1;
                        //        while (repeat)
                        //        {
                        //            MedicalItemCode = ValueHelper.TimeNowRandom('D');
                        //            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                        //            if (repeatdata == null)
                        //            {
                        //                data.MedicalItemCode = MedicalItemCode;
                        //                repeat = false;
                        //            }

                        //        }

                        //        break;
                        //    case MedicalItemTypeEnum.U:
                        //        data.MedicalItemType = 2;
                        //        data.MedicalItemCode = ValueHelper.TimeNowRandom('U');

                        //        break;
                        //    case MedicalItemTypeEnum.G:
                        //        data.MedicalItemType = 3;
                        //        data.MedicalItemCode = ValueHelper.TimeNowRandom('G');
                        //        break;
                        //    case MedicalItemTypeEnum.L:
                        //        data.MedicalItemType = 4;
                        //        data.MedicalItemCode = ValueHelper.TimeNowRandom('L');
                        //        break;
                        //    case MedicalItemTypeEnum.T:
                        //        data.MedicalItemType = 5;
                        //        data.MedicalItemCode = ValueHelper.TimeNowRandom('T');
                        //        break;
                        //    case MedicalItemTypeEnum.A:
                        //        data.MedicalItemType = 6;
                        //        data.MedicalItemCode = ValueHelper.TimeNowRandom('A');
                        //        break;
                        //    default:
                        //        break;
                        //}
                        #endregion

                        data.MedicalItemType = (int)input.medicalItemTypeEnum;
                        data.MedicalItemCode = input.MedicalItemWorkCode.Trim() != "" ? input.MedicalItemWorkCode.Trim() : await ReturnMedicalItemCode(input.medicalItemTypeEnum);
                        var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == data.MedicalItemCode);
                        if (repeatdata != null)
                        {
                            throw new Exception("当前档案编码已存在，档案编码禁止重复");
                        }
                        data.SpecificationsUnit = input.Specifications;
                        data.Specifications = input.MinDose;
                        if (input.medicalItemTypeEnum == MedicalItemTypeEnum.G)
                            data.Specifications = input.Brand;
                        data.Mnemonic = data.Mnemonic == "" ? CDGService.Data.Helper.Pinyin.GetInitials(data.MedicalItemName) : data.Mnemonic;//助记码为空?自动生成
                        if (!input.IsSplited)
                        {
                            data.SpecificationsQuantity = 1;
                            data.ProcurementPackage = data.Packaging;//: data.ProcurementPackage; data.ProcurementPackage == "" ?
                            data.ProcurementUnit = data.PackageUnit;//: data.ProcurementUnit;data.ProcurementUnit == "" ?
                        }
                        else
                        {
                            data.ProcurementPackage = data.Specifications;// : data.ProcurementPackage;data.ProcurementPackage == "" ? 
                            data.ProcurementUnit = data.SpecificationsUnit;//: data.ProcurementUnit;data.ProcurementUnit == "" ?
                        }
                        if (input.medicalItemTypeEnum == MedicalItemTypeEnum.G)
                        {
                            data.SpecificationsUnit = data.ProcurementUnit = data.PackageUnit;
                            data.Specifications = data.Packaging = data.ProcurementPackage = data.Brand;
                        }
                        if (input.medicalItemTypeEnum == MedicalItemTypeEnum.T)
                        {
                            data.ProcurementUnit = data.PackageUnit = data.SpecificationsUnit;
                            data.ProcurementPackage = data.Packaging = data.Specifications;
                        }
                        if (input.IsMedCanal.HasValue)
                            data.IsMedCanal = input.IsMedCanal;
                        else
                            data.IsMedCanal = false;

                        if (input.MayCharges.HasValue)
                        {
                            data.MayCharges = input.MayCharges;
                        }
                        else
                        {
                            switch (data.MedicalItemType)
                            {
                                case 1:
                                    data.MayCharges = 1;
                                    break;
                                case 2:
                                    if (data.WareHouseIds == "187b752e52414196b763f91fae53a825")//有价卫材
                                        data.MayCharges = 1;
                                    else
                                        data.MayCharges = 0;
                                    break;

                                case 3:
                                    data.MayCharges = 0;
                                    break;
                                case 4:
                                    data.MayCharges = 0;
                                    break;
                                case 5:
                                    data.MayCharges = 1;
                                    break;
                                default:
                                    break;
                            }
                        }


                        MedicalItemRecordStore.Insert(data);

                        MedicalDrugExtension medicalDrugExtension = Mapper.Map<MedicalDrugExtension>(input.MedicalDrugExtension);
                        medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                        medicalDrugExtension.DataState = 1;
                        medicalDrugExtension.IsCurrentUse = true;
                        medicalDrugExtension.CenterId = "0";
                        medicalDrugExtension.MedicalId = data.Id;
                        medicalDrugExtension.FounderDate = DateTime.Now;
                        medicalDrugExtension.ModifierDate = DateTime.Now;
                        medicalDrugExtension.Founder = userId;
                        medicalDrugExtension.Modifier = userId;
                        medicalDrugExtension.Coefficient = 2;
                        MedicalDrugExtensionStore.Insert(medicalDrugExtension);
                    }

                    _unitOfWork.SaveChanges();


                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, input.medicalItemTypeEnum.ToChinese(),
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }


        //调价

        /// <summary>
        /// 不同透析中心 存在不同价格
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<bool> CrateUpdateMedicalPriceAsync(MedicalDrugExtensionInput input)
        {
            return Task.Run(async () =>
             {
                 bool Falg = true;
                 var userId = await _getUserInfo.GetCurrentUserIdAsync();
                 if (input == null)
                     throw new Exception(MessageFormater.PrameterNeedProvider("请提供详细的物品价格信息"));

                 if (input.CenterId == null)
                     throw new Exception(MessageFormater.PrameterNeedProvider("请提供机构ID,统一价机构ID为0"));
                 MedicalDrugExtension medicalDrugExtension = null;
                 try
                 {
                     if (input.Id + "" != "" || input.Id + "" != "0")
                     {
                         var medicalDrugExtensions = await MedicalDrugExtensionStore.Entities.Where(t => t.CenterId == input.CenterId && t.MedicalId == input.MedicalId).ToListAsync();
                         if (medicalDrugExtensions != null)
                         {
                             //EntityHelper.CoptyPropertys(input, medicalDrugExtension);
                             medicalDrugExtensions.ForEach((t) =>
                     {
                         t.IsCurrentUse = false;
                         t.Modifier = userId;
                         t.ModifierDate = DateTime.Now;
                     });

                             MedicalDrugExtensionStore.Update(medicalDrugExtensions);
                         }
                     }
                     //else
                     //{
                     //    medicalDrugExtension = await MedicalDrugExtensionStore.GetFirstOrDefaultAsync(t => t.CenterId == input.CenterId);
                     medicalDrugExtension = Mapper.Map<MedicalDrugExtension>(input);
                     medicalDrugExtension.Id = Guid.NewGuid().tostring32();
                     // medicalDrugExtension.DataState = 1;
                     medicalDrugExtension.IsCurrentUse = true;
                     medicalDrugExtension.Founder = userId;
                     medicalDrugExtension.FounderDate = DateTime.Now;
                     medicalDrugExtension.Modifier = userId;
                     medicalDrugExtension.ModifierDate = DateTime.Now;
                     medicalDrugExtension.CenterId = input.CenterId + "" == "" ? "0" : input.CenterId;
                     MedicalDrugExtensionStore.Insert(medicalDrugExtension);

                     _unitOfWork.SaveChanges();

                 }
                 catch (Exception exp)
                 {
                     Falg = false;
                     throw new Exception(exp.Message, exp);
                 }

                 // }
                 return Falg;

             });
        }

        /// <summary>
        /// 单个物品各透析中心不同价格列表
        /// </summary>
        /// <param name="MedicalId"></param>
        /// <returns></returns>
        public Task<MedicalDrugExtensionOutput[]> GetMedicalDrugExtensionOutputs(string MedicalId)
        {
            return Task.Run(async () =>
            {
                MedicalDrugExtensionOutput[] medicalDrugExtensionOutputs = null;
                if (MedicalId == "")
                    throw new Exception(MessageFormater.PrameterNeedProvider("请提供物品ID"));
                var medicalDrugExtensions = await MedicalDrugExtensionStore.Entities.Include(t => t.centerDialysis).Include(t => t.medicalItemRecord).Where(t => t.MedicalId == MedicalId && t.IsCurrentUse == true).OrderBy(t => t.CenterId).ToArrayAsync();
                medicalDrugExtensionOutputs = Mapper.Map<MedicalDrugExtensionOutput[]>(medicalDrugExtensions);
                return medicalDrugExtensionOutputs;
            });

        }




        /// <summary>
        /// 系统编码
        /// </summary>
        /// <param name="medicalItemTypeEnum"></param>
        /// <returns></returns>
        public Task<string> ReturnMedicalItemCode(MedicalItemTypeEnum medicalItemTypeEnum)
        {
            return Task.Run(async () =>
            {
                string MedicalItemCode = "";
                bool repeat = true;
                switch (medicalItemTypeEnum)
                {
                    case MedicalItemTypeEnum.D:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('D');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }

                        break;
                    case MedicalItemTypeEnum.U:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('U');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }

                        break;
                    case MedicalItemTypeEnum.G:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('G');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }
                        break;
                    case MedicalItemTypeEnum.L:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('L');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }
                        break;
                    case MedicalItemTypeEnum.T:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('T');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }
                        break;
                    case MedicalItemTypeEnum.A:
                        while (repeat)
                        {
                            MedicalItemCode = ValueHelper.TimeNowRandom('A');
                            var repeatdata = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.MedicalItemCode == MedicalItemCode);
                            if (repeatdata == null)
                                repeat = false;
                        }
                        break;
                    default:
                        break;
                }
                return MedicalItemCode;
            });
        }


        /// <summary>
        ///根据ID删除物品档案-软删
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <returns></returns>
        public Task<bool> DeleteMedicalItemRecordAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore

                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    var subData = _purchSubmit.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                    if (subData == null)
                        throw new Exception("您暂时没有维护物资档案的权限");

                    var data = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    data.ApplyState = 2;
                    MedicalItemRecordStore.Update(data);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        ///启用、禁用
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <returns></returns>
        public Task<bool> StateMedicalItemRecordAsync(string id, int dataState)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.DataState = dataState;
                    if (dataState == 1)
                        data.IsDelete = false;
                    MedicalItemRecordStore.Update(data);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }

        /// <summary>
        ///启用、禁用采购申请
        /// </summary>
        /// <param name="id">物品Id</param>
        /// <param name="ApplyState">状态1允许 2 禁止</param>
        /// <returns></returns>
        public Task<bool> ApplyStateMedicalItemRecordAsync(string id, int ApplyState)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();

                    var user = await _getUserInfo.GetUserAsync(); //根据当前登录权限获取不同级别的数据 PurchaseApproveStore

                    var name = await _getUserInfo.GetCurrentUserEmpNameAsync();
                    var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                    var subData = _purchSubmit.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                    if (subData == null)
                        throw new Exception("您暂时没有维护物资档案的权限");
                    var data = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.ApplyState = ApplyState;


                    MedicalItemRecordStore.Update(data);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }



        public Task<MedicalItemRecordOutPut> GetMedicalItemRecordAsync(string id)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                MedicalItemRecordOutPut result = null;
                int count = 0;
                try
                {
                    Expression<Func<MedicalItemRecord, bool>> predicate = t => t.IsDelete == IsDel;
                    if (id != "")
                        predicate = predicate.And(t => t.Id == id);
                    var datas = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Where(predicate).FirstOrDefaultAsync();
                    if (datas == null)
                    {
                        throw new Exception("未能获取到数据详情");
                    }
                    result = Mapper.Map<MedicalItemRecordOutPut>(datas);
                    var dd = datas.MedicalDrugExtensions.Where(t => t.IsCurrentUse && t.CenterId == "0").FirstOrDefault();
                    result.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                    //   result.WareHouseId.Clear();
                    result.Forms.Clear();
                    GetFromManId(datas.Form, result.Forms);
                    //  GetManId(datas.WareHouseIds, result.WareHouseId);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }
        public void GetManId(string id, List<string> listId, List<WarehouseCatalog> catalogs)
        {
            try
            {
                var data = catalogs.Where(t => t.Id == id).FirstOrDefault();
                if (data != null)
                {
                    listId.Insert(0, data.Id);
                    if (data.ParentId + "" != "")
                    {
                        GetManId(data.ParentId, listId, catalogs);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }

        public void GetFromManId(string id, List<string> listId)
        {
            try
            {
                var data = DosageFormStore.Entities.Where(t => t.Id == id).FirstOrDefault();
                if (data != null)
                {
                    listId.Insert(0, data.Id);
                    if (data.ParentId + "" != "")
                    {
                        GetFromManId(data.ParentId, listId);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message, ex);
            }
        }



        #endregion

        #region 供应商
        /// <summary>
        ///  新增/修改供应商
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateSupplierAsync(SupplierInPut input)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("供应商信息"));
                if (string.IsNullOrEmpty(input.Name))
                    throw new Exception(MessageFormater.PrameterNeedProvider("供应商名称"));

                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    Supplier data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await SupplierStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.MainCategories = string.Join(',', input.MainCategories.OrderBy(t => t));
                        //image
                        string ImagePath = Path.Combine(_documentSetting.ImageRoot, "供应商", Guid.NewGuid().tostring16() + "1.jpg");
                        string ImagePath1 = Path.Combine(_documentSetting.ImageRoot, "供应商", Guid.NewGuid().tostring16() + "2.jpg");
                        if (input.Image1 + "" != "")
                            data.Image1 = ImageHelper.Base64StringToImage1(input.Image1, ImagePath);
                        if (input.Image2 + "" != "")
                            data.Image2 = ImageHelper.Base64StringToImage1(input.Image2, ImagePath1);
                        SupplierStore.Update(data);
                    }
                    else
                    {

                        data = Mapper.Map<Supplier>(input);
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.MainCategories = string.Join(',', input.MainCategories.OrderBy(t => t));
                        data.IsDelete = false;
                        data.DataState = 1;
                        data.Id = Guid.NewGuid().tostring32();
                        data.IsCenterAdd = 0;
                        bool flag = true;
                        while (flag)
                        {
                            data.SysCode = ValueHelper.TimeNowRandom('S');
                            var Codedata = await SupplierStore.GetFirstOrDefaultAsync(t => t.SysCode == data.SysCode);
                            if (Codedata == null)
                                flag = false;
                        }
                        //image
                        if (input.Image1 + "" != "")
                        {
                            string ImagePath = Path.Combine(_documentSetting.ImageRoot, "供应商", Guid.NewGuid().tostring16() + "1.jpg");
                            data.Image1 = ImageHelper.Base64StringToImage1(input.Image1, ImagePath);
                        }
                        if (input.Image2 + "" != "")
                        {
                            string ImagePath1 = Path.Combine(_documentSetting.ImageRoot, "供应商", Guid.NewGuid().tostring16() + "2.jpg");
                            data.Image2 = ImageHelper.Base64StringToImage1(input.Image2, ImagePath1);
                        }
                        SupplierStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id != "" ? LogType.DataUpdate : LogType.DataAdded, "供应商",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        /// <summary>
        ///根据ID删除供应商-软删
        /// </summary>
        /// <param name="id">机构ID</param>
        /// <returns></returns>
        public Task<bool> DeleteSupplierAsync(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var data = await SupplierStore.GetFirstOrDefaultAsync(t => t.Id == id);
                    if (data == null)
                        return true;
                    data.IsDelete = true;
                    data.DataState = 3;
                    SupplierStore.Update(data);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        /// <summary>
        /// 获取供应商列表
        /// </summary>
        public Task<PageData<SupplierOutPut[]>> GetSupplierQueryableAsync(SupplierSearchInput input = null)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                SupplierOutPut[] result = null;
                int count = 0;
                try
                {
                    if (input == null)
                    {
                        input = new SupplierSearchInput();
                    }
                    Expression<Func<Supplier, bool>> predicate = t => t.IsDelete == IsDel;

                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.Name.Contains(input.Name));
                    if (input.Id + "" != "" && input.Id + "" != "0")
                        predicate = predicate.And(t => t.Id == input.Id);
                    if (input.MainCategories != null && input.MainCategories.Length > 0)
                    {
                        //
                        if (!input.MainCategories.Contains(0))
                        {
                            string a = string.Join(',', input.MainCategories.Where(t => t != 0).OrderBy(t => t));
                            foreach (var item in input.MainCategories)
                            {
                                if (a != "")
                                { //var dd = a.Split(',');
                                    predicate = predicate.Or(t => t.MainCategories.Contains(item + ""));
                                    //   predicate = predicate.And(t => dd.Distinct().Contains(t.MainCategories));
                                }

                            }
                        }

                    }
                    if (input.PageNum > 0 && input.PageSize > 0)
                    {
                        var Sum = SupplierStore.Entities.Where(predicate).OrderBy(t => t.SupCode);
                        var Dialysis = await PaginatedList<Supplier>.CreateAsync(Sum, input.PageNum, input.PageSize);
                        // result = Mapper.Map<CenterDialysisOutPut[]>(Dialysis);
                        count = Sum.Count();

                        result = Mapper.Map<SupplierOutPut[]>(Dialysis);
                    }
                    else
                    {
                        var datas = await SupplierStore.Entities.Where(predicate).OrderBy(t => t.SupCode).ToArrayAsync();
                        result = Mapper.Map<SupplierOutPut[]>(datas);
                        count = result.Length;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return new PageData<SupplierOutPut[]>(result, count);
            });
        }

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        public Task<Supplier[]> GetSupplierQueryableAsync()
        {

            return Task.Run(async () =>
            {

                Supplier[] result = null;

                try
                {
                    result = await SupplierStore.Entities.Where(t => t.IsCenterAdd != 1).ToArrayAsync();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }


        #endregion

        #region 药品剂型
        /// <summary> 
        /// 获取剂型列表(树)
        /// </summary>
        public Task<DosageFormOutput[]> GetDosageFormTreeQueryableAsync()
        {
            return Task.Run(async () =>
            {
                bool IsDel = false;
                DosageFormOutput[] result = null;
                string Name = "根节点";
                Expression<Func<DosageForm, bool>> predicate = t => t.IsDelete == IsDel;//ParentId

                DosageFormOutput curItem = new DosageFormOutput() { TypeName = Name, Id = "0", ParentId = "" };
                try
                {
                    var datas = await DosageFormStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<DosageFormOutput[]>(datas);
                    LoopToAppendChildren(result, curItem);
                    // 6.14  9.11 18.32
                    //6.13   9.31 18.52

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return curItem.children.ToArray();

            });
        }

        /// <summary>
        /// 获取剂型列表[]
        /// </summary>
        public Task<DosageFormOutput[]> GetDosageFormQueryableAsync()
        {
            return Task.Run(async () =>
            {
                DosageFormOutput[] result = null;

                try
                {
                    var datas = await DosageFormStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<DosageFormOutput[]>(datas);


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;
            });
        }

        /// <summary>
        /// 中心端获取剂型列表[]
        /// </summary>
        public Task<DosageFormCenterOutput[]> GetCenterDosageFormQueryableAsync()
        {
            return Task.Run(async () =>
            {
                DosageFormCenterOutput[] result = null;

                try
                {
                    var datas = await DosageFormStore.Entities.Where(t => t.IsDelete == false).ToArrayAsync();
                    result = Mapper.Map<DosageFormCenterOutput[]>(datas);


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;
            });
        }


        /// <summary>
        ///  新增/修改剂型目录
        ///  Id为0.视为新增
        /// </summary>
        public Task<bool> CreateUpdateDosageFormAsync(DosageFormInput input)
        {
            return Task.Run(async () =>
            {
                //if (input.Id.HasValue && (input.Id.Value == 1 || input.Id.Value == 2 || input.Id.Value == 3))
                //{
                //    throw new Exception("当前剂型目录不可修改！");
                //}

                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.TypeName))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.TypeName"));

                bool isRe = await GetDosageNameRepeat(input.TypeName, input.TypeCode, input.Id);
                if (isRe)
                    throw new Exception(MessageFormater.PrameterValueIsUsed("剂型名称", input.TypeName));
                try
                {
                    DosageFormOutput ParentMenu = null;
                    if (input.ParentId + "" != "")
                    {
                        ParentMenu = await GetDosageUpMenu(input.ParentId + "");
                    }
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    DosageForm data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await DosageFormStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;

                        if (ParentMenu == null)
                        {

                            data.ParentId = "0";
                        }
                        else
                        {
                            data.ParentId = ParentMenu.Id;

                        }
                        DosageFormStore.Update(data);
                    }
                    else
                    {
                        data = Mapper.Map<DosageForm>(input);
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        data.Id = Guid.NewGuid().tostring32();
                        DosageFormStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "剂型目录",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }


        /// <summary>
        /// 删除剂型目录
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<bool> DelDosageFormAsync(string Id)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    //if (Id == 1 || Id == 2 || Id == 3)
                    //{
                    //    throw new Exception("该剂型目录不可删除！");
                    //}

                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    DosageForm data = null;

                    data = await DosageFormStore.GetFirstOrDefaultAsync(t => t.Id == Id);
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    if (await GetDosageMenus(data.Id))
                        throw new Exception("该剂型目录下存在子目录，不可删除！");
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.IsDelete = true;
                    data.DataState = 3;
                    DosageFormStore.Update(data);

                    await
                        _logManager.WriteLogAsync(LogType.DataDelete, "剂型目录",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }



        /// <summary>
        /// 获取父级菜单
        /// </summary>
        /// <param name="ParentMenuCode"></param>           
        public Task<DosageFormOutput> GetDosageUpMenu(string ParentID)
        {
            return Task.Run(async () =>
            {
                var data = await DosageFormStore.GetFirstOrDefaultAsync(t => t.Id == ParentID && t.IsDelete == false);
                return Mapper.Map<DosageFormOutput>(data);
            });

        }
        /// <summary>
        /// 判断是否有子菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Task<bool> GetDosageMenus(string id)
        {
            return Task.Run(async () =>
            {
                var data = await DosageFormStore.GetFirstOrDefaultAsync(t => t.ParentId == id && t.IsDelete == false);
                if (data == null)
                {
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        private Task<bool> GetDosageNameRepeat(string name, string TypeCode, string id = "0")
        {
            return Task.Run(async () =>
            {
                bool IsRepeat = false;
                try
                {
                    var data = await DosageFormStore.Entities.Where(t => t.TypeName == name && t.TypeCode == TypeCode && t.IsDelete == false).ToArrayAsync();
                    if (data == null)
                        IsRepeat = false;
                    if (data != null && data.Where(t => t.Id != id).Count() > 0)
                        IsRepeat = true;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return IsRepeat;
            });

        }


        public void LoopToAppendChildren(DosageFormOutput[] catelist, DosageFormOutput children)
        {
            var subItems = catelist.Where(x => x.ParentId == children.Id).ToList();
            children.children = new List<DosageFormOutput>();
            // subItems.Sort(t=>t.);
            var query = from items in subItems orderby items.SortNo select items;
            children.children.AddRange(query);
            foreach (var item in subItems)
            {
                LoopToAppendChildren(catelist, item);
            }
        }





        #endregion

        #region 单位

        /// <summary>
        /// 获取单位列表[]
        /// </summary>
        public Task<PageData<MedicalUnitOutput[]>> GetMedicalUnitQueryableAsync(UnitQueryInput input = null)
        {
            return Task.Run(async () =>
            {
                MedicalUnitOutput[] result = null;
                bool IsDel = false;
                int count = 0;
                try
                {
                    if (input == null)
                    {
                        input = new UnitQueryInput() { PageSize = 1000000, PageNum = 1 };
                    }
                    if (input.PageNum <= 0 || input.PageSize <= 0)
                    //throw new Exception(MessageFormater.PrameterNeedProvider("input.PageNum,input.PageSize"));
                    {
                        input.PageNum = 1; input.PageSize = 100000;
                    }
                    Expression<Func<MedicalUnit, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.SpeUnitCHS.Contains(input.Name) || t.SpeUnitUS.Contains(input.Name) || t.UnitCode.Contains(input.Name));
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);
                    if (input.unitType != UnitEnum.AllUnit)
                        predicate = predicate.And(t => t.UnitType == (int)input.unitType || t.UnitType == 4);
                    if (input.DataState > 0)
                        predicate = predicate.And(t => t.DataState == input.DataState);
                    var datas = MedicalUnitStore.Entities.Where(predicate).OrderBy(t => t.SortNum).OrderBy(t => t.UnitType);
                    var unitData = await PaginatedList<MedicalUnit>.CreateAsync(datas, input.PageNum, input.PageSize);

                    result = Mapper.Map<MedicalUnitOutput[]>(unitData);
                    count = datas.Count();

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return new PageData<MedicalUnitOutput[]>(result, count);

            });
        }


        public Task<bool> CreateUpdateMedicalUnitAsync(MedicalUnitInput input)
        {
            return Task.Run(async () =>
            {

                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.SpeUnitCHS))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.SpeUnitCHS"));

                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    MedicalUnit data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        MedicalUnitStore.Update(data);
                    }
                    else
                    {
                        data = Mapper.Map<MedicalUnit>(input);
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;

                        bool flag = true;
                        while (flag)
                        {
                            data.SysCode = ValueHelper.TimeNowRandom('U');
                            var Codedata = await MedicalUnitStore.GetFirstOrDefaultAsync(t => t.SysCode == data.SysCode);
                            if (Codedata == null)
                                flag = false;
                        }
                        data.ModifierDate = DateTime.Now;

                        data.DataState = 1;
                        data.Id = Guid.NewGuid().tostring32();

                        MedicalUnitStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" ? LogType.DataUpdate : LogType.DataAdded, "物品单位",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }

        public Task<bool> EnableUnitAsync(UnitActiveInPut input)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("参数"));
                if (input.Id == null || input.Id.Length < 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Id"));
                if (input.IsActive <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.IsActive"));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();


                    var data = await MedicalUnitStore.Entities.Where(t => t.Id == input.Id).FirstOrDefaultAsync();
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    data.DataState = input.IsActive;

                    MedicalUnitStore.Update(data);


                    _unitOfWork.SaveChanges();

                    result = true;
                    await
                        _logManager.WriteLogAsync(LogType.DataUpdate, input.IsActive == 1 ? "启用单位,ID:" + data.Id : "禁用单位,ID" + data.Id,
                            "");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        #endregion

        #region 用法用量

        /// <summary>
        /// 获取单位列表[]
        /// </summary>
        public Task<PageData<UseWayOutput[]>> GetUseWayQueryableAsync(UseWayQueryInput input = null)
        {
            return Task.Run(async () =>
            {
                UseWayOutput[] result = null;
                int count = 0;
                bool IsDel = false;
                try
                {
                    if (input == null)
                    {
                        input = new UseWayQueryInput() { PageNum = 1, PageSize = 100000 };
                    }
                    if (input.PageNum <= 0 || input.PageSize <= 0)
                    {
                        input.PageNum = 1; input.PageSize = 100000;
                    }
                    Expression<Func<UseWay, bool>> predicate = t => t.IsDelete == IsDel;
                    if (!string.IsNullOrEmpty(input.Name))
                        predicate = predicate.And(t => t.WayDescribe.Contains(input.Name) || t.WayCode.Contains(input.Name));
                    if (input.Id + "" != "")
                        predicate = predicate.And(t => t.Id == input.Id);
                    if (input.WayType != WayEnum.ALL)
                        predicate = predicate.And(t => t.WayType == input.WayType);

                    var datas = UseWayStore.Entities.Where(predicate);
                    var unitData = await PaginatedList<UseWay>.CreateAsync(datas, input.PageNum, input.PageSize);
                    result = Mapper.Map<UseWayOutput[]>(unitData);
                    count = datas.Count();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return new PageData<UseWayOutput[]>(result, count);
                //return result;
            });
        }


        public Task<bool> CreateUpdateUseWayAsync(UseWayInput input)
        {
            return Task.Run(async () =>
            {


                bool result = false;

                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                if (string.IsNullOrEmpty(input.WayCode))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.WayCode"));
                if (string.IsNullOrEmpty(input.WayDescribe))
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.WayDescribe"));

                try
                {


                    var userId = await _getUserInfo.GetCurrentUserIdAsync();
                    UseWay data = null;
                    if (input.Id + "" != "" && input.Id + "" != "0")
                    {
                        data = await UseWayStore.GetFirstOrDefaultAsync(t => t.Id == input.Id);
                        if (data == null)
                            throw new Exception(MessageFormater.PrameterNeedExist("id"));
                        EntityHelper.CoptyProperty(input, data);
                        data.Modifier = userId;
                        data.ModifierDate = DateTime.Now;
                        UseWayStore.Update(data);
                    }
                    else
                    {
                        data = Mapper.Map<UseWay>(input);
                        data.Founder = userId;
                        data.FounderDate = DateTime.Now;
                        data.Modifier = userId;

                        bool flag = true;
                        while (flag)
                        {
                            data.SysCode = ValueHelper.TimeNowRandom('W');
                            var Codedata = await UseWayStore.GetFirstOrDefaultAsync(t => t.SysCode == data.SysCode);
                            if (Codedata == null)
                                flag = false;
                        }
                        data.ModifierDate = DateTime.Now;
                        data.DataState = 1;
                        data.Id = Guid.NewGuid().tostring32();

                        UseWayStore.Insert(data);
                    }
                    _unitOfWork.SaveChanges();
                    result = true;
                    await
                        _logManager.WriteLogAsync(input.Id + "" != "" && input.Id + "" != "0" ? LogType.DataUpdate : LogType.DataAdded, "用法用量",
                            $"编号为{data.Id}");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex + "", ex);
                }
                return result;
            });
        }

        public Task<bool> UseWayEnableUnitAsync(UnitActiveInPut input)
        {

            return Task.Run(async () =>
            {
                bool result = false;
                if (input == null)
                    throw new Exception(MessageFormater.PrameterNeedProvider("参数"));
                if (input.Id == null || input.Id.Length < 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.Id"));
                if (input.IsActive <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input.IsActive"));
                try
                {
                    var userId = await _getUserInfo.GetCurrentUserIdAsync();


                    var data = await UseWayStore.Entities.Where(t => input.Id == t.Id).FirstOrDefaultAsync();
                    if (data == null)
                        throw new Exception(MessageFormater.PrameterNeedExist("id"));
                    data.DataState = input.IsActive;

                    UseWayStore.Update(data);


                    _unitOfWork.SaveChanges();

                    result = true;
                    await
                        _logManager.WriteLogAsync(LogType.DataUpdate, input.IsActive == 1 ? "启用用法用量,ID:" + data.Id : "批量禁用启用用法用量,ID:" + data.Id,
                            "");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return result;
            });
        }

        #endregion

        #region 医保对照

        /// <summary>
        /// 医保对照  //弹子石
        /// </summary>
        public Task<bool> HIComparison(string MedId, int siId, int type)
        {

            //2019年12月11日17:20:26 尹总安排：医保对照只有医保部长、医保专员可更改，直接写死 
            //名称(TYM) 商品名(SPM) 指导价(YLBZDJ) 剂型(JX) 包装数量（BZSL）包装单位(BZDW) 含量（HL） 含量单位（HLDW）  容量（RL） 容量单位（RLDW）  厂家（YCMC）

            //名称 厂家
            return Task.Run(async () =>
            {
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var user = await _getUserInfo.GetUserAsync();
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var subData = _orderPricingRole.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                if (subData == null)
                    throw new Exception("您暂时没有医保对照的权限");
                SI_YPMLS isData = new SI_YPMLS();
                SI_ZLXMS sI_ZLXM = new SI_ZLXMS();
                var MeiData = await MedicalItemRecordStore.Entities.Include(t => t.MedicalDrugExtensions).Where(t => t.Id == MedId).FirstOrDefaultAsync();
                string OldHiCode = "";
                if (type == 1)
                {
                    isData = await SI_YPMLSStore.GetFirstOrDefaultAsync(t => t.id == siId);
                    if (MeiData != null)
                    {
                        OldHiCode = MeiData.HiCenterCode;
                        MeiData.HiCenterCode = isData.YPLSH;
                        MeiData.HiCenterName = isData.TYM;
                        MeiData.HiLevel = Convert.ToInt32(isData.YLFYDJ);
                        MeiData.NationItemCode = isData.GJYPDM;
                        MeiData.MedicalDrugExtensions.ForEach(t => t.SocialSecurityPrice = isData.YLBZDJ);
                        MedicalItemRecordStore.Update(MeiData);
                        _unitOfWork.SaveChanges();
                        await
                      _logManager.WriteLogAsync(LogType.DataUpdate, "医保对照",
                          $"药品对码：药品ID:{MedId},医保编码：{isData.YPLSH},原对照码：{OldHiCode}");
                    }
                }
                else
                {
                    sI_ZLXM = await SI_ZLXMStore.GetFirstOrDefaultAsync(t => t.id == siId);
                    if (MeiData != null)
                    {
                        OldHiCode = MeiData.HiCenterCode;
                        MeiData.HiCenterCode = sI_ZLXM.XMLSH;
                        MeiData.HiCenterName = sI_ZLXM.XMMC;
                        MeiData.HiLevel = Convert.ToInt32(sI_ZLXM.YLFYDJ);
                        MeiData.NationItemCode = sI_ZLXM.GJXMDM;
                        MeiData.MedicalDrugExtensions.ForEach(t => t.SocialSecurityPrice = sI_ZLXM.YLBZJ);
                        MedicalItemRecordStore.Update(MeiData);
                        _unitOfWork.SaveChanges();
                        if (MeiData.MedicalItemType == 2)
                            await _logManager.WriteLogAsync(LogType.DataUpdate, "其他对照",
                             $"耗材对码：档案ID:{MedId},医保编码：{sI_ZLXM.XMLSH},原对照码：{OldHiCode}");
                        else
                            await _logManager.WriteLogAsync(LogType.DataUpdate, "其他对照",
                       $"诊疗项目对码：档案ID:{MedId},医保编码：{sI_ZLXM.XMLSH},原对照码：{OldHiCode}");
                    }
                }
                return true;
            });

        }

        /// <summary>
        /// 医保中心药品档案
        /// </summary>
        public Task<yibao> SI_YPMLAsync(SIContrast input)
        {
            //SI_YPMLS

            return Task.Run(async () =>
            {
                bool IsDel = false;
                SI_YPMLS[] result = null;
                MedicalItemRecordOutPut[] result1 = null;
                Expression<Func<SI_YPMLS, bool>> predicate = t => t.id > 0;
                MedicalItemRecordSearchInput searchInput = new MedicalItemRecordSearchInput();
                int count = 0;
                try
                {
                    if (input != null && input.si_YpmlQueryInput != null)
                    {
                        input.si_YpmlQueryInput.YpName = input.si_YpmlQueryInput.YpName + "" == "" ? input.MedicalItemInput.YpName : input.si_YpmlQueryInput.YpName;
                        if (!string.IsNullOrEmpty(input.si_YpmlQueryInput.YpName))
                            predicate = predicate.And(t => t.TYM.Contains(input.si_YpmlQueryInput.YpName) || t.TYMZJM.Contains(input.si_YpmlQueryInput.YpName));

                        //if (input.si_YpmlQueryInput.GoodsName + "" != "")
                        //    predicate = predicate.And(t => t.SPM.Contains(input.si_YpmlQueryInput.GoodsName) || t.SPMZJM.Contains(input.si_YpmlQueryInput.GoodsName));
                        if (input.si_YpmlQueryInput.Brand + "" != "")
                            predicate = predicate.And(t => t.SPM.Contains(input.si_YpmlQueryInput.Brand) || t.SPMZJM.Contains(input.si_YpmlQueryInput.Brand));
                        if (input.si_YpmlQueryInput.SalePrice > 0)
                            predicate = predicate.And(t => t.YLBZDJ <= input.si_YpmlQueryInput.SalePrice);
                        if (input.si_YpmlQueryInput.Manufacturer + "" != "")
                            predicate = predicate.And(t => t.YCMC.Contains(input.si_YpmlQueryInput.Manufacturer));
                        if (input.si_YpmlQueryInput.level > 0)
                        {
                            predicate = predicate.And(t => t.YLFYDJ == input.si_YpmlQueryInput.level + "");

                        }
                        //if (input.Packaging != "")
                        //    predicate = predicate.And(t => t.BZSL.Contains(input.Packaging));
                    }
                    result = await SI_YPMLSStore.Entities.Where(predicate).Skip(0).Take(450).ToArrayAsync();
                    //  result = Mapper.Map<MedicalItemRecordOutPut[]>(datas);
                    //本地

                    //  searchInput = new MedicalItemRecordSearchInput() { Brand = input.MedicalItemInput.Brand, MedicalItemType = 1, Manufacturer = input.MedicalItemInput.Manufacturer, Name = input.MedicalItemInput.YpName, PageNum = 1, PageSize = 100 };
                    Expression<Func<MedicalItemRecord, bool>> Itempredicate = t => t.IsDelete == IsDel && t.MedicalItemType == 1;
                    if (input != null && input.MedicalItemInput != null)
                    {
                        if (input.MedicalItemInput.YpName + "" != "")
                            Itempredicate = Itempredicate.And(t => t.MedicalItemName.Contains(input.MedicalItemInput.YpName));
                        if (input.MedicalItemInput.Brand + "" != "")
                            Itempredicate = Itempredicate.And(t => t.GoodsName.Contains(input.MedicalItemInput.Brand));
                        if (input.MedicalItemInput.Manufacturer + "" != "")
                            Itempredicate = Itempredicate.And(t => t.Manufacturer.Contains(input.MedicalItemInput.Manufacturer));

                        if (input.MedicalItemInput.isContrast == true)
                        {
                            Itempredicate = Itempredicate.And(t => t.HiCenterCode + "" != "");
                        }
                    }
                    var BDItems = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Where(Itempredicate).Skip(0).Take(80).ToArrayAsync();
                    result1 = Mapper.Map<MedicalItemRecordOutPut[]>(BDItems);
                    count = result1.Length;
                    int i = 0;
                    foreach (var item in result1)
                    {
                        // GetManId(datas[i].WareHouseIds, item.WareHouseId);
                        //GetFromManId(result1[i].Form, item.Forms);
                        var dd = BDItems[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse).FirstOrDefault();
                        item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                        i++;
                        result1 = result1.OrderBy(t => t.HiCenterCode).ToArray();
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                yibao yb = new yibao() { medical = result1, SIypmls = result };

                return yb;
            });



        }

        //诊疗项目

        public Task<ZLXM> SI_ZLXM(Si_ZLXMQueryInput input)
        {
            return Task.Run(async () =>
            {
                ZLXM zlXM = new ZLXM();
                bool IsDel = false;
                SI_ZLXMS[] result = null;
                MedicalItemRecordOutPut[] result1 = null;
                Expression<Func<SI_ZLXMS, bool>> predicate = t => t.id > 0;
                MedicalItemRecordSearchInput searchInput = new MedicalItemRecordSearchInput();
                int count = 0;
                try
                {
                    input.ZXItemName = input.ZXItemName + "" == "" ? input.BDItemName : input.ZXItemName;
                    if (input != null && input.ZXItemName + "" != "")
                    {
                        predicate = predicate.And(t => t.XMMC.Contains(input.ZXItemName));
                    }
                    if (input != null && input.bz + "" != "")
                    {
                        predicate = predicate.And(t => t.BZ.Contains(input.bz));
                    }
                    result = await SI_ZLXMStore.Entities.Where(predicate).Skip(0).Take(550).OrderBy(t => t.XMMC).ToArrayAsync();


                    //Expression<Func<SI_ZLXMS, bool>> predicate1 = t => t.id > 0;
                    //if (input != null && input.ZXItemName + "" != "")
                    //{
                    //    predicate1 = predicate1.And(t => t.XMMC.Contains(input.ZXItemName));
                    //}
                    //if (input != null && input.bz + "" != "")
                    //{
                    //    predicate1 = predicate1.And(t => t.BZ.Contains(input.bz));
                    //}
                    //var result2 = await SI_ZLXMStore.Entities.Where(predicate1).Skip(0).Take(150).OrderBy(t => t.XMMC).ToListAsync();

                    //if (result == null || result.Length<1)
                    //{
                    //    Expression<Func<SI_ZLXMS, bool>> predicate1 = t => t.id > 0;
                    //    if (input != null && input.ZXItemName + "" != "")
                    //    {
                    //        predicate1 = predicate1.And(t => t.XMMC.Contains (input.ZXItemName));
                    //    }
                    //    if (input != null && input.bz + "" != "")
                    //    {
                    //        predicate1 = predicate1.And(t => t.BZ.Contains(input.bz));
                    //    }
                    //    result = await SI_ZLXMStore.Entities.Where(predicate1).Skip(0).Take(150).ToArrayAsync();
                    //}
                    //  result = Mapper.Map<MedicalItemRecordOutPut[]>(datas);
                    //本地

                    searchInput = new MedicalItemRecordSearchInput() { MedicalItemType = 5, Name = input.BDItemName, Manufacturer = input.Manufacturer };

                    var BDItems = await this.GetMedicalItemRecordQueryableAsync(searchInput);

                    searchInput = new MedicalItemRecordSearchInput() { MedicalItemType = 2, Name = input.BDItemName, Manufacturer = input.Manufacturer };

                    var BDItems1 = await this.GetMedicalItemRecordQueryableAsync(searchInput);

                    var restData = BDItems.Result.ToList();
                    restData.AddRange(BDItems1.Result.ToList());
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i].YLGGXMBJ == "2")
                        {
                            result[i].XMMC = result[i].XMMC + "2";
                        }
                        if (result[i].YLGGXMBJ == "1")
                        {
                            result[i].XMMC = result[i].XMMC + "1";
                        }

                    }
                    zlXM = new ZLXM() { medical = restData.ToArray(), sI_ZLXMS = result };
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                return zlXM;

            });
        }
        //已对照项目
        public Task<PageData<object>> YDZSI_YPMLAsync(Si_YpmlQueryInput input)
        {
            //SI_YPMLS

            return Task.Run(async () =>
            {
                bool IsDel = false;
                SI_YPMLS[] result = null;
                SI_ZLXMS[] sI_ZLXMs = null;
                MedicalItemRecordOutPut[] result1 = null;
                Expression<Func<SI_YPMLS, bool>> predicate = t => t.id > 0;
                MedicalItemRecordSearchInput searchInput = new MedicalItemRecordSearchInput();
                int count = 0;
                try
                {
                    if (input == null)
                        throw new Exception("请提供查询参数");
                    List<int> Typelist = new List<int>();
                    if (input.ItemType == 1) Typelist.Add(1);
                    else
                    { Typelist.Add(2); Typelist.Add(5); };

                    Expression<Func<MedicalItemRecord, bool>> Itempredicate = t => t.IsDelete == IsDel && Typelist.Contains(t.MedicalItemType) && (t.HiCenterCode != null && t.HiCenterCode != "");
                    if (input != null)
                    {
                        if (input.YpName + "" != "")
                            Itempredicate = Itempredicate.And(t => t.MedicalItemName.Contains(input.YpName));
                        if (input.Brand + "" != "")
                            Itempredicate = Itempredicate.And(t => t.GoodsName.Contains(input.Brand));
                        if (input.Manufacturer + "" != "")
                            Itempredicate = Itempredicate.And(t => t.Manufacturer.Contains(input.Manufacturer));
                    }
                    //本地 
                    var datas = MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Where(Itempredicate);

                    var MedicaltData = await PaginatedList<MedicalItemRecord>.CreateAsync(datas, input.PageNum, input.PageSize);

                    result1 = Mapper.Map<MedicalItemRecordOutPut[]>(MedicaltData);
                    count = MedicaltData.Count;
                    List<string> hicode = MedicaltData.Select(t => t.NationItemCode).ToList();
                    if (input.ItemType == 1)
                        result = await SI_YPMLSStore.Entities.Where(t => hicode.Contains(t.GJYPDM)).ToArrayAsync();
                    else
                        sI_ZLXMs = await SI_ZLXMStore.Entities.Where(t => hicode.Contains(t.GJXMDM)).ToArrayAsync();
                    int i = 0;
                    foreach (var item in result1)
                    {
                        // GetManId(datas[i].WareHouseIds, item.WareHouseId);
                        // GetFromManId(datas[i].Form, item.Forms);
                        var dd = MedicaltData.Where(t => t.Id == item.Id).First().MedicalDrugExtensions.Where(t => t.IsCurrentUse).FirstOrDefault();
                        item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                        i++;
                    }

                    // var BDItems = await this.GetMedicalItemRecordQueryableAsync(searchInput);
                    // result1 = BDItems.Result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                object OBJ = new object();
                if (input.ItemType == 1)
                    OBJ = new yibao() { medical = result1, SIypmls = result };
                else

                    OBJ = new ZLXM() { medical = result1, sI_ZLXMS = sI_ZLXMs };
                return new PageData<object>(OBJ, count);
            });
        }

        //取消对照
        public Task<bool> QXDZSI_YPMLAsync(string ItemId)
        {
            return Task.Run(async () =>
            {
                var MeiData = await MedicalItemRecordStore.GetFirstOrDefaultAsync(t => t.Id == ItemId);
                if (MeiData != null)
                {
                    string OldHiCode = MeiData.HiCenterCode;
                    MeiData.HiCenterCode = null;
                    MeiData.HiLevel = null;
                    MeiData.HiCenterName = null;
                    MeiData.NationItemCode = null;
                    MedicalItemRecordStore.Update(MeiData);
                    _unitOfWork.SaveChanges();
                    await _logManager.WriteLogAsync(LogType.DataUpdate, "取消对照",
                       $"档案取消对码：档案ID:{MeiData.Id},原对照码：{OldHiCode}");
                    return true;
                }
                else
                {
                    throw new Exception("未能获取到项目信息");
                }

            });

        }



        #region  2021年7月28日10:41:44   新医保对照

        // 疑似失效医保编码  

        //
        public Task<MedicalItemRecord[]> GetMedicalSICodeOutAsync()
        {
            return Task.Run(async () =>
            {
                string QuerySQL = "SELECT  * from MedicalItemRecords mir  where  NationItemCode  in (select NationItemCode from SICodeOutServerInfo where DataState =1)";
                var data = _unitOfWork.SqlQuery<MedicalItemRecord>(QuerySQL).ToList();

                return data.ToArray();
            });

        }
        //
        //public Task<bool> UpdateMedicalSICodeOutAsync(string NationItemCode)
        //{
        //    return Task.Run(async () =>
        //    {


        //        string QuerySQL = $"Update   SICodeOutServerInfo set    DataState =2  where NationItemCode = '{NationItemCode}'";
        //        var data = _unitOfWork.SqlQuery<MedicalItemRecord>(QuerySQL).ToList();

        //        return data.ToArray();
        //    });

        //}


        //药品

        public Task<yibaoNew> SI_YPAsync(SIContrast input)
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.gjypdm))
                {
                    await _dataReceive.GetSIBaseDataAsync(input.si_YpmlQueryInput.gjypdm.Trim());
                }
                //if (string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.YpName))
                //    throw new Exception("医保中心目录名称非空");

                //if (string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.Manufacturer))
                //    throw new Exception("医保中心目录厂家非空");
                bool IsDel = false;
                string SqlWhere = "";
                if (input != null)
                {
                    if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.YpName))
                    {
                        SqlWhere += $"and hc.ZCMC like '%{input.si_YpmlQueryInput.YpName.Trim()}%' ";

                    }
                    if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.Manufacturer))
                    {
                        SqlWhere += $"and hc.SCQYMC like '%{input.si_YpmlQueryInput.Manufacturer.Trim()}%'";

                    }
                    if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.Brand))
                    {
                        SqlWhere += $"and hc.YPSPM like '%{input.si_YpmlQueryInput.Brand.Trim()}%'";

                    }
                    if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.gjypdm))
                    {
                        SqlWhere += $"and hc.YLMLBM like '%{input.si_YpmlQueryInput.gjypdm.Trim()}%'";

                    }

                }
                string QuerySQL = @"
	select  hc.ID, hc.ZCMC as tym ,hc.YPSPM as spm ,hc.YPTYM, hc.YPJX as jx, hc.ZXBZSL as bzsl,hc.ZXBZDW as bzdw,hc.ZXZJDW,hc.YPGG as hl, hc.SCQYMC as ycmc ,hc.YLMLBM as gjypdm, cp.hilist_pric_uplmt_amt as ylbzdj, own.selfpay_prop as ylzfbl, cat.chrgitm_lv as ylfydj  , cp.hilist_lmtpric_type as  lmttype,own.selfpay_prop_psn_type as protype  ,hc.ZHB ,hc.XZSYBZ ,hc.XZSYFW,hc.PZWH ,cp.updt_time as bgsj ,cat.memo as bz ,hc.BZCZ,hc.BZCZMC from   SI_YP as hc 
				left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM and  ((cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
				left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM  and((own.selfpay_prop_psn_type = '9901' or own.selfpay_prop_psn_type is null) and own.enddate is null)
				left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null 
				 where  hc.YXBZ =1  and hc.BBHMC = ( select MAX(BBHMC) from SI_YP  where SI_YP.YLMLBM =hc.YLMLBM)
                " + SqlWhere;
                //医保中心数据 13036374133
                var data = new List<Si_YPOutPut>();

                if (!string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.YpName) || !string.IsNullOrWhiteSpace(input.si_YpmlQueryInput.gjypdm))
                    data = _unitOfWork.SqlQuery<Si_YPOutPut>(QuerySQL).ToList();
                //MedicalItemRecordOutPut[] result1 = null;
                //Expression<Func<MedicalItemRecord, bool>> Itempredicate = t => t.IsDelete == IsDel && t.MedicalItemType == 1;
                //if (input != null && input.MedicalItemInput != null)
                //{
                //    if (input.MedicalItemInput.nationItemCode + "" != "")
                //        Itempredicate = Itempredicate.And(t => t.NationItemCode.Contains(input.MedicalItemInput.nationItemCode.Trim()));
                //    if (input.MedicalItemInput.YpName + "" != "")
                //        Itempredicate = Itempredicate.And(t => t.MedicalItemName.Contains(input.MedicalItemInput.YpName.Trim()));
                //    if (input.MedicalItemInput.Brand + "" != "")
                //        Itempredicate = Itempredicate.And(t => t.GoodsName.Contains(input.MedicalItemInput.Brand.Trim()));
                //    if (input.MedicalItemInput.Manufacturer + "" != "")
                //        Itempredicate = Itempredicate.And(t => t.Manufacturer.Contains(input.MedicalItemInput.Manufacturer.Trim()));

                //    if (input.MedicalItemInput.isContrast == true)
                //    {
                //        Itempredicate = Itempredicate.And(t => t.NationItemCode + "" != "");
                //    }
                //}
                //var BDItems = await MedicalItemRecordStore.Entities.Include(t => t.SpecificationsUnits).Include(t => t.PackageUnits).Include(t => t.doseUnits).Include(t => t.supplier).Include(t => t.MedicalDrugExtensions).Include(t => t.dosageForm).Where(Itempredicate).Skip(0).Take(80).ToArrayAsync();
                //result1 = Mapper.Map<MedicalItemRecordOutPut[]>(BDItems);


                string BDSQLWhere = "";
                if (input != null && input.MedicalItemInput != null)
                {
                    if (input.MedicalItemInput.nationItemCode + "" != "")
                        BDSQLWhere += $" and a.NationItemCode like '%{input.MedicalItemInput.nationItemCode.Trim()}%'"; // Itempredicate.And(t => t.NationItemCode.Contains(input.MedicalItemInput.nationItemCode.Trim()));
                    if (input.MedicalItemInput.YpName + "" != "")
                        BDSQLWhere += $" and a.MedicalItemName like '%{input.MedicalItemInput.YpName.Trim()}%'"; //Itempredicate.And(t => t.MedicalItemName.Contains(input.MedicalItemInput.YpName.Trim()));
                    if (input.MedicalItemInput.Brand + "" != "")
                        BDSQLWhere += $" and a.Brand like '%{input.MedicalItemInput.Brand.Trim()}%'";
                    if (input.MedicalItemInput.Manufacturer + "" != "")
                        BDSQLWhere += $" and a.Manufacturer like '%{input.MedicalItemInput.Manufacturer.Trim()}%'";

                }
                string BDSQL = $@" select a.*  ,b.chrgitm_lv,c.hilist_pric_uplmt_amt ,d.RetailPrice as PurchasingPrice ,e.TypeName  as formValue ,f.SpeUnitUS as doseUnitValue ,a.Specifications as minDose
   from MedicalItemRecords    as a 
left join SI_YBcatalog as  b on b.hilist_code = a.NationItemCode  and b.enddate is   null
left join SI_CheckPriceInfo as c on c.hilist_code = a.NationItemCode and c.hilist_lmtpric_type='902' and c.enddate is null
left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.CenterId = '0'
left join DosageForms as e on e.Id = a.Form
left join MedicalUnits as f on f.Id = a.DoseUnit
  where a.MedicalItemType =1 and a.DataState=1 {BDSQLWhere} ";
                var bddata = _unitOfWork.SqlQuery<MedicalItemOutPut>(BDSQL).ToList();


                yibaoNew yb = new yibaoNew() { medical = bddata.ToArray(), SIypmls = data.ToArray() };

                return yb;

            });
        }




        //耗材 
        public Task<HC_OutPut> SI_HCAsync(Si_ZLXMQueryInput input)
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(input.gjxmdm))
                {
                    await _dataReceive.GetSIBaseDataAsync(input.gjxmdm);
                }
                //if (string.IsNullOrWhiteSpace(input.BDItemName))
                //    throw new Exception("本地项目名称非空");
                //if (string.IsNullOrWhiteSpace(input.ZXItemName))
                //    throw new Exception("医保中心目录名称非空");

                //if (string.IsNullOrWhiteSpace(input.bz))
                //    throw new Exception("医保中心目录厂家非空");
                var HC_OutPut = new HC_OutPut();
                //if (string.IsNullOrWhiteSpace(input.ZXItemName)) input.ZXItemName = input.BDItemName;
                string SqlWhere = "";

                if (!string.IsNullOrWhiteSpace(input.ZXItemName))
                    SqlWhere += $" and (hc.HCMC like '%{input.ZXItemName}%'  or cat.hilist_name like '%{input.ZXItemName}%' ) ";
                //if (!string.IsNullOrWhiteSpace(input.Manufacturer))
                //    SqlWhere += $"and hc.SCQYMC like '%{input.Manufacturer}%'";
                if (!string.IsNullOrWhiteSpace(input.bz))
                    SqlWhere += $" and hc.SCQYMC like '%{input.bz}%'";
                if (!string.IsNullOrWhiteSpace(input.gjxmdm))
                    SqlWhere += $" and hc.YLMLBM like '%{input.gjxmdm}%'";

                string QuerySQL = @" 
  select hc.Id, hc.YLMLBM as gjxmdm,cat.hilist_name as xmmc,hc.GG as gg,hc.GGXH,hc.HCFL,hc.SCQYMC as bz,cat.memo , hc.YXBS,hc.BBH,cp.hilist_pric_uplmt_amt as ylzdj, own.selfpay_prop as txbl, cat.chrgitm_lv as ylfydj , cp.hilist_lmtpric_type as  lmttype ,cp.updt_time as bgsj , hc.HCFL as hcfl from SI_HC as hc 
  left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM   and  ((cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
  left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM and ((own.selfpay_prop_psn_type = '9901' or own.selfpay_prop_psn_type is null) and own.enddate is null)
  left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null   WHERE hc.YXBS != '0' and   hc.BBH = ( select MAX(BBH) from SI_HC  where SI_HC.YLMLBM =hc.YLMLBM) " + SqlWhere;
                //医保中心数据
                var data = new List<SI_HCOutPut>();
                if (!string.IsNullOrWhiteSpace(input.ZXItemName) || !string.IsNullOrWhiteSpace(input.gjxmdm))
                    data = _unitOfWork.SqlQuery<SI_HCOutPut>(QuerySQL).ToList();
                data.ForEach(t => t.bz = t.bz.Replace("null", ""));



                ////本地

                //var searchInput = new MedicalItemRecordSearchInput() { MedicalItemType = 2, Name = input.BDItemName, Manufacturer = input.Manufacturer, nationItemCode = input.nationItemCode };



                //var BDItems = await this.GetMedicalItemRecordQueryableAsync(searchInput);



                string BDSQLWhere = "";
                if (input != null && input.BDItemName != null)
                {
                    if (input.nationItemCode + "" != "")
                        BDSQLWhere += $" and a.NationItemCode like '%{input.nationItemCode.Trim()}%'"; // Itempredicate.And(t => t.NationItemCode.Contains(input.MedicalItemInput.nationItemCode.Trim()));
                    if (input.BDItemName + "" != "")
                        BDSQLWhere += $" and a.MedicalItemName like '%{input.BDItemName.Trim()}%'"; //Itempredicate.And(t => t.MedicalItemName.Contains(input.MedicalItemInput.YpName.Trim()));
                    //if (input.MedicalItemInput.Brand + "" != "")
                    //    BDSQLWhere += $" and a.Brand like '%{input.MedicalItemInput.Brand.Trim()}%'";
                    if (input.Manufacturer + "" != "")
                        BDSQLWhere += $" and a.Manufacturer like '%{input.Manufacturer.Trim()}%'";

                }
                string BDSQL = $@" select a.*  ,b.chrgitm_lv,c.hilist_pric_uplmt_amt ,d.RetailPrice as PurchasingPrice ,e.TypeName  as formValue ,f.SpeUnitUS as doseUnitValue ,a.Specifications as minDose
   from MedicalItemRecords    as a 
left join SI_YBcatalog as  b on b.hilist_code = a.NationItemCode  and b.enddate is   null
left join SI_CheckPriceInfo as c on c.hilist_code = a.NationItemCode and c.hilist_lmtpric_type='902' and c.enddate is null
left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.CenterId = '0'
left join DosageForms as e on e.Id = a.Form
left join MedicalUnits as f on f.Id = a.DoseUnit
  where a.MedicalItemType =2 and a.DataState=1 {BDSQLWhere} ";
                var bddata = _unitOfWork.SqlQuery<MedicalItemOutPut>(BDSQL).ToList();

                HC_OutPut.medical = bddata.ToArray();
                HC_OutPut.sI_HCOutPuts = data.ToArray();
                return HC_OutPut;
            });
        }


        //诊疗项目

        public Task<HC_FWXM> SI_ZLXMAsync(Si_ZLXMQueryInput input)
        {
            // _unitOfWork.
            return Task.Run(async () =>

            {

                if (!string.IsNullOrWhiteSpace(input.gjxmdm))
                {
                    await _dataReceive.GetSIBaseDataAsync(input.gjxmdm);
                }


                var HC_OutPut = new HC_FWXM();

                string SqlWhere = "";

                //if (string.IsNullOrWhiteSpace(input.BDItemName))
                //    throw new Exception("本地项目名称非空");
                //if (string.IsNullOrWhiteSpace(input.ZXItemName))
                //    throw new Exception("医保中心目录名称非空");

                //if (string.IsNullOrWhiteSpace(input.ZXItemName))
                //    input.ZXItemName = input.BDItemName;

                if (!string.IsNullOrWhiteSpace(input.ZXItemName))
                    SqlWhere += $"and hc.YLFWXMMC like '%{input.ZXItemName}%'";
                if (!string.IsNullOrWhiteSpace(input.Manufacturer))
                    SqlWhere += $"and hc.ZLXMLH like '%{input.Manufacturer}%'";
                if (!string.IsNullOrWhiteSpace(input.bz))
                    SqlWhere += $"and hc.BZ like '%{input.bz}%'";
                if (!string.IsNullOrWhiteSpace(input.gjxmdm))
                    SqlWhere += $"and hc.LYMLBM like '%{input.gjxmdm}%'";

                string QuerySQL = @"   
                 select hc.Id, hc.LYMLBM as gjxmdm,hc.JJDW as jjdw ,cat.memo as bz ,hc.YLFWXMMC as xmmc ,cp.hilist_pric_uplmt_amt as ylbzj, own.selfpay_prop as txbl, cat.chrgitm_lv as ylfydj,cp.hilist_lmtpric_type as  lmttype,own.selfpay_prop_psn_type as protype ,hc.ZLXMLH as zlxmhl ,cp.updt_time as bgsj from SI_FWXM as hc 
 left join SI_CheckPriceInfo as cp on  cp.hilist_code  = hc.LYMLBM  and ( (cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
 left join SI_OwnExpenseInfo as own on  own.hilist_code = hc.LYMLBM  and((own.selfpay_prop_psn_type = '9901' or own.selfpay_prop_psn_type is null) and own.enddate is null)
 left join SI_YBcatalog as cat on    cat.hilist_code  = hc.LYMLBM  and cat.enddate is null 
 where  hc.YXBZ  ='1'    and hc.BBMC = ( select MAX(BBMC) from SI_FWXM  where SI_FWXM.LYMLBM =hc.LYMLBM)

                " + SqlWhere;
                //医保中心数据
                var data = new List<SI_FWXMOutPut>();
                if (!string.IsNullOrWhiteSpace(input.ZXItemName) || !string.IsNullOrWhiteSpace(input.gjxmdm))
                    data = _unitOfWork.SqlQuery<SI_FWXMOutPut>(QuerySQL).ToList();

                //本地

                string BDSQLWhere = "";
                if (input != null && input.BDItemName != null)
                {
                    if (input.nationItemCode + "" != "")
                        BDSQLWhere += $" and a.NationItemCode like '%{input.nationItemCode.Trim()}%'"; // Itempredicate.And(t => t.NationItemCode.Contains(input.MedicalItemInput.nationItemCode.Trim()));
                    if (input.BDItemName + "" != "")
                        BDSQLWhere += $" and a.MedicalItemName like '%{input.BDItemName.Trim()}%'"; //Itempredicate.And(t => t.MedicalItemName.Contains(input.MedicalItemInput.YpName.Trim()));
                    //if (input.MedicalItemInput.Brand + "" != "")
                    //    BDSQLWhere += $" and a.Brand like '%{input.MedicalItemInput.Brand.Trim()}%'";
                    if (input.Manufacturer + "" != "")
                        BDSQLWhere += $" and a.Manufacturer like '%{input.Manufacturer.Trim()}%'";

                }
                string BDSQL = $@" select a.*  ,b.chrgitm_lv,c.hilist_pric_uplmt_amt ,d.RetailPrice as PurchasingPrice ,e.TypeName  as formValue ,f.SpeUnitUS as doseUnitValue ,a.Specifications as minDose
   from MedicalItemRecords    as a 
left join SI_YBcatalog as  b on b.hilist_code = a.NationItemCode  and b.enddate is   null
left join SI_CheckPriceInfo as c on c.hilist_code = a.NationItemCode and c.hilist_lmtpric_type='902' and c.enddate is null
left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.CenterId = '0'
left join DosageForms as e on e.Id = a.Form
left join MedicalUnits as f on f.Id = a.DoseUnit
  where a.MedicalItemType =5 and a.DataState=1 {BDSQLWhere} ";
                var bddata = _unitOfWork.SqlQuery<MedicalItemOutPut>(BDSQL).ToList();
                HC_OutPut.medical = bddata.ToArray();
                HC_OutPut.sI_HCOutPuts = data.ToArray();


                return HC_OutPut;
            });
        }


        /// <summary>
        /// 医保对照  
        /// </summary>
        public Task<bool> NewHIComparison(string MedId, int siId, int type)
        {

            //2019年12月11日17:20:26 尹总安排：医保对照只有医保部长、医保专员可更改，直接写死 

            return Task.Run(async () =>
            {
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                var user = await _getUserInfo.GetUserAsync();
                var roleIds = user.RoleUsers.Select(y => y.RoleId).ToList();
                var subData = _orderPricingRole.Where(t => roleIds.Contains(t.RoleId)).FirstOrDefault();
                // if (subData == null)
                //  throw new Exception("您暂时没有医保对照的权限"); 

                var MeiData = await MedicalItemRecordStore.Entities.Include(t => t.MedicalDrugExtensions).Where(t => t.Id == MedId).FirstOrDefaultAsync();
                string OldHiCode = "";
                if (type == 1) //药品
                {
                    string QuerySQL = @"select ID,YLMLBM as gjypdm from SI_YP where id=" + siId;
                    //医保中心数据 13036374133
                    var data = _unitOfWork.SqlQuery<Si_YPOutPut>(QuerySQL).FirstOrDefault();
                    //  isData = await SI_YPMLSStore.GetFirstOrDefaultAsync(t => t.id == siId);
                    if (MeiData != null)
                    {
                        OldHiCode = MeiData.NationItemCode;

                        MedMatchCode medMatch = new MedMatchCode()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            AuditState = 0,
                            DataState = 1,
                            Founder = userId,
                            FounderDate = DateTime.Now,
                            MedId = MeiData.Id,
                            Modifier = userId,
                            ModifierDate = DateTime.Now,
                            NewNationItemCode = data.gjypdm,
                            OldNationItemCode = OldHiCode,
                            MedType = 1,
                        };
                        MedMatchCodeStore.Insert(medMatch);

                        _unitOfWork.SaveChanges();
                        await
                      _logManager.WriteLogAsync(LogType.DataUpdate, "医保对照",
                          $"药品对码：药品ID:{MedId},医保编码：{data.gjypdm},原对照码：{OldHiCode}");
                    }
                }
                else if (type == 2)
                {
                    //耗材
                    string QuerySQL = @"  select ID,YLMLBM as gjxmdm from SI_HC where id=" + siId;
                    //医保中心数据
                    var data = _unitOfWork.SqlQuery<SI_HCOutPut>(QuerySQL).FirstOrDefault();
                    if (MeiData != null)
                    {
                        OldHiCode = MeiData.NationItemCode;
                        MedMatchCode medMatch = new MedMatchCode()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            AuditState = 0,
                            DataState = 1,
                            Founder = userId,
                            FounderDate = DateTime.Now,
                            MedId = MeiData.Id,
                            Modifier = userId,
                            ModifierDate = DateTime.Now,
                            NewNationItemCode = data.gjxmdm,
                            OldNationItemCode = OldHiCode,
                            MedType = 2,
                        };
                        MedMatchCodeStore.Insert(medMatch);
                        _unitOfWork.SaveChanges();
                        if (MeiData.MedicalItemType == 2)
                            await _logManager.WriteLogAsync(LogType.DataUpdate, "耗材对照",
                             $"耗材对码：档案ID:{MedId},医保编码：{data.gjxmdm},原对照码：{OldHiCode}");

                    }
                }
                else
                {
                    //服务项目
                    //sI_ZLXM = await SI_ZLXMStore.GetFirstOrDefaultAsync(t => t.id == siId);
                    string QuerySQL = @"  select ID,LYMLBM as gjxmdm from SI_FWXM where  id =" + siId;
                    //医保中心数据
                    var data = _unitOfWork.SqlQuery<SI_FWXMOutPut>(QuerySQL).FirstOrDefault();
                    if (MeiData != null)
                    {
                        OldHiCode = MeiData.NationItemCode;
                        OldHiCode = MeiData.NationItemCode;
                        MedMatchCode medMatch = new MedMatchCode()
                        {
                            Id = Guid.NewGuid().tostring32(),
                            AuditState = 0,
                            DataState = 1,
                            Founder = userId,
                            FounderDate = DateTime.Now,
                            MedId = MeiData.Id,
                            Modifier = userId,
                            ModifierDate = DateTime.Now,
                            NewNationItemCode = data.gjxmdm,
                            OldNationItemCode = OldHiCode,
                            MedType = 5,
                        };
                        MedMatchCodeStore.Insert(medMatch);
                        _unitOfWork.SaveChanges();

                        await _logManager.WriteLogAsync(LogType.DataUpdate, "诊疗项目对照",
                   $"诊疗项目对码：档案ID:{MedId},医保编码：{data.gjxmdm},原对照码：{OldHiCode}");
                    }
                }
                return true;
            });

        }


        //对照审核列表

        public Task<YBDZMedicalItem[]> GetMedMatchCodeDataAsync()
        {
            return Task.Run(async () =>
            {


                string QuerySQL = @"  select mc.*, d.RetailPrice,d.RetailPrice as purchasingPrice,mr.MedicalItemName,mr.Manufacturer,mr.DoseMin,mu.SpeUnitCHS as DoseUnitName,mr.Specifications,mr.Packaging,mr.Brand,df.TypeName as DosageFormName,(select top 1 chrgitm_lv  from SI_YBcatalog where hilist_code = mc.NewNationItemCode and enddate is null) as ylfydj,
(select top 1 hilist_pric_uplmt_amt  from SI_CheckPriceInfo where hilist_code = mc.NewNationItemCode and enddate is null) as ylbzdj
, hc.ZCMC as tym ,hc.YPSPM as spm ,hc.YPTYM, hc.YPJX as jx, hc.ZXBZSL as bzsl,hc.ZXBZDW as bzdw,hc.ZXZJDW,hc.YPGG as hl, hc.SCQYMC as ycmc ,hc.YLMLBM as gjypdm
 from MedMatchCode as mc
 left join MedicalItemRecords mr on mr.Id = mc.MedId
 left join MedicalDrugExtensions as d  on d.MedicalId =mc.MedId and d.IsCurrentUse = 1 and d.CenterId = '0' 
 left join MedicalUnits mu on mu.Id = mr.DoseUnit
 left join DosageForms df on df.Id = mr.Form
 left join SI_YP hc    on hc.YLMLBM = mc.NewNationItemCode and hc.BBHMC = (select MAX(BBHMC) from SI_YP where YLMLBM = mc.NewNationItemCode) where mc.MedType =1 and  mc.AuditState =0
";

                string HCsql = @"
 		  select mc.*,d.RetailPrice,d.RetailPrice as purchasingPrice,mr.MedicalItemName,mr.Manufacturer,mr.DoseMin,mu.SpeUnitCHS as DoseUnitName,mr.Specifications,mr.Packaging,mr.Brand,df.TypeName as DosageFormName,(select top 1 chrgitm_lv  from SI_YBcatalog where hilist_code = mc.NewNationItemCode and enddate is null) as ylfydj,
(select top 1 hilist_pric_uplmt_amt  from SI_CheckPriceInfo where hilist_code = mc.NewNationItemCode and enddate is null) as ylbzdj
, (select top 1 hilist_name  from SI_YBcatalog where hilist_code = mc.NewNationItemCode and enddate is null)  as tym ,hc.HCMC as spm ,hc.GGXH as hl,  hc.BZSL as bzsl,hc.BZDW as bzdw,hc.BZDW,hc.BZGG as hl, hc.SCQYMC as ycmc ,hc.YLMLBM as gjypdm
 from MedMatchCode as mc
 left join MedicalItemRecords mr on mr.Id = mc.MedId
 left join MedicalUnits mu on mu.Id = mr.DoseUnit
 left join MedicalDrugExtensions as d  on d.MedicalId =mc.MedId and d.IsCurrentUse = 1 and d.CenterId = '0' 
 left join DosageForms df on df.Id = mr.Form 
 left join SI_HC hc    on hc.YLMLBM = mc.NewNationItemCode and hc.BBH = (select MAX(BBH) from SI_HC where YLMLBM = mc.NewNationItemCode) where mc.MedType = 2 and  mc.AuditState =0";


                string zlSql = @"	  select mc.*,d.RetailPrice,d.RetailPrice as purchasingPrice,mr.MedicalItemName,mr.Manufacturer,mr.DoseMin,mu.SpeUnitCHS as DoseUnitName,mr.Specifications,mr.Packaging,mr.Brand,df.TypeName as DosageFormName,(select top 1 chrgitm_lv  from SI_YBcatalog where hilist_code = mc.NewNationItemCode and enddate is null) as ylfydj,
(select top 1 hilist_pric_uplmt_amt  from SI_CheckPriceInfo where hilist_code = mc.NewNationItemCode and enddate is null) as ylbzdj
, hc.YLFWXMMC as tym ,hc.ZLXMSM as spm ,hc.JJDW as hl,   hc.LYMLBM as gjypdm
 from MedMatchCode as mc
 left join MedicalItemRecords mr on mr.Id = mc.MedId
 left join MedicalUnits mu on mu.Id = mr.DoseUnit
 left join MedicalDrugExtensions as d  on d.MedicalId =mc.MedId and d.IsCurrentUse = 1 and d.CenterId = '0' 
 left join DosageForms df on df.Id = mr.Form
 left join SI_FWXM hc    on hc.LYMLBM = mc.NewNationItemCode and hc.BBH = (select MAX(BBH) from SI_FWXM where LYMLBM = mc.NewNationItemCode) where mc.MedType = 5 and  mc.AuditState =0";
                //医保中心数据
                var hcdata = _unitOfWork.SqlQuery<YBDZMedicalItem>(HCsql).ToList();

                //医保中心数据
                var zldata = _unitOfWork.SqlQuery<YBDZMedicalItem>(zlSql).ToList();
                //医保中心数据
                var data = _unitOfWork.SqlQuery<YBDZMedicalItem>(QuerySQL).ToList();
                if (hcdata != null && hcdata.Count > 0)
                    data.AddRange(hcdata);
                if (zldata != null && zldata.Count > 0)
                    data.AddRange(zldata);

                data.ForEach(t => { t.HiLevel = t.ylfydj = string.IsNullOrWhiteSpace(t.ylfydj) ? "丙" : t.ylfydj.Contains("1") ? "甲" : t.ylfydj.Contains("2") ? "乙" : "丙"; t.NationItemCode = t.NewNationItemCode; t.SocialSecurityPrice = t.ylbzdj; });
                var result = data.OrderByDescending(t => t.ModifierDate).ToArray();
                return result;

            });
        }

        //对照审核
        /// <summary>
        /// 对照审核
        /// </summary>
        public Task<bool> NewHIMatchCode(MedMatchCode matchCode)
        {
            return Task.Run(async () =>
            {
                string sql = $"select top 1 *  from SI_YBcatalog where hilist_code = '{matchCode.NewNationItemCode}' and enddate is null";

                var zldata = _unitOfWork.SqlQuery<SI_YBcatalog>(sql).FirstOrDefault();
                var medData = MedMatchCodeStore.Entities.FirstOrDefault(t => t.Id == matchCode.Id);
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                medData.AuditState = matchCode.AuditState;
                medData.ModifierDate = DateTime.Now;
                medData.Modifier = userId;
                MedMatchCodeStore.Update(medData);
                if (matchCode.AuditState == 1)
                {
                    var mitemData = MedicalItemRecordStore.Entities.FirstOrDefault(t => t.Id == matchCode.MedId);
                    mitemData.NationItemCode = matchCode.NewNationItemCode;
                    if (zldata != null)
                    {
                        mitemData.HiLevel = Convert.ToInt32(zldata.chrgitm_lv);
                        mitemData.HiCenterName = zldata.hilist_name;
                    }
                    else
                    {
                        mitemData.HiLevel = 3;
                    }
                    MedicalItemRecordStore.Update(mitemData);
                }
                _unitOfWork.SaveChanges();
                return true;
            });
        }


        //已对照项目
        public Task<PageData<object>> NewYDZSI_YPMLAsync(Si_YpmlQueryInput input)
        {
            return Task.Run(async () =>
            {
                int count = 0;

                string CountSql = $"select COUNT(*) as count from MedicalItemRecords as a  where a.MedicalItemType ={input.ItemType} and a.DataState=1 and (a.NationItemCode is not null   and  NationItemCode != '') ";

                if (!string.IsNullOrWhiteSpace(input.Manufacturer))
                    CountSql += $" and a.Manufacturer like '%{input.Manufacturer}%'";
                if (!string.IsNullOrWhiteSpace(input.YpName))
                    CountSql += $" and a.MedicalItemName like '%{input.YpName}%'";
                if (!string.IsNullOrWhiteSpace(input.nationItemCode))
                    CountSql += $" and a.NationItemCode like '%{input.nationItemCode}%'";
                if (!string.IsNullOrWhiteSpace(input.Packaging))
                    CountSql += $" and a.Specifications like '%{input.Packaging}%'";
                count = _unitOfWork.SqlQuery<PageCount>(CountSql).First().count;

                string QuerySQL = $@"
select a.*  ,COALESCE(b.chrgitm_lv, '03') as chrgitm_lv ,c.hilist_pric_uplmt_amt ,d.RetailPrice as PurchasingPrice ,e.TypeName  as formValue ,f.SpeUnitUS as doseUnitValue ,a.Specifications as minDose  from MedicalItemRecords    as a
left join SI_YBcatalog as  b on b.hilist_code = a.NationItemCode  and b.enddate is   null
left join SI_CheckPriceInfo as c on c.hilist_code = a.NationItemCode and c.hilist_lmtpric_type='902' and c.enddate is null
left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.CenterId = '0'
left join DosageForms as e on e.Id = a.Form
left join MedicalUnits as f on f.Id = a.DoseUnit
  where a.MedicalItemType ={input.ItemType} and a.DataState=1  and (a.NationItemCode is not null   and  NationItemCode != '')  ";
                if (!string.IsNullOrWhiteSpace(input.Manufacturer))
                    QuerySQL += $" and a.Manufacturer like '%{input.Manufacturer}%'";
                if (!string.IsNullOrWhiteSpace(input.YpName))
                    QuerySQL += $" and a.MedicalItemName like '%{input.YpName}%'";
                if (!string.IsNullOrWhiteSpace(input.nationItemCode))
                    QuerySQL += $" and a.NationItemCode like '%{input.nationItemCode}%'";
                if (!string.IsNullOrWhiteSpace(input.Packaging))
                    QuerySQL += $" and a.Specifications like '%{input.Packaging}%'";
                QuerySQL += $"order by  a.Id  offset {(input.PageNum - 1) * input.PageSize} rows fetch next {input.PageSize} rows only";
                var data = _unitOfWork.SqlQuery<MedicalItemOutPut>(QuerySQL).ToArray();
                //医保中心数据
                string codes = "";
                foreach (var item in data)
                {
                    if (codes == "")
                        codes += $"'{item.NationItemCode}'";
                    else
                        codes += $",'{item.NationItemCode}'";
                }
                string SqlWhere = $"  and  hc.YLMLBM in({codes})";
                List<Si_YPOutPut> SIdata = new List<Si_YPOutPut>();
                List<SI_HCOutPut> hcdata = new List<SI_HCOutPut>();
                var fwxmdata = new List<SI_FWXMOutPut>();
                object OBJ = new object();
                //医保中心数据  
                switch (input.ItemType)
                {
                    case 1:
                        string SIQuerySQL = @"
	select  hc.ID, hc.ZCMC as tym ,hc.YPSPM as spm ,hc.YPTYM, hc.YPJX as jx, hc.ZXBZSL as bzsl,hc.ZXBZDW as bzdw,hc.ZXZJDW,hc.YPGG as hl, hc.SCQYMC as ycmc ,hc.YLMLBM as gjypdm, cp.hilist_pric_uplmt_amt as ylbzdj, own.selfpay_prop as ylzfbl, COALESCE(   cat.chrgitm_lv, '03')    as ylfydj  , cp.hilist_lmtpric_type as  lmttype,own.selfpay_prop_psn_type as protype  ,hc.ZHB ,hc.XZSYBZ ,hc.XZSYFW,hc.PZWH ,cp.updt_time as bgsj ,cat.memo as bz ,hc.BZCZ,hc.BZCZMC from   SI_YP as hc 
				left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM and  ((cp.hilist_lmtpric_type = '902' ) and cp.enddate is null)
				left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM  and((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
				left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null 
				 where  hc.YXBZ =1  and hc.BBHMC = ( select MAX(BBHMC) from SI_YP  where SI_YP.YLMLBM =hc.YLMLBM)
                " + SqlWhere;
                        SIdata = _unitOfWork.SqlQuery<Si_YPOutPut>(SIQuerySQL).ToList();
                        OBJ = new Newyibao() { medical = data, SIypmls = SIdata.ToArray() };
                        break;
                    case 2:
                        QuerySQL = @" 
  select hc.Id, hc.YLMLBM as gjxmdm,cat.hilist_name as xmmc,hc.GG as gg ,hc.HCFL,hc.SCQYMC as bz, hc.YXBS,hc.BBH,cp.hilist_pric_uplmt_amt as ylzdj, own.selfpay_prop as txbl,  COALESCE(   cat.chrgitm_lv, '03') as ylfydj , cp.hilist_lmtpric_type as  lmttype ,cp.updt_time as bgsj , hc.HCFL as hcfl from SI_HC as hc 
  left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM   and  (( cp.hilist_lmtpric_type = '902' ) and cp.enddate is null)
  left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM and ((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
  left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null   WHERE hc.YXBS != '0' and   hc.BBH = ( select MAX(BBH) from SI_HC  where SI_HC.YLMLBM =hc.YLMLBM) " + SqlWhere;
                        //医保中心数据 
                        hcdata = _unitOfWork.SqlQuery<SI_HCOutPut>(QuerySQL).ToList();
                        OBJ = new HC_OutPut() { medical = data, sI_HCOutPuts = hcdata.ToArray() };
                        break;
                    case 5:
                        SqlWhere = $"  and  hc.LYMLBM in({codes})";
                        QuerySQL = @"   
                 select hc.Id, hc.LYMLBM as gjxmdm,hc.JJDW as jjdw ,hc.BZ as bz ,hc.YLFWXMMC as xmmc ,cp.hilist_pric_uplmt_amt as ylbzj, own.selfpay_prop as txbl, COALESCE(   cat.chrgitm_lv, '03')  as ylfydj,cp.hilist_lmtpric_type as  lmttype,own.selfpay_prop_psn_type as protype ,hc.ZLXMLH as zlxmhl ,cp.updt_time as bgsj from SI_FWXM as hc 
 left join SI_CheckPriceInfo as cp on  cp.hilist_code  = hc.LYMLBM  and ( ( cp.hilist_lmtpric_type = '902') and cp.enddate is null)
 left join SI_OwnExpenseInfo as own on  own.hilist_code = hc.LYMLBM  and((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
 left join SI_YBcatalog as cat on    cat.hilist_code  = hc.LYMLBM  and cat.enddate is null 
 where  hc.YXBZ  ='1'    and hc.BBMC = ( select MAX(BBMC) from SI_FWXM  where SI_FWXM.LYMLBM =hc.LYMLBM)

                " + SqlWhere;

                        fwxmdata = _unitOfWork.SqlQuery<SI_FWXMOutPut>(QuerySQL).ToList();
                        OBJ = new HC_FWXM() { sI_HCOutPuts = fwxmdata.ToArray(), medical = data };
                        break;
                    default:
                        break;
                }

                return new PageData<object>(OBJ, count);

            });
        }








        #endregion






        #endregion

        #region  Center
        public Task<MedicalItemRecord[]> GetMedicalItemRecordQueryableAsync(string centerId)
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                MedicalItemRecord[] result = null;
                try
                {
                    // Expression<Func<MedicalItemRecord, bool>> predicate = t => t.IsDelete == IsDel;

                    var jiage = await GetCenterMedicalDrugExtensionsQueryableAsync(centerId);

                    result = await MedicalItemRecordStore.Entities.Where(t => t.IsCenterAdd != 1).Include(t => t.SpecificationsUnits).OrderBy(t => t.MedicalItemType).ToArrayAsync();
                    result.Where(t => t.MedicalItemType == 5).ToList().ForEach(t =>
                    {

                        var MedJiaGe = jiage.Where(k => k.MedicalId == t.Id && k.IsCurrentUse == true).FirstOrDefault();
                        if (MedJiaGe != null)
                            t.Packaging = t.ProcurementPackage = t.Specifications = $"{MedJiaGe.RetailPrice.ToFloorRound()}元/{t.SpecificationsUnits.SpeUnitCHS}";
                    });
                    result.ToList().ForEach(t => t.MedicalDrugExtensions = null);
                    //result = Mapper.Map<CenterMedicalItemRecordOutPut[]>(datas);
                    // 
                    //int i = 0;  
                    //foreach (var item in result)
                    //{
                    //    GetManId(datas[i].WareHouseIds, item.WareHouseId);
                    //    GetFromManId(datas[i].Form, item.Forms);
                    //    var dd = datas[i].MedicalDrugExtensions.Where(t => t.IsCurrentUse).FirstOrDefault();
                    //    item.MedicalDrugExtension = dd != null ? Mapper.Map<MedicalDrugExtensionOutput>(dd) : new MedicalDrugExtensionOutput();
                    //    i++;
                    //} 
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }


        public Task<CenterWarehouseCatalogOutPut[]> GetCenterWarehouseCatalogQueryableAsync()
        {
            return Task.Run(async () =>
            {
                CenterWarehouseCatalogOutPut[] result = null;

                try
                {
                    var datas = await WarehouseCatalogStore.Entities.ToArrayAsync();
                    result = Mapper.Map<CenterWarehouseCatalogOutPut[]>(datas);


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                return result;
            });
        }
        /// <summary>
        ///档案价格
        /// </summary>
        /// <returns></returns>
        public Task<MedicalDrugExtension[]> GetCenterMedicalDrugExtensionsQueryableAsync(string CenterId)
        {
            return Task.Run(async () =>
            {
                List<MedicalDrugExtension> result = new List<MedicalDrugExtension>();

                try
                {
                    List<string> centerIs = new List<string>();
                    centerIs.Add("0");
                    centerIs.Add(CenterId);
                    result = await MedicalDrugExtensionStore.Entities.Where(t => centerIs.Contains(t.CenterId) && t.DataState == 1).ToListAsync();
                    //result = Mapper.Map<CenterWarehouseCatalogOutPut[]>(datas);
                    //int count = result.Count;
                    //for (int i = 0; i < count; i++)
                    //{


                    //}
                    var GroupData = result.GroupBy(t => t.MedicalId);
                    foreach (var item in GroupData)
                    {
                        var ListData = item.ToList().Where(t => t.IsCurrentUse == true);
                        //中心本地价格
                        var centerPrice = ListData.Where(t => t.CenterId != "0").OrderByDescending(t => t.FounderDate).FirstOrDefault();// != null
                        if (centerPrice != null)
                        {
                            ListData.Where(t => t.CenterId == "0").ToList().ForEach(t => t.IsCurrentUse = false);
                        }

                        //统一价格

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
        /// 获取字典类型列表
        /// </summary>
        public Task<DictionaryTypeOutPut[]> GetDictionaryTypesQueryableAsync()
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                DictionaryTypeOutPut[] result = null;
                try
                {
                    List<string> typeList = new List<string>()
                    { 
                        //学历、性别、人员类型、职称等级、在职情况
                        "学历",
                        "性别",
                        "职位",
                        "职称",
                        "在职情况",
                        "费用类别"

                    };

                    Expression<Func<DictionaryType, bool>> predicate = t => t.IsDelete == IsDel && typeList.Contains(t.Name);

                    var datas = await DictionaryTypeStore.Entities.Where(predicate).ToArrayAsync();
                    result = Mapper.Map<DictionaryTypeOutPut[]>(datas);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        /// <summary>
        /// 获取字典类型列表
        /// </summary>
        public Task<SystemDictionaryOutPut[]> GetSysDictionaryQueryableAsync()
        {

            return Task.Run(async () =>
            {
                bool IsDel = false;
                SystemDictionaryOutPut[] result = null;
                try
                {
                    var data = await GetDictionaryTypesQueryableAsync();
                    var list = await EmployeeStore.Entities.Include(t => t.CenterDialysis).Where(t => t.CenterDialysisId != "" && t.CenterDialysisId != null && t.CenterDialysis.IsDelete == false).Select(t => t.positionId).ToListAsync();
                    //&& list.Contains(t.Id)
                    Expression<Func<SystemDictionary, bool>> predicate = t => t.IsDelete == IsDel && t.IsCentDic == IsDel;
                    list = list.CompareDistinct(t => t).ToList();
                    if (data != null && data.Length > 0)
                    {
                        List<string> typeList = data.Select(t => t.Id).ToList();
                        predicate = predicate.And(t => typeList.Contains(t.TypeId));
                        //
                    }
                    var datas = await SystemDictionaryStore.Entities.Include(t => t.DictionaryType).Where(predicate).OrderBy(t => t.ShowSortNo).ToArrayAsync();
                    result = Mapper.Map<SystemDictionaryOutPut[]>(datas);

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }


                return result;
            });
        }

        //病种目录
        /// <summary>
        /// 病种目录
        /// </summary>
        /// <returns></returns>
        public Task<SI_BZML[]> GetSI_BZMLAsync()
        {
            return Task.Run(async () =>
            {
                try
                { 
                    var list = await SI_BZMLStore.Entities.ToListAsync();
                    return list.ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

            });
        }
        #endregion


        #region   物品档案关联
        //MRelevancyStore
        //新增
        public Task<bool> CreateMRelevancyAsync(List<MedicalRelevancy> input)
        {
            return Task.Run(async () =>
            {
                bool result = false;

                if (input == null || input.Count <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                foreach (var item in input)
                {
                    if (string.IsNullOrEmpty(item.MainMedId))
                        throw new Exception(MessageFormater.PrameterNeedProvider("input.MainMedId"));
                    if (string.IsNullOrEmpty(item.ChildMedId))
                        throw new Exception(MessageFormater.PrameterNeedProvider("input.ChildMedId"));

                    item.Founder = userId;
                    item.FounderDate = DateTime.Now;
                    item.Modifier = userId;
                    item.ModifierDate = DateTime.Now;
                    item.DataState = 1;
                    item.Id = Guid.NewGuid().tostring32();
                    MRelevancyStore.Insert(item);

                }
                _unitOfWork.SaveChanges();
                result = true;

                return result;

            });
        }

        //修改
        //删除
        public Task<bool> DeleteMRelevancyAsync(List<MedicalRelevancy> input)
        {
            return Task.Run(async () =>
            {
                bool result = false;

                if (input == null || input.Count <= 0)
                    throw new Exception(MessageFormater.PrameterNeedProvider("input"));
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                foreach (var item in input)
                {
                    var data = await MRelevancyStore.GetFirstOrDefaultAsync(t => t.Id == item.ChildMedId);
                    if (data == null)
                        continue;
                    // 
                    data.Modifier = userId;
                    data.ModifierDate = DateTime.Now;
                    data.DataState = 3;
                    MRelevancyStore.Update(data);
                }
                _unitOfWork.SaveChanges();
                result = true;

                return result;

            });
        }

        //查询
        /// <summary>
        /// 物品关联列表
        /// </summary>
        /// <returns></returns>
        public Task<MRelevancyOutPut[]> GetMRelevancyOutPutAsync()
        {
            return Task.Run(async () =>
            {


                List<MRelevancyOutPut> relevancyOutPuts = new List<MRelevancyOutPut>();

                var data = await MRelevancyStore.Entities.Include(t => t.MainMed).Include(t => t.ChildMed).Where(t => t.DataState == 1).ToListAsync();

                if (data != null && data.Count > 0)
                {


                    var groupData = data.GroupBy(t => t.MainMedId);
                    foreach (var item in groupData)
                    {
                        var result = Mapper.Map<MRelevancyOutPut>(item.First());
                        //获取主档案
                        var main = item.First().MainMed;
                        if (main == null)
                            continue;
                        var msp = main.Specifications;

                        if (main.MedicalItemType == 1 || main.MedicalItemType == 2)
                            msp = main.IsSplited ? main.Specifications : main.Packaging;
                        result.MainMedModel = new RelevancyMedModel()
                        {
                            Brand = main.Brand,
                            Manufacturer = main.Manufacturer,
                            MedicalItemCode = main.MedicalItemCode,
                            MedicalItemName = main.MedicalItemName,
                            Packaging = msp
                        };
                        result.ChildMedModel = new List<RelevancyMedModel>();
                        foreach (var items in item)
                        {
                            var ChildMed = items.ChildMed;
                            if (ChildMed == null)
                                continue;
                            var sp = ChildMed.Specifications;
                            if (ChildMed.MedicalItemType == 1 || ChildMed.MedicalItemType == 2)
                                sp = ChildMed.IsSplited ? ChildMed.Specifications : ChildMed.Packaging;
                            result.ChildMedModel.Add(new RelevancyMedModel()
                            {
                                Id = items.Id,
                                MedicalItemId = ChildMed.Id,
                                Brand = ChildMed.Brand,
                                Manufacturer = ChildMed.Manufacturer,
                                MedicalItemCode = ChildMed.MedicalItemCode,
                                MedicalItemName = ChildMed.MedicalItemName,
                                Packaging = sp

                            });
                        }
                        // 
                        relevancyOutPuts.Add(result);

                    }

                }

                return relevancyOutPuts.ToArray();

            });
        }



        #endregion

        #region  手动更新目录
        public Task<MsgModel> GetYPMLAsync()
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                await _dataReceive.GetYPMLAsync();
                return new MsgModel() { msg = "更新成功" };
            });
        }

        public Task<MsgModel> GetHCMLAsync()
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                await _dataReceive.GetHCMLAsync();
                return new MsgModel() { msg = "更新成功" };
            });
        }

        public Task<MsgModel> GetFWMLAsync()
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                await _dataReceive.GetFWMLAsync();
                return new MsgModel() { msg = "更新成功" };

            });
        }

        public Task<MsgModel> GetXJAsync()
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {
                await _dataReceive.GetXJAsync();
                return new MsgModel() { msg = "更新成功" };

            });
        }
        /// <summary>
        /// GetChronicSpecialAsync(string data)
        /// </summary>
        /// <returns></returns>
        public Task<Datas.ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>> GetChronicSpecialAsync(string data)
        {
            // _unitOfWork.
            return Task.Run(async () =>
            {

                string strResult = await _dataReceive.GetChronicSpecialAsync(data);

                var ResultData = JsonConvert.DeserializeObject<Datas.ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>>(strResult);


                return ResultData;
            });
        }

        #endregion

        #region 更新提示
        public Task<PageData<DirectoryModel[]>> CatalogUpdateReminder(QualityControlQueryInPut InPut)
        {
            var now = DateTime.Now.Date.AddDays(-7);
            return Task.Run(async () =>
            {
                string ypWhere = string.Empty;
                string hcWhere = string.Empty;
                string fwWhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(InPut.QueryName))
                {
                    ypWhere = $@" and  hc.ZCMC like '%{InPut.QueryName}%' ";
                    hcWhere = $@" and  hc.HCMC like '%{InPut.QueryName}%' ";
                    fwWhere = $@" and  hc.YLFWXMMC like '%{InPut.QueryName}%' ";
                }
                string QuerySQL = $@"
	SELECT DISTINCT  * FROM (select   hc.ZCMC as '名称' ,hc.YPSPM as '商品名', hc.YLMLBM AS '国家医保编码', hc.ZXBZDW  AS '包装单位',(hc.ZZGG+'*'+  hc.ZXBZSL++ zxzjdw+'/'+hc.ZXBZDW) AS '规格型号', cp.hilist_pric_uplmt_amt as '基准价格',hc.update_time as '本地更新时间', own.selfpay_prop as '自付比例',cat.memo as  '医保目录备注'
,CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11)) as '目录变更时间'
 from   SI_YP as hc 
left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM and  ((cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM  and((own.selfpay_prop_psn_type = '9901' or own.selfpay_prop_psn_type is null) and own.enddate is null)
left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null 
INNER join dbo.MedicalItemRecords as mir on  mir.NationItemCode=hc.YLMLBM
where  hc.YXBZ =1  and hc.BBHMC = ( select MAX(BBHMC) from SI_YP  where SI_YP.YLMLBM =hc.YLMLBM) AND   (hc.update_time>='{now}'   OR CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11))>='{now}')  AND mir.NationItemCode IS NOT NULL  AND   NationItemCode!='' {ypWhere}
UNION ALL
SELECT    hc.HCMC AS '名称' ,hc.HCMC AS  '商品名', hc.YLMLBM AS '国家医保编码', ''  AS '包装单位',hc.GGXH AS '规格型号', cp.hilist_pric_uplmt_amt AS '基准价格',hc.update_time AS '本地更新时间', own.selfpay_prop AS '自付比例',cat.memo AS  '医保目录备注'
,CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11)) as '目录变更时间'
FROM SI_HC AS hc 
  LEFT JOIN SI_CheckPriceInfo AS cp ON cp.hilist_code = hc.YLMLBM   AND  ((cp.hilist_lmtpric_type = '901' OR cp.hilist_lmtpric_type = '902' OR cp.hilist_lmtpric_type IS NULL) AND cp.enddate IS NULL)
  LEFT JOIN SI_OwnExpenseInfo AS own ON own.hilist_code = hc.YLMLBM AND ((own.selfpay_prop_psn_type = '9901' OR own.selfpay_prop_psn_type IS NULL) AND own.enddate IS NULL)
  LEFT JOIN SI_YBcatalog AS cat ON cat.hilist_code = hc.YLMLBM  AND cat.enddate IS NULL 
INNER join dbo.MedicalItemRecords as mir on  mir.NationItemCode=hc.YLMLBM
WHERE hc.YXBS != '0' AND   hc.BBH = ( SELECT MAX(BBH) FROM SI_HC  WHERE SI_HC.YLMLBM =hc.YLMLBM) AND  (hc.update_time>='{now}'   OR CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11))>='{now}') AND mir.NationItemCode IS NOT NULL  AND   NationItemCode!='' {hcWhere}
  UNION ALL
  select       hc.YLFWXMMC as '名称'  ,hc.ZLXMSM AS  '商品名', hc.LYMLBM AS '国家医保编码', ''  AS '包装单位','' AS '规格型号', cp.hilist_pric_uplmt_amt as '基准价格',hc.update_time as '本地更新时间', own.selfpay_prop as '自付比例',cat.memo as  '医保目录备注'
  ,CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11)) as '目录变更时间'
   from SI_FWXM as hc 
 left join SI_CheckPriceInfo as cp on  cp.hilist_code  = hc.LYMLBM  and ( (cp.hilist_lmtpric_type = '901' or cp.hilist_lmtpric_type = '902' or cp.hilist_lmtpric_type is null) and cp.enddate is null)
 left join SI_OwnExpenseInfo as own on  own.hilist_code = hc.LYMLBM  and((own.selfpay_prop_psn_type = '9901' or own.selfpay_prop_psn_type is null) and own.enddate is null)
 left join SI_YBcatalog as cat on    cat.hilist_code  = hc.LYMLBM  and cat.enddate is null 
INNER join dbo.MedicalItemRecords as mir on  mir.NationItemCode=hc.LYMLBM
 where  hc.YXBZ  ='1'    and hc.BBMC = ( select MAX(BBMC) from SI_FWXM  where SI_FWXM.LYMLBM =hc.LYMLBM) AND  (hc.update_time>='{now}'   OR CONVERT(Datetime,  SUBSTRING(cp.begndate,0,11))>='{now}')  AND mir.NationItemCode IS NOT NULL  AND   NationItemCode!='' {fwWhere} )AS aa
                ";
                var result = _unitOfWork.SqlQuery<DirectoryModel>(QuerySQL).ToList();
                int count = result.Count;
                if (InPut.PageNum > 0 && InPut.PageSize > 0)
                    result = await PaginatedList<DirectoryModel>.CreateAsync(result.OrderBy(t => t.名称).ToList(), InPut.PageNum, InPut.PageSize);
                return new PageData<DirectoryModel[]>(result.ToArray(), count);
            });


        }
        #endregion

        #region  医保月核对

        //MedHisCatalogOutPut

        public async Task<List<MedHisCatalogOutPut>> GetMedHisCatalogOutPutAsync(int ItemType)
        {

            return await Task.Run(() =>
            {

                //CAST(EmployeeID AS VARCHAR(10))
                string QuerySQL = "";

                switch (ItemType)
                {
                    case 1:
                        QuerySQL = @" select * from (				 
				 SELECT mma.AuditState AS AuditState, '本地目录' as MedCatalog, a.Id as MedicalId,   a.MedicalItemName,a.MedicalItemCode, a.Packaging, f.SpeUnitCHS AS MedUnit, a.ApprovalNum,a.Manufacturer,a.NationItemCode, CAST(d.PurchasingPrice AS VARCHAR(10)) as  PurchasingPrice  ,CAST(d.RetailPrice AS VARCHAR(10)) as  RetailPrice    ,a.HiLevel  as ylfydj from MedicalItemRecords a 
				 left join MedicalUnits as f on f.Id = a.PackageUnit
				 left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.DataState = 1  and  d.ModifierDate = ( select  MAX(e.ModifierDate)  from MedicalDrugExtensions e  where e.MedicalId =a.Id and e.IsCurrentUse = 1 and e.DataState = 1)
LEFT  JOIN  ( 
   SELECT 
        *,
        ROW_NUMBER() OVER (PARTITION BY MedicalId ORDER BY FounderDate DESC) AS rn
    FROM 
      MedMonthAudit
    WHERE 
        FounderDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
        AND FounderDate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0)
 ) as  mma on  mma.MedicalId = a.id and mma.rn =1
				 where a.MedicalItemType =1 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != '')	
				UNION ALL
	select  -1 AS AuditState,'医保目录' as MedCatalog, '-' as MedicalId,  hc.ZCMC as MedicalItemName ,'' MedicalItemCode  ,(hc.YPGG +'*' + hc.ZXBZSL +hc.ZXZJDW+'/'+hc.ZXBZDW ) as Packaging, hc.ZXBZDW MedUnit,hc.pzwh as ApprovalNum  , hc.SCQYMC as Manufacturer ,hc.YLMLBM as NationItemCode, '-' PurchasingPrice, CAST( cp.hilist_pric_uplmt_amt AS VARCHAR(10)) as RetailPrice,  cat.chrgitm_lv as ylfydj  from   SI_YP as hc 
				left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM and  ((cp.hilist_lmtpric_type = '902' ) and cp.enddate is null)
				left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM  and((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
				left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null 
				 where  hc.YXBZ =1  and hc.BBHMC = ( select MAX(BBHMC) from SI_YP  where SI_YP.YLMLBM =hc.YLMLBM)
				 
				 and   hc.YLMLBM  in ( SELECT a.NationItemCode  from  MedicalItemRecords a    where a.MedicalItemType =1 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != ''))
				 )  te  
				 
				 ORDER BY   NationItemCode ,MedCatalog ";
                        break;
                    case 2:
                        QuerySQL = @" select * from (
			SELECT mma.AuditState AS AuditState, '本地目录' MedCatalog,  a.Id as MedicalId,   a.MedicalItemName,a.MedicalItemCode, a.Packaging, f.SpeUnitCHS AS MedUnit, a.ApprovalNum,a.Manufacturer,a.NationItemCode, CAST(d.PurchasingPrice AS VARCHAR(10)) as  PurchasingPrice  ,CAST(d.RetailPrice AS VARCHAR(10)) as  RetailPrice    ,a.HiLevel  as ylfydj from MedicalItemRecords a 
				 left join MedicalUnits as f on f.Id = a.PackageUnit
				 left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.DataState = 1  and  d.ModifierDate = ( select  MAX(e.ModifierDate)  from MedicalDrugExtensions e  where e.MedicalId =a.Id and e.IsCurrentUse = 1 and e.DataState = 1)	
LEFT  JOIN  ( 
   SELECT 
        *,
        ROW_NUMBER() OVER (PARTITION BY MedicalId ORDER BY FounderDate DESC) AS rn
    FROM 
      MedMonthAudit
    WHERE 
        FounderDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
        AND FounderDate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0)
 ) as  mma on  mma.MedicalId = a.id and mma.rn =1
				 where a.MedicalItemType =2 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != '')			
			UNION ALL	
			select  -1  AS AuditState,'医保目录' MedCatalog,  '-' as MedicalId,cat.hilist_name as MedicalItemName,'' MedicalItemCode,hc.GG as Packaging ,hc.HCFL as MedUnit ,hc.ZCBAH as ApprovalNum ,hc.SCQYMC as Manufacturer,hc.YLMLBM as NationItemCode, '-' PurchasingPrice, CAST( cp.hilist_pric_uplmt_amt AS VARCHAR(10)) as RetailPrice,   cat.chrgitm_lv as ylfydj from SI_HC as hc 
left join SI_CheckPriceInfo as cp on cp.hilist_code = hc.YLMLBM   and  (( cp.hilist_lmtpric_type = '902' ) and cp.enddate is null)
left join SI_OwnExpenseInfo as own on own.hilist_code = hc.YLMLBM and ((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
left join SI_YBcatalog as cat on cat.hilist_code = hc.YLMLBM  and cat.enddate is null   WHERE hc.YXBS != '0' and   hc.BBH = ( select MAX(BBH) from SI_HC  where SI_HC.YLMLBM =hc.YLMLBM)  and 
		     hc.YLMLBM  in ( SELECT a.NationItemCode  from  MedicalItemRecords a    where a.MedicalItemType =2 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != ''))
			
			
			 )  te   ORDER BY   NationItemCode ,MedCatalog ";
                        break;
                    case 3:
                        QuerySQL = @" select * from (
			SELECT mma.AuditState AS AuditState, '本地目录' MedCatalog,   a.Id as MedicalId,  a.MedicalItemName,a.MedicalItemCode, a.Packaging, f.SpeUnitCHS AS MedUnit,a.Manufacturer,a.NationItemCode, CAST(d.PurchasingPrice AS VARCHAR(10)) as  PurchasingPrice  ,CAST(d.RetailPrice AS VARCHAR(10)) as  RetailPrice   ,a.HiLevel  as ylfydj from MedicalItemRecords a 
				 left join MedicalUnits as f on f.Id = a.PackageUnit
				 left join MedicalDrugExtensions as d  on d.MedicalId =a.Id and d.IsCurrentUse = 1 and d.DataState = 1  and  d.ModifierDate = ( select  MAX(e.ModifierDate)  from MedicalDrugExtensions e  where e.MedicalId =a.Id and e.IsCurrentUse = 1 and e.DataState = 1)		
LEFT  JOIN  ( 
   SELECT 
        *,
        ROW_NUMBER() OVER (PARTITION BY MedicalId ORDER BY FounderDate DESC) AS rn
    FROM 
      MedMonthAudit
    WHERE 
        FounderDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
        AND FounderDate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0)
 ) as  mma on  mma.MedicalId = a.id and mma.rn =1
				 where a.MedicalItemType =5 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != '')
				UNION ALL
			select -1 AS AuditState,'医保目录' MedCatalog, '-' as MedicalId,hc.YLFWXMMC as MedicalItemName,'' MedicalItemCode,  (''+CONVERT(VARCHAR(256),cp.hilist_pric_uplmt_amt )+'元/'+hc.JJDW)   as Packaging, hc.JJDW as MedUnit,hc.ZLXMLH  as Manufacturer ,hc.LYMLBM as NationItemCode, '-' PurchasingPrice, CAST( cp.hilist_pric_uplmt_amt AS VARCHAR(10)) as RetailPrice, cat.chrgitm_lv as ylfydj from SI_FWXM as hc 
left join SI_CheckPriceInfo as cp on  cp.hilist_code  = hc.LYMLBM  and ( ( cp.hilist_lmtpric_type = '902') and cp.enddate is null)
left join SI_OwnExpenseInfo as own on  own.hilist_code = hc.LYMLBM  and((own.selfpay_prop_psn_type = '9901') and own.enddate is null)
left join SI_YBcatalog as cat on    cat.hilist_code  = hc.LYMLBM  and cat.enddate is null 
where  hc.YXBZ  ='1'    and hc.BBMC = ( select MAX(BBMC) from SI_FWXM  where SI_FWXM.LYMLBM =hc.LYMLBM)
		and      hc.LYMLBM  in ( SELECT a.NationItemCode  from  MedicalItemRecords a    where a.MedicalItemType =5 and a.DataState=1  and (a.NationItemCode is not null   and  a.NationItemCode != ''))
			
			
			 )  te   ORDER BY   NationItemCode ,MedCatalog ";
                        break;
                    default:
                        break;
                }
                var result = _unitOfWork.SqlQuery<MedHisCatalogOutPut>(QuerySQL).ToList();


                result.ForEach(x =>
                {
                    x.AuditState = x.AuditState.HasValue ? x.AuditState : 0;
                });
                return result;
            });


        }



        //月审核  MedMonthAuditStore

        public Task<bool> SI_MedMonthAudit(AuditInput input)
        {
            return Task.Run(async () =>
            {
                var userId = await _getUserInfo.GetCurrentUserIdAsync();
                MedMonthAudit medMatch = new MedMonthAudit()
                {
                    Id = Guid.NewGuid().tostring32(),
                    AuditState = input.AuditState,
                    MedicalId = input.MedicalId,
                    NationItemCode = input.NationItemCode,
                    Founder = userId,
                    DataState = 1,
                    FounderDate = DateTime.Now,
                    Modifier = userId,
                    ModifierDate = DateTime.Now,


                };
                MedMonthAuditStore.Insert(medMatch);

                _unitOfWork.SaveChanges();

                return true;
            });

        }





        #endregion




        public class PageCount
        {
            public int count { get; set; }
        }


        public class AuditInput
        {
            public string MedicalId { get; set; }
            /// <summary>
            /// 国家码
            /// </summary>
            public string NationItemCode { get; set; }

            /// <summary>
            /// 审核状态 NULL 未审核 1 审核通过 2 未通过
            /// </summary>
            public int AuditState { get; set; }


        }

        public class DirectoryModel
        {
            public string 名称 { get; set; }
            public string 商品名 { get; set; }
            public string 国家医保编码 { get; set; }
            public string 包装单位 { get; set; }
            public string 规格型号 { get; set; }
            public decimal? 基准价格 { get; set; }
            public decimal? 自付比例 { get; set; }
            public string 医保目录备注 { get; set; }
            public DateTime? 本地更新时间 { get; set; }
            public DateTime? 目录变更时间 { get; set; }
        }


    }
}