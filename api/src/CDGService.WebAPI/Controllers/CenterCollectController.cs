
using CDGService.Data.Datas;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    /// <summary>   
    /// 中心端采集接口     
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class CenterCollectController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly Data.DocumentSetting _documentSetting;
        private readonly EmployeeManger _employeeManger;
        private readonly IGetUserInfo _getUserInfo;
        private readonly CenterDialysisManger _CenterDialysisManger;
        private readonly InformationManager _InformationManager;
        private readonly CenterDockingManger _CenterDockingManger;
        private string DialysisId = "";
        /// <summary>
        /// 中心端采集接口     
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="documentSetting"></param>
        public CenterCollectController(DataManager dataManager, IGetUserInfo getUserInfo, Microsoft.Extensions.Options.IOptions<Data.DocumentSetting> documentSetting, EmployeeManger employeeManger, CenterDialysisManger centerDialysisManger, InformationManager informationManager, CenterDockingManger centerDockingManger)
        {
            _dataManager = dataManager;
            _documentSetting = documentSetting.Value;
            _employeeManger = employeeManger;
            _getUserInfo = getUserInfo;
            _CenterDialysisManger = centerDialysisManger;
            _InformationManager = informationManager;
            _CenterDockingManger = centerDockingManger;
        }

        /// <summary> 
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDateTime")]
        public virtual ServiceMessage<string> GetDateTime()
        {
            var NowDate = DateTime.Now;
            return new ServiceMessage<string>(NowDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        #region  员工

        /// <summary>
        ///  获取员工数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>       
        /// <returns></returns>
        [HttpGet("Employee")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<EmployeeOutPut[]>> GetEmployeeQueryableAsync()
        {

            return Task.Run(async () =>
            {
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                //string Token = _getUserInfo.Token;
                //switch (Token)
                //{
                //    case "ab29aa0ae0644549a6bfb9639889f54b"://康美
                //        DialysisId = "abc3d60b474c4fef946a66887348c41a";
                //        break;
                //    case "ab29aa0ae0644549a6bfb9639889f54a"://秀山
                //        DialysisId = "c07406f87d0946c99a031be5034382ec";
                //        break;
                //    default:
                //        throw new Exception("无效token");
                //        break;
                //}
                //user.Employee.CenterDialysis.Id
                // var user = await _getUserInfo.Token;
                //if (user == null || user.Employee == null || user.Employee.CenterDialysis == null)
                //    throw new Exception("无效token");
                if (DialysisId == "") throw new Exception("无效token");
                //忠县 陶家 沙坪坝 
                var result = await _employeeManger.GetDialysisQueryableAsync(new EmployeeQueryInput() { DialysisId = DialysisId, PageNum = 1, pageSize = 99999 });
                return new ServiceMessage<EmployeeOutPut[]>(result.Result, result.Count);
            });
        }

        #endregion

        #region 档案

        /// <summary>
        /// 获取单位列表     
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <returns></returns>
        [HttpGet("MedicalUnit/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<MedicalUnitOutput[]>> GetMedicalUnitQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetMedicalUnitQueryableAsync();
                return new ServiceMessage<MedicalUnitOutput[]>(result.Result, result.Count);
            });

        }
        /// <summary>
        /// 剂型目录
        /// </summary>
        /// <returns></returns>
        [HttpGet("DosageForm/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DosageFormCenterOutput[]>> GetDosageFormQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetCenterDosageFormQueryableAsync();
                return new ServiceMessage<DosageFormCenterOutput[]>(result);
            });
        }

        /// <summary>
        /// 获取用法用量列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("UseWay/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public Task<ServiceMessage<UseWayOutput[]>> GetUseWayQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetUseWayQueryableAsync();
                return new ServiceMessage<UseWayOutput[]>(result.Result, result.Count);
            });

        }

        /// <summary>
        /// 供应商列表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("Supplier/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<Supplier[]>> GetSupplierQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetSupplierQueryableAsync();
                return new ServiceMessage<Supplier[]>(result);
            });
        }
        /// <summary>
        /// 物品类型目录
        /// </summary>
        /// <returns></returns>
        [HttpGet("WarehouseCatalog/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<CenterWarehouseCatalogOutPut[]>> GetWarehouseCatalogQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetCenterWarehouseCatalogQueryableAsync();
                return new ServiceMessage<CenterWarehouseCatalogOutPut[]>(result);
            });
        }
        /// <summary>
        /// 物品档案表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("MedicalItemRecord/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalItemRecord[]>> GetMedicalItemRecordQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                var result = await _dataManager.GetMedicalItemRecordQueryableAsync(DialysisId);
                return new ServiceMessage<MedicalItemRecord[]>(result);
            });
        }

        //价格GetCenterMedicalDrugExtensionsQueryableAsync
        /// <summary>
        /// 物品价格表
        /// </summary> 
        /// <returns></returns>
        [HttpGet("MedicalDrugExtension/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MedicalDrugExtension[]>> GetCenterMedicalDrugExtensionsQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                // string DialysisId = "abc3d60b474c4fef946a66887348c41a";
                //string Token = _getUserInfo.Token;
                //switch (Token)
                //{
                //    case "ab29aa0ae0644549a6bfb9639889f54b"://康美
                //        DialysisId = "abc3d60b474c4fef946a66887348c41a";
                //        break;
                //    case "ab29aa0ae0644549a6bfb9639889f54a"://秀山
                //        DialysisId = "c07406f87d0946c99a031be5034382ec";
                //        break;
                //    default:
                //        throw new Exception("无效token");
                //        break;
                //}
                if (DialysisId == "") throw new Exception("无效token");
                var result = await _dataManager.GetCenterMedicalDrugExtensionsQueryableAsync(DialysisId);
                return new ServiceMessage<MedicalDrugExtension[]>(result);
            });
        }
        //
        [HttpGet("bzml/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SI_BZML[]>> GetSI_BZMLAsync()
        {
            return Task.Run(async () =>
            {
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                if (DialysisId == "") throw new Exception("无效token");
                var result = await _dataManager.GetSI_BZMLAsync();
                return new ServiceMessage<SI_BZML[]>(result);
            });
        }
        #endregion

        #region 数据字典

        //类型(人员相关-学历、性别、人员类型、职称等级、在职情况) 
        /// <summary>
        /// 类型(人员相关-学历、性别、人员类型、职称等级、在职情况) 、费用类别
        /// </summary> 
        /// <returns></returns> 
        [HttpGet("DicType/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<DictionaryTypeOutPut[]>> GetDicTypeQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetDictionaryTypesQueryableAsync();
                return new ServiceMessage<DictionaryTypeOutPut[]>(result);
            });
        }

        //字典数据
        //GetSysDictionaryQueryableAsync
        /// <summary>
        /// 字典详细数据（人员相关-学历、性别、人员类型、职称等级、在职情况) 费用类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("SysDic/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SystemDictionaryOutPut[]>> GetSysDictionaryQueryableAsync()
        {
            return Task.Run(async () =>
            {
                var result = await _dataManager.GetSysDictionaryQueryableAsync();
                return new ServiceMessage<SystemDictionaryOutPut[]>(result);
            });
        }

        /// <summary>
        /// 中心端撤销报废申请审核
        /// </summary>
        /// <param name="MapId"></param>
        /// <returns></returns>
        [HttpGet("ReturnMaterialApply/{MapId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ReturnMapproveAsync([FromRoute] string MapId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.ReturnMapproveAsync(MapId);
                return new ServiceMessage<bool>(result);
            });
        }

        #endregion

        #region  机构
        [HttpGet("Dialysis")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OrganizationInfo>> GetDialysisAsync()
        {
            return Task.Run(async () =>
            {
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                var result = await _CenterDialysisManger.GetDialysisAsync(DialysisId);
                return new ServiceMessage<OrganizationInfo>(result);
            });
        }

        #endregion


        //通知反馈 InformationFeedbackasync         public Task<bool> InformationFeedbackasync(InformationFeedback feedback)

        /// <summary>
        /// 中心端通知反馈
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        [HttpPost("InformationFeedback")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> InformationFeedbackasync([FromBody]InformationFeedback feedback)
        {
            return Task.Run(async () =>
            {

                if (feedback == null)
                {
                    throw new Exception($"请提供有效参数");
                }
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;


                if (DialysisId != feedback.CenterId)
                {
                    throw new Exception($"token与centerid不匹配");
                }
                var result = await _InformationManager.InformationFeedbackasync(feedback);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        /// 中心端添加订单
        /// </summary>
        /// <param name="purchaseOrder"></param>
        /// <returns></returns>
        [HttpPost("AddPurchaseOrder")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> PurchaseOrderasync([FromBody]PurchaseOrder purchaseOrder)
        {
            return Task.Run(async () =>
            {

                if (purchaseOrder == null)
                {
                    throw new Exception($"请提供有效参数");
                }
                var token = await _getUserInfo.GetUserToken();
                var DialysisId = token.CenterId;
                purchaseOrder.CenterId = DialysisId;
                purchaseOrder.OrderDetails.ForEach(t=>t.CenterId = DialysisId);
                purchaseOrder.GroupAuditStatus = "2";
                var result = await _CenterDockingManger.AddCenterOrder(purchaseOrder);
                return new ServiceMessage<bool>(result);
            });
        }


    }
}
