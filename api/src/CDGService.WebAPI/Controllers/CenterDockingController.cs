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
using System.Threading.Tasks;

namespace CDGService.WebAPI.Controllers
{
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class CenterDockingController : Controller
    {
        private readonly CenterDockingManger _CenterDockingManger;

        public CenterDockingController(CenterDockingManger centerDockingManger)
        {
            _CenterDockingManger = centerDockingManger;
        }

        /// <summary>
        ///  获取采购申请单数据列表
        ///  参数为空视为查询所有数据
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="input">查询透析中心条件参数</param>
        /// <returns></returns>
        [HttpPost("PurchaseRequestList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseRequestOutPut[]>> GetPurchaseRequestQueryableAsync([FromBody] PurchaseRequestQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetPurchaseRequestQueryableAsync(input);
                return new ServiceMessage<PurchaseRequestOutPut[]>(result.Result, result.Count);
            });
        }


        //   public Task<PageData<PurchaseRequestOutPut[]>> CollectGetPurchaseRequestQueryableAsync(PurchaseRequestQueryInput input)
        /// <summary>
        /// 立即采集采购申请单和申请单明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CollectGetPurchaseRequestList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseRequestOutPut[]>> CollectGetPurchaseRequestQueryableAsync([FromBody] PurchaseRequestQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CollectGetPurchaseRequestQueryableAsync(input);
                return new ServiceMessage<PurchaseRequestOutPut[]>(result.Result, result.Count);
            });
        }

        //
        /// <summary>
        ///  获取采购申请单明细数据列表       
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="PurchaseRequestId">采购单ID</param>
        /// <returns></returns>
        [HttpPost("PurchaseDetailList/{PurchaseRequestId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseOutPut>> GetPurchaseDetailQueryableAsync([FromRoute] string PurchaseRequestId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetPurchaseDetailQueryableAsync(PurchaseRequestId);
                return new ServiceMessage<PurchaseOutPut>(result);
            });
        }
        //  public Task<PurchaseOutPut> GetPurchaseDetailQueryableAsync(PurchaseDetailQueryInPut inPut)

        /// <summary>
        ///  获取采购申请单明细数据列表-多采购单
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("PurchaseDetail/batchList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<BatchPurchaseOutPut>> GetPurchaseDetailbatchListAsync([FromBody] PurchaseDetailQueryInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetPurchaseDetailQueryableAsync(inPut);
                return new ServiceMessage<BatchPurchaseOutPut>(result);
            });
        }
        //ApprovalPurchaseRequestAsync

        /// <summary>
        /// 审核
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [HttpPost("ApprovalPurchaseDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ApprovalPurchaseRequestAsync([FromBody] ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.ApprovalPurchaseRequestAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 调整申请单明细价格
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [HttpPost("UpdatePurchasPrice")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdatePurchasPriceState([FromBody] PurchaseDetaiPriceInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.UpdatePurchasPriceState(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        //        public Task<bool> DelPurchasDetilAsync(string Id)

        //
        /// <summary>
        ///  删除指定采购明细
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="PurchaseDetiId">明细单ID</param>
        /// <returns></returns>
        [HttpPost("PurchaseDetail/del")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> DelPurchasDetilAsync([FromBody] PurchaseDetailDelInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.DelPurchasDetilAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        //     public Task<bool> AddPurchaseDetailAsync(PurchaseDetailInPut input)
        /// <summary>
        /// 新增采购明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("PurchaseDetail/Add")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddPurchaseDetailAsync([FromBody] PurchaseDetailInPut input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.AddPurchaseDetailAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 拆分采购明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("PurchaseDetail/SplitAdd")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> PurchaseDetailSplitAsync([FromBody] PurchaseDetailSplit input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.PurchaseDetailSplitAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }
        #region 订单

        //      public Task<bool> PurchasToOrder(OrderInput inPut)
        /// <summary>
        /// 采购申请单生成采购订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("PurchaseDetail/PurchasToOrder")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> PurchasToOrder([FromBody] OrderInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.PurchasToOrder(input);
                return new ServiceMessage<bool>(result);
            });
        }
        //
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="OrderId">订单ID</param>
        /// <returns></returns>
        [HttpGet("DelOrder/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> PurchasOrderDel([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.PurchasOrderDel(OrderId);
                return new ServiceMessage<bool>(result);
            });
        }
        //修改订单  public Task<bool> UpdatePurchasOrder(PurchaseOrderInPut inPut)
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="inPut">订单</param>
        /// <returns></returns>
        [HttpPost("Order/Update")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdatePurchasOrder([FromBody] PurchaseOrderInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.UpdatePurchasOrder(inPut);
                return new ServiceMessage<bool>(result);
            });
        }
        //   public Task<PageData<OrderQueryOutPut[]>> GetOrderDataAsync(OrderQueryInput input)
        /// <summary>
        /// 查询订单列表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Order/List")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OrderQueryOutPut[]>> GetOrderDataAsync([FromBody] OrderQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetOrderDataAsync(inPut);
                return new ServiceMessage<OrderQueryOutPut[]>(result.Result, result.Count);
            });
        }
        //

        /// <summary>
        /// 查询订单明细列表
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet("OrderDetail/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OrderDetailOutPut>> GetOrderDetailAsync([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetOrderDetailAsync(OrderId);
                return new ServiceMessage<OrderDetailOutPut>(result);
            });
        }
        //   public Task<bool> UpdateOrderDetailAsync(OrderDetailUpdateInPut input)
        /// <summary>
        /// 修改订单明细
        /// </summary>
        /// <param name="inPut">订单明细</param>
        /// <returns></returns>
        [HttpPost("OrderDetail/Update")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateOrderDetailAsync([FromBody] OrderDetailUpdateInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.UpdateOrderDetailAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        //    public Task<bool> CloseOrderAsync(string OrderId)

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="input">订单明细</param>
        /// <returns></returns>
        [HttpPost("OrderDetail/CloseOrder")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CloseOrderAsync([FromBody] OrderCloseInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CloseOrderAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }


        /// <summary>
        /// 关闭订单明细
        /// </summary>
        /// <param name="input">订单明细</param>
        /// <returns></returns>
        [HttpPost("OrderDetail/CloseOrderDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> CloseOrderDetailAsync([FromBody] OrderDetailCloseInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CloseOrderDetailAsync(input);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        /// 拆分订单
        /// </summary>
        /// <param name="OrderId">订单ID</param>
        /// <returns></returns>
        [HttpGet("OrderDetail/SplitOrder/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> SplitOrder([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.SplitOrder(OrderId);
                return new ServiceMessage<bool>(result);
            });
        }
        // public Task<bool> SplitOrder(string OrderId)



        /// <summary>
        /// 采购信息推送至中心端（采购单下所有明细都推送则下发至中心端）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet("OrderDetail/PushCenter/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> PushCenter([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.PushCenter(OrderId);
                return new ServiceMessage<bool>(result);
            });
        }
        #endregion

        #region 退货

        // public Task<PageData<ReturnRequestOutPut[]>> GetReturnAsync(ReturnQuerInput input)

        [HttpPost("ReturnRequest/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ReturnRequestOutPut[]>> GetReturnAsync([FromBody] ReturnQuerInput ReturnQuerInput)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetReturnAsync(ReturnQuerInput);
                return new ServiceMessage<ReturnRequestOutPut[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        /// 立即获取退货列表
        /// </summary>
        /// <param name="ReturnQuerInput"></param>
        /// <returns></returns>
        [HttpPost("ReturnRequestNow/list")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ReturnRequestOutPut[]>> CollectGetRequestQueryableAsync([FromBody] ReturnQuerInput ReturnQuerInput)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CollectGetRequestQueryableAsync(ReturnQuerInput);
                return new ServiceMessage<ReturnRequestOutPut[]>(result.Result, result.Count);
            });
        }
        //    public Task<ReturnDetailsOutPut[]> GetreturnDetailsAsync(string ReturnId)

        [HttpGet("ReturnDetails/list/{ReturnId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ReturnDetailsOutPut[]>> GetreturnDetailsAsync([FromRoute] string ReturnId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetreturnDetailsAsync(ReturnId);
                return new ServiceMessage<ReturnDetailsOutPut[]>(result);
            });
        }


        //   public Task<bool> UpdatereturnDetailsAsync(ReturnDetailsInPut inPut)



        [HttpPost("ReturnDetails/UpdatereturnDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdatereturnDetailsAsync([FromBody] ReturnDetailsInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.UpdatereturnDetailsAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }


        [HttpPost("ReturnDetails/Examine")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ReturnExamine([FromBody] ReturnExamineInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.ReturnExamine(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        #endregion

        #region 采购新流程
        //NewGetPurchaseRequestQueryableAsync

        [HttpPost("NewPurchaseRequestList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseRequestOutPut[]>> NewGetPurchaseRequestQueryableAsync([FromBody] PurchaseRequestQueryInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewsGetPurchaseRequestQueryableAsync(input);
                return new ServiceMessage<PurchaseRequestOutPut[]>(result.Result, result.Count);
            });
        }
        [HttpPost("UpdateTest")]

        public virtual Task<ServiceMessage<bool>> UpdateTest([FromBody] PurchaseRequestQueryInput input)
        {
            return Task.Run(async () =>
            {
                //var result = await _CenterDockingManger.UpdateTest();
                return new ServiceMessage<bool>(false);
            });
        }
        //申请单审批
        // public Task<bool> NewApprovalPurchaseRequestAsync(ApprovalPurchaseRequestInput Input)
        [HttpPost("NewApprovalPurchaseDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> NewApprovalPurchaseRequestAsync([FromBody] ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewApprovalPurchaseRequestAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        //   public Task<PurchaseOutPut> NewGetPurchaseDetailQueryableAsync(string PurchaseRId)
        [HttpPost("NewPurchaseDetailList/{PurchaseRequestId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseOutPut>> NewGetPurchaseDetailQueryableAsync([FromRoute] string PurchaseRequestId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewGetPurchaseDetailQueryableAsync(PurchaseRequestId);
                return new ServiceMessage<PurchaseOutPut>(result);
            });
        }

        //生成订单        public Task<bool> NewPurchasToOrder(OrderInput inPut)

        /// <summary>
        /// 采购申请单生成采购订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("NewPurchasToOrder/PurchasToOrder")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> NewPurchasToOrder([FromBody] OrderInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewPurchasToOrder(input);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 查询订单列表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("Order/NewList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OrderQueryOutPut[]>> NewGetOrderDataAsync([FromBody] OrderQueryInput inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewsGetOrderDataAsync(inPut);
                return new ServiceMessage<OrderQueryOutPut[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        ///新 订单明细
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet("NewOrderDetail/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OrderDetailOutPut>> NewGetOrderDetailAsync([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.NewGetOrderDetailAsync(OrderId);
                return new ServiceMessage<OrderDetailOutPut>(result);
            });
        }
        /// <summary>
        /// 新审核
        /// 把登录操作获取的token，放到header里，key="token"
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [HttpPost("OrderApproval")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> OrderPurchaseRequestAsync([FromBody] ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.OrderPurchaseRequestAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        //定价
        [HttpGet("OrderPricing/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> OrderPricingAsync([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.OrderPricingAsync(OrderId);
                return new ServiceMessage<bool>(result);
            });
        }

        //NewPushCenter 推送
        /// <summary>
        /// 采购信息推送至中心端（采购单下所有明细都推送则下发至中心端）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet("OrderDetail/NewPushCenter/{OrderId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> NewPushCenter([FromRoute] string OrderId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.PartOrderPushCenter(OrderId);
                return new ServiceMessage<bool>(result);
            });
        }
        [HttpPost("NewPushCenter/BatchPush")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> NewBatchPushCenter([FromBody] batchPurchModel OrderIds)
        {
            return Task.Run(async () =>
            {
                if (OrderIds == null || OrderIds.OrderId.Length <= 0)
                    return new ServiceMessage<bool>(new Exception($"请提供有效参数"));
                int i = 0;

                foreach (var item in OrderIds.OrderId)
                {
                    try
                    {
                        var result = await _CenterDockingManger.PartOrderPushCenter(item);
                        if (result)
                            i++;
                    }
                    catch (Exception)
                    {

                    }
                }
                //var result = await _CenterDockingManger.PartOrderPushCenter(OrderId);
                if (i == OrderIds.OrderId.Length)
                    return new ServiceMessage<bool>(true);
                else
                    return new ServiceMessage<bool>(new Exception($"已成功推送{i}条，剩余{OrderIds.OrderId.Length - i}条推送失败，请稍后再试！"));


            });
        }

        /// <summary>
        /// 合并（未审批）采购申请单
        /// </summary>
        /// <param name="PurchaseId">采购申请单ID数组</param>
        /// <returns></returns>
        [HttpPost("Purchase/GroupMergePurchase")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PurchaseOutPut>> GroupMergePurchaseAsync([FromBody] String[] PurchaseId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GroupMergePurchaseAsync(PurchaseId);
                return new ServiceMessage<PurchaseOutPut>(result);
            });
        }
        //       public Task<bool> MergePurchaseBreakAsync(MergePurchaseBreakInput input)

        /// <summary>
        /// 从合并单里面拆分出来
        /// </summary>
        /// <param name="PurchaseId">参数</param>
        /// <returns></returns>
        [HttpPost("Purchase/MergePurchaseBreak")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MergePurchaseBreakAsync([FromBody] MergePurchaseBreakInput PurchaseId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.MergePurchaseBreakAsync(PurchaseId);
                return new ServiceMessage<bool>(result);
            });
        }
        /// <summary>
        /// 新采购单并入合并单
        /// </summary>
        /// <param name="PurchaseId">参数</param>
        /// <returns></returns>
        [HttpPost("Purchase/MergePurchaseAdd")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MergePurchaseAddAsync([FromBody] MergePurchaseBreakInput PurchaseId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.MergePurchaseAddAsync(PurchaseId);
                return new ServiceMessage<bool>(result);
            });
        }
        #endregion


        #region 采购审批权限配置AddApprovalConfig

        /// <summary>
        /// 新增审核流程
        /// </summary>
        /// <param name="approvalConfig"></param>
        /// <returns></returns>
        [HttpPost("Purchase/AddApprovalConfig")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AddApprovalConfigAsync([FromBody] ApprovalConfig[] approvalConfig)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.AddApprovalConfig(approvalConfig);
                return new ServiceMessage<bool>(result);
            });
        }
        ///// <summary>
        ///// 修改审核流程
        ///// </summary>
        ///// <param name="approvalConfig"></param>
        ///// <returns></returns>
        //[HttpPost("Purchase/UpdateApprovalConfig")]
        //[CheckLogin]
        //[ServiceMessageTryCatch]
        //public virtual Task<ServiceMessage<bool>> UpdateApprovalConfigAsync([FromBody]ApprovalConfig approvalConfig)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var result = await _CenterDockingManger.UpdateApprovalConfig(approvalConfig);
        //        return new ServiceMessage<bool>(result);
        //    });
        //}
        /// <summary>
        /// 获取审核流程
        /// </summary>
        /// <param name="ApprovalType">类型1 采购申请 2 采购订单</param>
        /// <returns></returns>
        [HttpGet("Purchase/GetApprovalConfig/{ApprovalType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ApprovalConfig[]>> GetApprovalConfigAsync([FromRoute] int ApprovalType)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetApprovalConfigAsync(ApprovalType);
                return new ServiceMessage<ApprovalConfig[]>(result);
            });
        }

        ///// <summary>
        ///// 删除审核流程
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //[HttpGet("Purchase/DelApprovalConfig/{Id}")]
        //[CheckLogin]
        //[ServiceMessageTryCatch]
        //public virtual Task<ServiceMessage<bool>> DelApprovalConfigAsync([FromRoute] string Id)
        //{
        //    return Task.Run(async () =>
        //    {
        //        var result = await _CenterDockingManger.DelApprovalConfig(Id);
        //        return new ServiceMessage<bool>(result);
        //    });
        //}


        /// <summary>
        /// 预览审核流程
        /// </summary>
        /// <param name="ApprovalType">类型1 采购申请 2 采购订单</param>
        /// <returns></returns>
        [HttpGet("Purchase/ApprovalConfigPreview/{ApprovalType}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ApprovalProcessOutPut[]>> GetApprovalConfigPreviewAsync([FromRoute] int ApprovalType)
        {
            return Task.Run(async () =>
            {
                //   private Task<ApprovalProcessOutPut[]> GetApprovalConfigPreviewAsync(int ApprovalType)
                var result = await _CenterDockingManger.GetApprovalConfigPreviewAsync(ApprovalType);
                return new ServiceMessage<ApprovalProcessOutPut[]>(result);
            });
        }

        #endregion



        #region 
        //  public Task<PageData<MaterialApplyApproveOutPut[]>> GetMapproveAsync(ReturnQuerInput input)
        [HttpPost("GetMapprove")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MaterialApplyApproveOutPut[]>> GetMapproveAsync([FromBody] ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetMapproveAsync(input);
                return new ServiceMessage<MaterialApplyApproveOutPut[]>(result.Result, result.Count);
            });
        }

        [HttpPost("CollectGetMaterialsWarningApplyList")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MMaterialsWarningApplyListOutPut[]>> CollectGetMaterialsWarningApplyListAsync([FromBody] ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CollectGetMaterialsWarningApplyListAsync(input);
                return new ServiceMessage<MMaterialsWarningApplyListOutPut[]>(result.Result, result.Count);
            });
        }

        [HttpPost("CollectGetCollectGetMaterialApplyApprove")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MaterialApplyApproveOutPut[]>> CollectGetMaterialApplyApproveAsync([FromBody] ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.CollectGetMaterialApplyApproveAsync(input);
                return new ServiceMessage<MaterialApplyApproveOutPut[]>(result.Result, result.Count);
            });
        }

        // public Task<mapDetailOutPut> GetmappDetailsAsync(string AppId)
        [HttpGet("GetmappDetails/{AppId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<mapDetailOutPut>> GetmappDetailsAsync([FromRoute] string AppId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetmappDetailsAsync(AppId);
                return new ServiceMessage<mapDetailOutPut>(result);
            });
        }




        //  public Task<bool> MaterialApplyRequestAsync(ApprovalPurchaseRequestInput Input)
        [HttpPost("MaterialApplyRequest")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MaterialApplyRequestAsync([FromBody] ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.MaterialApplyRequestAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 固定报废明细估值   public Task<bool> FixedAssetsScrapValueAsync(FixedAssetsScrapInput input)
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [HttpPost("FixedAssetsScrap")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> FixedAssetsScrapValueAsync([FromBody] FixedAssetsScrapInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.FixedAssetsScrapValueAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }


        /// <summary>
        /// 确定估值
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [HttpPost("AscertainFixedAssetsScrap")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> AscertainFixedAssetsScrapAsync([FromBody] FixedAssetsScrapInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.AscertainFixedAssetsScrapAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }







        //  public Task<PageData<MMaterialsWarningApplyListOutPut[]>> GetMMaterialsWarningApplyListAsync(ReturnQuerInput input)


        /// <summary>
        /// 滞销
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetMaterialsWarning")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MMaterialsWarningApplyListOutPut[]>> GetMMaterialsWarningApplyListAsync([FromBody] ReturnQuerInput input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetMMaterialsWarningApplyListAsync(input);
                return new ServiceMessage<MMaterialsWarningApplyListOutPut[]>(result.Result, result.Count);
            });
        }


        /// <summary>
        ///   滞销明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>


        [HttpGet("GetMaterialsWarningDetails/{AppId}")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MaterialsWarningOutPut>> GetMaterialsWarningDetailsAsync([FromRoute] string AppId)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.GetMaterialsWarningDetailsAsync(AppId);
                return new ServiceMessage<MaterialsWarningOutPut>(result);
            });
        }
        // public Task<bool> MaterialsWarningRequestAsync(ApprovalPurchaseRequestInput Input)

        [HttpPost("MaterialsWarningRequest")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> MaterialsWarningRequestAsync([FromBody] ApprovalPurchaseRequestInput Input)
        {
            return Task.Run(async () =>
            {
                var result = await _CenterDockingManger.MaterialsWarningRequestAsync(Input);
                return new ServiceMessage<bool>(result);
            });
        }

        #endregion





























        [HttpGet("StockExcessive")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<StockExcessive>> GetGoodsInStockdCY()
        {
            return Task.Run(async () =>
            {
                StockExcessive Excessive = new StockExcessive();

                var result = await _CenterDockingManger.GetGoodsInStockdCY();
                if (result > 0)
                    Excessive.StockExcessiveCount = result;
                else
                    Excessive.StockExcessiveCount = 0;
                return new ServiceMessage<StockExcessive>(Excessive);
            });
        }


    }
    public class StockExcessive
    {
        public int StockExcessiveCount { get; set; }
    }
}
