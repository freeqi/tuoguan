using CDGService.Data.Helper;
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
    /// <summary>
    /// 物资报表
    /// </summary>
    [EnableCors("any")] //启用跨域
    [Route("api/[controller]")]
    public class MaterialsStatisticaController : Controller
    {
        private readonly MaterialsStatisticaManager _materialsStatisticaManager;
        /// <summary>
        /// 物资报表
        /// </summary>
        /// <param name="materialsStatisticaManager"></param>
        public MaterialsStatisticaController(MaterialsStatisticaManager materialsStatisticaManager)
        {
            _materialsStatisticaManager = materialsStatisticaManager;
        }

        // public Task<MaterialsConfluenceOutPut[]> GetMaterialsConfluenceAsync(MaterialsConfluenceInPut inPut)

        #region 物资报表统计

        /// <summary>
        /// 入出汇总统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/Confluence")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MaterialsConfluenceOutPut[]>> GetMaterialsConfluenceAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                } 
                var result = await _materialsStatisticaManager.GetMaterialsConfluenceAsync(inPut);
                return new ServiceMessage<MaterialsConfluenceOutPut[]>(result);
            });
        }

        /// <summary>
        /// 物品进销存查询  
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/EntersSellsSaves")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ItemsTalesAndInventorySummaryModel[]>> GetMaterialsEntersSellsSavesAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetMaterialsEntersSellsSavesAsync(inPut);
                return new ServiceMessage<ItemsTalesAndInventorySummaryModel[]>(result);
            });
        }
        /// <summary>
        /// 物品进销存查询 （简）
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/EntersSellsSavesSimplify")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ItemsTalesAndInventorySummaryModel[]>> GetMaterialsEntersSellsSavesSimplifyAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetMaterialsEntersSellsSavesSimplifyAsync(inPut);
                return new ServiceMessage<ItemsTalesAndInventorySummaryModel[]>(result);
            });
        }

        //  public Task<ItemFortheModel[]> GetItemFortheModelAsync(MaterialsConfluenceInPuts model)
        /// <summary>
        /// 物品实际出入库
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ItemForthe")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ItemFortheModel[]>> GetItemFortheModelAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetItemFortheModelAsync(inPut);
                return new ServiceMessage<ItemFortheModel[]>(result);
            });
        }

        /// <summary>
        /// 根据供应商查询进销存汇总表
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/EntersSellsSavesBySupplier")]
       // [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ItemsTalesAndInventorySummaryModel[]>> GetMaterialsEntersSellsBySupplierIdSavesAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetMaterialsEntersSellsBySupplierIdSavesAsync(inPut);
                return new ServiceMessage<ItemsTalesAndInventorySummaryModel[]>(result);
            });
        }
        /// <summary>
        /// 物品入库开单信息查询
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/PutInStorage")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PutInStorageBillTjPageModel[]>> GetPutInStorageAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetPutInStorageAsync(inPut);
                //return new ServiceMessage<PutInStorageBillTjPageModel[]>(result.Result, result.Count);
                return new ServiceMessage<PutInStorageBillTjPageModel[]>(result);
            });
        }

        /// <summary>
        /// 物品入库开单明细信息查询
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/PutInStorageDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<PutInStorageBillDetailsTjTmpModel[]>> GetPutInStorageDetailAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetPutInStorageDetailAsync(inPut);
                //return new ServiceMessage<PutInStorageBillDetailsTjTmpModel[]>(result.Result, result.Count);
                return new ServiceMessage<PutInStorageBillDetailsTjTmpModel[]>(result);
            });
        }

        /// <summary>
        /// 物品入库开单结算
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/PutInStorageBillSettlement")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdatePutInStorageBillSettlementAsync([FromBody]SettlemenInPuts inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.UpdatePutInStorageBillSettlementAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 结算前核对勾稽
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/PutInStorageBillChecked")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdatePutInStorageBillCheckedAsync([FromBody]SettlemenInPuts inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.UpdatePutInStorageBillCheckedAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }



        /// <summary>
        /// 其它出库统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/OtherOutbound")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundModel[]>> GetOtherOutboundByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetOtherOutboundByDateAsync(inPut);
                //return new ServiceMessage<OtherOutboundModel[]>(result.Result, result.Count);
                return new ServiceMessage<OtherOutboundModel[]>(result);
            });
        }

        /// <summary>
        /// 其它出库明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/OtherOutboundDtail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundDetailModel[]>> GetOtherOutboundDetailByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetOtherOutboundDetailByDateAsync(inPut);
                // return new ServiceMessage<OtherOutboundDetailModel[]>(result.Result, result.Count);
                return new ServiceMessage<OtherOutboundDetailModel[]>(result);
            });
        }


        /// <summary>
        /// 领用出库统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/Recipients")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AccuratelyRecipientsModel[]>> GetAccuratelyRecipientsByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetAccuratelyRecipientsByDateAsync(inPut);
                return new ServiceMessage<AccuratelyRecipientsModel[]>(result);
            });
        }

        /// <summary>
        /// 领用出库明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/RecipientsDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AccuratelyRecipientsDetailModel[]>> GetAccuratelyRecipientsDetailByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetAccuratelyRecipientsDetailByDateAsync(inPut);
                return new ServiceMessage<AccuratelyRecipientsDetailModel[]>(result);
            });
        }
        /// <summary>
        /// 划价出库统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/AccuratelyOutbound")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AccuratelyOutboundModel[]>> GetAccuratelyOutboundByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetAccuratelyOutboundByDateAsync(inPut);
                //return new ServiceMessage<AccuratelyOutboundModel[]>(result.Result, result.Count);
                return new ServiceMessage<AccuratelyOutboundModel[]>(result);
            });
        }
        /// <summary>
        /// 划价出库明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/AccuratelyOutboundDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AccuratelyOutboundDetailModel[]>> GetAccuratelyOutboundDetailByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetAccuratelyOutboundDetailByDateAsync(inPut);
                return new ServiceMessage<AccuratelyOutboundDetailModel[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        /// 根据id查询划价出库明细
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/AccuratelyOutboundDetailById")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<AccuratelyOutboundDetailModel[]>> GetAccuratelyOutboundDetailByDateAndIdAsync([FromBody]MaterialsConfluenceInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetAccuratelyOutboundDetailByDateAndIdAsync(inPut);
                return new ServiceMessage<AccuratelyOutboundDetailModel[]>(result.Result, result.Count);
            });
        }
        /// <summary>
        /// 退货出库统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ReturnOutbound")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundModel[]>> GetReturnOutboundByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {

                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetReturnOutboundByDateAsync(inPut);
                return new ServiceMessage<OtherOutboundModel[]>(result);
            });
        }
        /// <summary>
        /// 退货结算前核对勾稽
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ReturnSettlementChecked")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> ReturnSettlementCheckedAsync([FromBody]SettlemenInPuts inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.ReturnSettlementCheckedAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 退货单结算  
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ReturnSettlement")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<bool>> UpdateReturnSettlementAsync([FromBody]SettlemenInPuts inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.UpdateReturnSettlementAsync(inPut);
                return new ServiceMessage<bool>(result);
            });
        }

        /// <summary>
        /// 退货出库明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ReturnOutboundDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundDetailModel[]>> GetReturnOutboundDetailByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetReturnOutboundDetailByDateAsync(inPut);
                return new ServiceMessage<OtherOutboundDetailModel[]>(result);
            });
        }

        /// <summary>
        /// 报废出库统计 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ScrapOutbound")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundModel[]>> GetScrapOutboundByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetScrapOutboundByDateAsync(inPut);
                return new ServiceMessage<OtherOutboundModel[]>(result);
            });
        }



        /// <summary>
        /// 报废出库明细统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ScrapOutboundDetail")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<OtherOutboundDetailModel[]>> GetScrapOutboundDetailByDateAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetScrapOutboundDetailByDateAsync(inPut);
                return new ServiceMessage<OtherOutboundDetailModel[]>(result);
            });
        }
        /// <summary>
        /// 近效期物品统计 
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/RecentValidityStatistics")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<RecentValiditymModel[]>> GetRecentValidityStatisticsAsync([FromBody]MaterialsRequst inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetRecentValidityStatisticsAsync(inPut);
                return new ServiceMessage<RecentValiditymModel[]>(result);
            });
        }

        /// <summary>
        /// 指定物品入出库台账查询
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ItemStandingBook")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<ItemStandingBookModel[]>> GetItemStandingBookAsync([FromBody]MaterialsConfluenceInPuts inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetItemStandingBookByIdAsync(inPut);
                return new ServiceMessage<ItemStandingBookModel[]>(result);
            });
        }
        #endregion
        #region 库存管理（物资统计）
        /// <summary>
        /// 低库存预
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/LowInventoryWarning")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<LowInventoryWarningModel[]>> GetLowInventoryWarningAsync([FromBody]MaterialsConfluenceInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetLowInventoryWarningAsync(inPut);
                return new ServiceMessage<LowInventoryWarningModel[]>(result);
            });
        }

        /// <summary>
        /// 供应商入库统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/SuppliePutInStorage")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<SupplierPutInStorageModel[]>> GetSupplierPutInStorageStatistics([FromBody]MaterialsConfluenceInPut inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetSupplierPutInStorageStatistics(inPut);
                return new ServiceMessage<SupplierPutInStorageModel[]>(result);
            });
        }
        /// <summary>
        /// 物品库存信息
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/GoodsInStockd")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<GoodsInStockdModel[]>> GetGoodsInStockdInfo([FromBody]MaterialsInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetGoodsInStockdInfo(inPut);
                return new ServiceMessage<GoodsInStockdModel[]>(result);
            });
        }
        /// <summary>
        /// 物品用量统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/UsageStatistics")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<GoodsInStockdModel[]>> GetUsageStatistics([FromBody]MaterialsConfluenceInPut inPut)
        {
            return Task.Run(async () =>
            {
                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2019-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }
                var result = await _materialsStatisticaManager.GetUsageStatistics(inPut);
                return new ServiceMessage<GoodsInStockdModel[]>(result);
            });
        }
        /// <summary>
        /// 高积压库存统计  
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/HighOverstockStatistics")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<GoodsInStockdModel[]>> GetHighOverstockStatistics([FromBody]MaterialsInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetHighOverstockStatistics(inPut);
                return new ServiceMessage<GoodsInStockdModel[]>(result);
            });
        }
        /// <summary>
        /// 近效期统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/ValidDateStatistics")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<GoodsInStockdModel[]>> GetValidDateStatistics([FromBody]MaterialsInPut inPut)
        {
            return Task.Run(async () =>
            {
                var result = await _materialsStatisticaManager.GetValidDateStatistics(inPut);
                return new ServiceMessage<GoodsInStockdModel[]>(result);
            });
        }
        #endregion
        //  public Task<MonthGoodsInStockdModel[]> GetMonthUsageStatistics(MaterialsConfluenceInPut model)


        /// <summary>
        /// 月均用量统计
        /// </summary>
        /// <param name="inPut"></param>
        /// <returns></returns>
        [HttpPost("MaterialsStatistica/MonthGoodsInStockdModel")]
        [CheckLogin]
        [ServiceMessageTryCatch]
        public virtual Task<ServiceMessage<MonthGoodsInStockdModel[]>> GetMonthGoodsInStockdModel([FromBody]MaterialsConfluenceInPut inPut)
        {
            return Task.Run(async () =>
            {

                if (!inPut.BeginTime.HasValue)
                {
                    inPut.BeginTime = Convert.ToDateTime("2022-1-1");
                }
                if (!inPut.EndTime.HasValue)
                {
                    inPut.EndTime = DateTime.Now.Date.AddDays(1);
                }
                else
                {
                    inPut.EndTime = inPut.EndTime.Value.Date.AddDays(1);
                }

                var result = await _materialsStatisticaManager.GetMonthUsageStatistics(inPut);
                return new ServiceMessage<MonthGoodsInStockdModel[]>(result);
            });
        }
    }
}
