using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{

    public interface IDataReceive
    {
        bool AddToken(string token, string ClientType, string Account);
        /// <summary>
        /// 用户登录 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<CenterLogionResult> UserLoginAsync(string username, string password);

        #region  报废、滞销
        Task<bool> GroupUpdateMatState(string CenterId, dynamic pas = null);
        Task<bool> GroupUpdateMaterialsWarningState(string CenterId, dynamic pas = null);
        Task<MaterialApplyApprove[]> GetMaterialApplyApproveAsync();
        Task<MaterialApplyApproveDetail[]> GetMaterialApplyApproveDetailAsync();
        Task<MaterialsWarningApplyList[]> GetMaterialsWarningApplyListAsync();
        Task<MaterialsWarningApplyDetailList[]> GetMaterialsWarningApplyDetailListAsync();
        Task<bool> MaterialPendingApproval(string CenterId, dynamic pas = null);
        Task<bool> WarningPendingApproval(string CenterId, dynamic pas = null);
        #endregion


        #region 采购

        Task<CenterEmployee[]> GetEmployeeList();
        /// <summary>
        /// 采购申请单
        /// </summary>
        /// <returns></returns>
        Task<PurchaseRequest[]> GetPurchasManagerAsync();
        /// <summary>
        /// 采购申请单细明
        /// </summary>
        /// <returns></returns>
        Task<PurchaseDetail[]> GetPurchaseDetailDataAsync();
        Task<Supplier[]> GetSupplierDataAsync();
        Task<MedicalItemRecord[]> GetMedicalItemRecordDataAsync();
        Task<bool> GetSIBaseDataAsync(string HisCode);
        /// <summary>
        /// 采购单审批
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> UpdatePurchasState(string CenterId, dynamic pas = null);

        /// <summary>
        /// 采购单审批调整物品价格
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> UpdatePurchasPriceState(string CenterId, dynamic pas = null);


        /// <summary>
        /// 删除采购明细
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> DelPurchaseDetails(string CenterId, dynamic pas = null);
        /// <summary>
        /// 添加采购明细
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> AddPurchaseDetails(string CenterId, dynamic pas = null);
        /// <summary>
        /// 关闭中心端采购明细
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<ServiceMessage<object>> ClosePurchaseDetails(string CenterId, dynamic pas = null);

        /// <summary>
        /// 推送采购流程至中心端
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> pushPurchaseApprovesl(string CenterId, dynamic pas = null);

        /// <summary>
        /// 低值推送采购流程至中心端
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> dzpushPurchaseApprovesl(string CenterId, dynamic pas = null);
        /// <summary>
        /// 采购申请单审批部分
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> PushPurchaseDetail(string CenterId, dynamic pas = null);
        /// <summary>
        /// 关闭中心端采购申请单
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> ClosePurchase(string CenterId, dynamic pas = null);

        /// <summary>
        /// 退货审批
        /// </summary>
        /// <param name="CenterId"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        Task<bool> ReturnGoods(string CenterId, dynamic pas = null);
        #endregion
        Task<T[]> GetCollectDataAsync<T>(string Url);




        /// <summary>
        /// 推送发布信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<bool> SendPublishVer(SysPublishInfo sysPublishInfo);


        /// <summary>
        /// 推送公告信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<bool> SendPublishNotices(CenterNotices centerNotices);


        #region  手动更新目录

        Task<bool> GetYPMLAsync();

        Task<bool> GetHCMLAsync();
        Task<bool> GetFWMLAsync();
        Task<bool> GetXJAsync();


        Task<string> GetChronicSpecialAsync(string data);






        #endregion
    }
}
