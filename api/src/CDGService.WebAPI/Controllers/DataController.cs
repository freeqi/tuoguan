using CDGService.Data.Datas;
using CDGService.Data.Helper;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Dto.AutoCenterPut;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CDGService.WebAPI.DataCore.DataManager;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly Data.DocumentSetting _documentSetting;
        private readonly LogManager _logManager;

        public DataController(DataManager dataManager, LogManager logManager, Microsoft.Extensions.Options.IOptions<Data.DocumentSetting> documentSetting)
        {
            _dataManager = dataManager;
            _logManager = logManager;
            _documentSetting = documentSetting.Value;
        }

        /// <summary>
        /// 获取日志列表。
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("log/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LogOutPut[]>> GetLogQueryableAsync([FromBody] Dto.LogInput logInput)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetLogQueryableAsync(logInput);
                return new ServiceMessage<LogOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 获取地区树
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("region/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SysRegionOutput[]>> GetSysRegionQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetSysRegionQueryableAsync();
                return new ServiceMessage<SysRegionOutput[]>(result);
            });
        }
        //   public Task<WarehouseCatalogOutPut[]> GetWarehouseCatalogTreeQueryableAsync()

        #region  药品、档案

        /// <summary>
        /// 库房目录树结构
        /// HouseType =1 药品库 =2 耗材库 =3 固定资产 =4 低值 =5 诊疗项目
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("WarehouseCatalog/tree/{HouseType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<WarehouseCatalogOutPut[]>> GetWarehouseCatalogTreeQueryableAsync([FromRoute] int HouseType = 0)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetWarehouseCatalogTreeQueryableAsync(HouseType);
                return new ServiceMessage<WarehouseCatalogOutPut[]>(result);
            });
        }

        //GetWarehouseCatalogQueryableAsync

        /// <summary>
        /// 库房目录list
        /// </summary>
        /// <returns></returns>
        [HttpPost("WarehouseCatalog/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<WarehouseCatalogOutPut[]>> GetWarehouseCatalogQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetWarehouseCatalogQueryableAsync();
                return new ServiceMessage<WarehouseCatalogOutPut[]>(result);
            });
        }

        /// <summary>
        /// 新增、修改库房目录结构 
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("WarehouseCatalog/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateWarehouseCatalogAsync([FromBody] WarehouseCatalogInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateWarehouseCatalogAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //   public Task<bool> DelWarehouseCatalogAsync(string Id )

        /// <summary>
        /// 删除指定库房目录
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("WarehouseCatalog/del/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelWarehouseCatalogAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.DelWarehouseCatalogAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }


        //  public Task<bool> CreateUpdateDialysisAsync(SupplierInPut input)

        /// <summary>
        ///  新增/修改供应商
        ///  Id为0.视为新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("Supplier/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateSupplierAsync([FromBody] SupplierInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateSupplierAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //
        /// <summary>
        /// 供应商列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Supplier/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SupplierOutPut[]>> GetSupplierQueryableAsync([FromBody] SupplierSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetSupplierQueryableAsync(input);
                return new ServiceMessage<SupplierOutPut[]>(result.Result, result.Count);
            });
        }
        //


        /// <summary>
        ///  删除供应商
        ///  Id为0.视为新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("Supplier/del/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> delSupplierAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.DeleteSupplierAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        ///  删除物品档案
        ///  Id为0.视为新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalItemRecord/del/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteMedicalItemRecordAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.DeleteMedicalItemRecordAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }


        /// <summary>
        /// 物品档案表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MedicalItemRecord/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalItemRecordOutPut[]>> GetMedicalItemRecordQueryableAsync([FromBody] MedicalItemRecordSearchInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalItemRecordQueryableAsync(input);
                return new ServiceMessage<MedicalItemRecordOutPut[]>(result.Result, result.Count);
            });
        }
        //
        //  public Task<ExportMedicalItem[]> ExportMedicalItemsAsync(int MedType)
        /// <summary>
        /// 导出物品档案信息
        /// </summary>
        /// <param name="MedType">档案类型1药品、2耗材、3固定资产、4低值</param>
        /// <returns></returns>
        [HttpGet("MedicalItemRecord/ExportList/{MedType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ExportMedicalItem[]>> ExportMedicalItemsAsync([FromRoute] int MedType)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.ExportMedicalItemsAsync(MedType);
                return new ServiceMessage<ExportMedicalItem[]>(result);
            });
        }
        ///// <summary>
        ///// 物品档案表
        ///// </summary> 
        ///// <returns></returns>
        //[HttpGet("MedicalItemRecord/abnormallist/{medType}")]
        //[CheckLogin]
        //[ServiceMessageTryCatch]
        //public virtual Task<ServiceMessage<MedicalItemRecordOutPut[]>> GetMedQueryableAsync([FromRoute]int medType)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var result = await _dataManager.GetMedQueryableAsync(medType);
        //        return new ServiceMessage<MedicalItemRecordOutPut[]>(result.Result, result.Count);
        //    });
        //}


        //[HttpPost("MedicalItemRecord/fuck")]
        //public virtual Task<bool> FUCK()
        //{

        //    return Task.Run(async () =>
        //    {
        //        var result = await _dataManager.SetSpecificationsQuantity();
        //        return true;
        //    });
        //}

        /// <summary>
        /// 物品档案详情
        /// </summary>
        /// <param name="id">物品Id</param>       
        /// <returns></returns>
        [HttpPost("MedicalItemRecordView/{id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalItemRecordOutPut>> GetMedicalItemRecordAsync([FromRoute] string id)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalItemRecordAsync(id);
                return new ServiceMessage<MedicalItemRecordOutPut>(result);
            });
        }
        /// <summary>
        ///  新增/修改药品档案
        ///  Id为0.视为新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalItemRecord/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateMedicalItemRecordAsync([FromBody] MedicalItemRecordInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateMedicalItemRecordAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //      public Task<bool> CrateUpdateMedicalPriceAsync(MedicalDrugExtensionInput input)

        /// <summary>
        ///  新增/修改药品价格  7.25 10.13 17.20
        ///  Id为0.视为新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalItemRecord/CreateUpdatePrice")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CrateUpdateMedicalPriceAsync([FromBody] MedicalDrugExtensionInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CrateUpdateMedicalPriceAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 单个物品各透析中心不同价格列表
        /// </summary>
        /// <param name="MedicalId">物品ID</param>
        /// <returns></returns>
        [HttpGet("MedicalItemRecord/GetPrice/{MedicalId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalDrugExtensionOutput[]>> GetMedicalDrugExtensionOutputs(string MedicalId)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalDrugExtensionOutputs(MedicalId);
                return new ServiceMessage<MedicalDrugExtensionOutput[]>(result);
            });
        }


        /// <summary>
        /// 启用禁用物品档案
        ///  DataState为1启用 2禁用
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalItemRecord/del/{Id}/{DataState}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> StateMedicalItemRecordAsync([FromRoute] string Id, int DataState)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.StateMedicalItemRecordAsync(Id, DataState);
                return new ServiceMessage<bool>(result);
            });
        }
        //  public Task<bool> ApplyStateMedicalItemRecordAsync(string id, int ApplyState)
        /// <summary>
        /// 允许或禁止采购
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ApplyState"></param>
        /// <returns></returns>
        [HttpGet("MedicalItemRecord/ApplyState/{Id}/{ApplyState}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ApplyStateMedicalItemRecordAsync([FromRoute] string Id, [FromRoute] int ApplyState)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.ApplyStateMedicalItemRecordAsync(Id, ApplyState);
                return new ServiceMessage<bool>(result);
            });
        }


        #region 剂型

        /// <summary>
        /// 剂型目录树结构

        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("DosageForm/tree")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DosageFormOutput[]>> GetDosageFormTreeQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetDosageFormTreeQueryableAsync();
                return new ServiceMessage<DosageFormOutput[]>(result);
            });
        }

        //GetDosageFormQueryableAsync

        /// <summary>
        /// 剂型目录list
        /// </summary>
        /// <returns></returns>
        [HttpPost("DosageForm/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DosageFormOutput[]>> GetDosageFormQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetDosageFormQueryableAsync();
                return new ServiceMessage<DosageFormOutput[]>(result);
            });
        }

        /// <summary>
        /// 新增、修改剂型目录结构 
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("DosageForm/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateDosageFormAsync([FromBody] DosageFormInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateDosageFormAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        //   public Task<bool> DelDosageFormAsync(string Id )

        /// <summary>
        /// 删除指定剂型目录
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("DosageForm/del/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelDosageFormAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.DelDosageFormAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }




        #endregion

        #region 单位

        /// <summary>
        /// 获取单位列表[]        
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalUnit/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<MedicalUnitOutput[]>> GetMedicalUnitQueryableAsync([FromBody] UnitQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalUnitQueryableAsync(input);
                return new ServiceMessage<MedicalUnitOutput[]>(result.Result, result.Count);
            });

        }


        /// <summary>
        /// 新增、修改单位 
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("MedicalUnit/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateMedicalUnitAsync([FromBody] MedicalUnitInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateMedicalUnitAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 批量启用、禁用单位
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MedicalUnit/Active")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> EnableUserAsync([FromBody] UnitActiveInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.EnableUnitAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        #endregion

        #region 用法用量

        /// <summary>
        /// 获取用法用量列表[]        
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("UseWay/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<UseWayOutput[]>> GetUseWayQueryableAsync([FromBody] UseWayQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetUseWayQueryableAsync(input);
                return new ServiceMessage<UseWayOutput[]>(result.Result, result.Count);
            });

        }


        /// <summary>
        /// 新增、修改用法用量
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("UseWay/CreateUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateUpdateUseWayAsync([FromBody] UseWayInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateUpdateUseWayAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 批量启用、禁用用法用量
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("UseWay/Active")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UseWayEnableUserAsync([FromBody] UnitActiveInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.UseWayEnableUnitAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        #endregion
        /// <summary>
        ///  医保药品目录查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SI/YPMLAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<yibao>> SI_YPMLAsync([FromBody] SIContrast input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.SI_YPMLAsync(input);
                return new ServiceMessage<yibao>(result);
            });
        }
        //以对照项目   public Task<yibao> YDZSI_YPMLAsync(SIContrast input)

        /// <summary>
        ///  以对照项目
        /// </summary> 
        /// <returns></returns>
        [HttpPost("SI/YDZYPMLAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<object>> YDZSI_YPMLAsync([FromBody] Si_YpmlQueryInput TYPE)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.NewYDZSI_YPMLAsync(TYPE);
                return new ServiceMessage<object>(result.Result, result.Count);
            });
        }

        //public Task<ZLXM> SI_ZLXM(Si_ZLXMQueryInput input)

        /// <summary>
        ///诊疗项目目录查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SI/ZLXMAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ZLXM>> SI_ZLXMAsync([FromBody] Si_ZLXMQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.SI_ZLXM(input);
                return new ServiceMessage<ZLXM>(result);
            });
        }

        /// <summary>
        /// 医保对照
        /// </summary>
        /// <param name="MedId">本地档案ID</param>
        /// <param name="siId">医保档案ID</param>
        /// <param name="type">1药品、2诊疗项目</param>
        /// <returns></returns>
        [HttpPost("SI/YPMLAsync/{MedId}/{siId}/{type}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> HIComparison([FromRoute] string MedId, int siId, int type)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.NewHIComparison(MedId, siId, type);
                return new ServiceMessage<bool>(result);
            });
        }

        // public Task<bool> QXDZSI_YPMLAsync(string ItemId)  

        /// <summary>
        /// 取消对照
        /// </summary>
        /// <param name="MedId">本地档案ID</param>       
        /// <returns></returns>
        [HttpPost("SI/Clear/{MedId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> HIClear([FromRoute] string MedId)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.QXDZSI_YPMLAsync(MedId);
                return new ServiceMessage<bool>(result);
            });
        }
        //取消对照


        /// <summary>
        ///  对照审核列表
        /// </summary>
        /// <param name="MedId">本地档案ID</param>       
        /// <returns></returns>
        [HttpPost("SI/GetMedMatchCodeDataAsync")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<YBDZMedicalItem[]>> GetMedMatchCodeDataAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedMatchCodeDataAsync();
                return new ServiceMessage<YBDZMedicalItem[]>(result);
            });
        }


        /// <summary>
        ///  对照审核
        /// </summary>
        /// <param name="input">本地档案ID</param>       
        /// <returns></returns>
        [HttpPost("SI/NewHIMatchCode")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> NewHIMatchCode([FromBody] MedMatchCode input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.NewHIMatchCode(input);
                return new ServiceMessage<bool>(result);
            });
        }


        #endregion

        /// <summary>
        /// 耗材
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/NewHC")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<HC_OutPut>> SI_HCAsync([FromBody] Si_ZLXMQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.SI_HCAsync(input);
                return new ServiceMessage<HC_OutPut>(result);
            });
        }
        /// <summary>
        /// 药品
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/NewYp")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<yibaoNew>> SI_YPAsync([FromBody] SIContrast input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.SI_YPAsync(input);
                return new ServiceMessage<yibaoNew>(result);
            });
        }
        //       public    Task<MedicalItemRecordOutPut[]> GetMedicalSICodeOutAsync()
        [HttpGet("SI/MedicalSICodeOut")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalItemRecord[]>> GetMedicalSICodeOutAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalSICodeOutAsync();
                return new ServiceMessage<MedicalItemRecord[]>(result);
            });
        }



        /// <summary>
        /// 诊疗项目目录
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/NewFWXM")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<HC_FWXM>> SI_NewZLXMAsync([FromBody] Si_ZLXMQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.SI_ZLXMAsync(input);
                return new ServiceMessage<HC_FWXM>(result);
            });
        }









        #region 版本

        //
        #region  中心端发布记录

        /// <summary>
        /// 获取中心端发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("Publish/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SysPublishInfoOutPut[]>> GetCenterPublishQueryableAsync([FromBody] SysPublishInfoQuery input)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.GetPublisQueryableAsync(input);
                return new ServiceMessage<SysPublishInfoOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 添加或修改中心端发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("Publish/AddOrUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddOrUpdatePublishAsync([FromBody] SysPublishInfoInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.AddUpdatePublishAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 删除中心端发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("Publish/Delete/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeletePublishAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.DelPublishAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        ///  发布记录列表推送至中心端
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("Publish/SendPublish/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SendPublishAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.SendPublishAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }




        /// <summary>
        /// 获取APP发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("APPPublish/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AppPublishInfoOutPut[]>> GetAPPPublishQueryableAsync([FromBody] SysPublishInfoQuery input)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.GetAPPPublisQueryableAsync(input);
                return new ServiceMessage<AppPublishInfoOutPut[]>(result.Result, result.Count);
            });
        }

        /// <summary>
        /// 添加或修改APP发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpPost("APPPublish/AddOrUpdate")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddOrUpdateAPPPublishAsync([FromBody] AppPublishInfoInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.AddUpdateAPPPublishAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 删除APP发布记录列表
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("APPPublish/Delete/{Id}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteAPPPublishAsync([FromRoute] string Id)
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.DelAPPPublishAsync(Id);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 获取APP最新一条发布记录列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("APPPublish/First")]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AppPublishInfoOutPut>> GetAPPPublishFirstAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _logManager.GetFirstAPPPublisQueryableAsync();
                return new ServiceMessage<AppPublishInfoOutPut>(result);
            });
        }
        //上次文件
        /// <summary>
        /// 上传文件(仅限中心端apk)
        /// </summary>
        /// <param name="fileName">文件名称</param>    
        /// <returns></returns>
        [HttpPost("packagefileup/{fileName}")]
        [ServiceMessageTryCatch]
        [DisableRequestSizeLimit]  //取消大小的限制
        public virtual Task<ServiceMessage<string>> UpLoadFile([FromRoute] string fileName)
        {

            string dir = Path.Combine(_documentSetting.ApkRoot);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
            {
                throw new Exception($"未能获取到上传文件信息");
            }
            try
            {
                var file = Request.Form.Files[0];
                string d = Path.GetExtension(fileName);//扩展名 “.docx” 

                string savePath = Path.Combine(dir, fileName);
                if (System.IO.File.Exists(savePath))
                    throw new Exception($"文件{fileName}已经存在");
                var size = file.Length;
                using (FileStream fs = System.IO.File.Create(savePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                //  _conversionMain.Start();http://192.168.10.245:9090/appApk/test.exe
            }
            catch (Exception ex)
            {
                throw new Exception(ex + "", ex);
            }
            return Task.FromResult(new ServiceMessage<string>(_documentSetting.BaseUrl + fileName));
        }


        #endregion

        #endregion



        #region  物品关联

        /// <summary>
        /// 批量新增物品关联
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MRelevancy/Create")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CreateMRelevancyAsync([FromBody] List<MedicalRelevancy> input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CreateMRelevancyAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 批量删除物品关联   
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MRelevancy/Delete")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DeleteMRelevancyAsync([FromBody] List<MedicalRelevancy> input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.DeleteMRelevancyAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 物品关联列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MRelevancy/GetALL")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MRelevancyOutPut[]>> GetMRelevancyOutPutAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMRelevancyOutPutAsync();
                return new ServiceMessage<MRelevancyOutPut[]>(result);
            });
        }
        #endregion

        #region 手动更新目录

        /// <summary>
        /// 药品
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/GetYPML")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MsgModel>> GetYPMLAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetYPMLAsync();
                return new ServiceMessage<MsgModel>(result);
            });
        }
        /// <summary>
        /// 耗材
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/GetHCML")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MsgModel>> GetHCMLAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetHCMLAsync();
                return new ServiceMessage<MsgModel>(result);
            });
        }
        /// <summary>
        /// 诊疗项目
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/FWML")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MsgModel>> GetFWMLAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetFWMLAsync();
                return new ServiceMessage<MsgModel>(result);
            });
        }

        /// <summary>
        /// 诊疗项目
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/XJML")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MsgModel>> GetXJAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetXJAsync();
                return new ServiceMessage<MsgModel>(result);
            });
        }
        #endregion
        /// <summary>
        ///获取7天内更新的目录信息
        /// </summary>   
        /// <returns></returns>
        [HttpPost("SI/UpdateReminder")]

        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DirectoryModel[]>> CatalogUpdateReminder([FromBody] QualityControlQueryInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.CatalogUpdateReminder(input);
                return new ServiceMessage<DirectoryModel[]>(result.Result, result.Count);
            });
        }
        // GetChronicSpecialAsync(string data)

        [HttpPost("SI/GetChronicSpecial")]
        //[CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>> GetChronicSpecialAsync([FromForm] string data)
        {
            return Task.Run(async () =>

            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    return new ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>(new Exception("参数为空"));
                }
                var model = JsonHelper.JsonToT<ChronicSpecialDiseaseDrugRecordModel>(data);
                if (string.IsNullOrWhiteSpace(model.Id))
                {
                    return new ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>(new Exception("患者ID为空"));
                }

                if ((model.endtime - model.begntime).Days > 31)
                    return new ServiceMessage<ChronicSpecialDiseaseDrugRecordResponseModel>(new Exception("查询时间间隔不能大于31天"));

                var result = await _dataManager.GetChronicSpecialAsync(data);

                return result;
            });
        }
        //GetMedHisCatalogOutPutAsync

        [HttpGet("SI/GetMedHisCatalog/{MedType}")]
        //[CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedHisCatalogOutPut[]>> GetMedHisCatalogOutPutAsync([FromRoute] int MedType)
        {
            return Task.Run(async () =>
            {
                List<int> list = new List<int>();
                list.Add(1);
                list.Add(2);
                list.Add(3);
                if (!list.Contains(MedType))
                {
                    return new ServiceMessage<MedHisCatalogOutPut[]>(new Exception("请提供有效的MedType"));
                }

                var result = await _dataManager.GetMedHisCatalogOutPutAsync(MedType);

                return new ServiceMessage<MedHisCatalogOutPut[]>(result.ToArray());
            });
        }

        //  public Task<bool> SI_MedMonthAudit(AuditInput input)
        [HttpPost("SI/MedMonthAudit")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SI_MedMonthAudit([FromBody] AuditInput input)
        {
            return Task.Run(async () =>
            {

                if (input == null)
                {
                    return new ServiceMessage<bool>(new Exception("请提供有效的参数"));
                }

                var result = await _dataManager.SI_MedMonthAudit(input);

                return new ServiceMessage<bool>(result);
            });
        }


    }
}
