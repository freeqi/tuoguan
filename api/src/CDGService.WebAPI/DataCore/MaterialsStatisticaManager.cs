using CDGService.Data;
using CDGService.Data.Datas;
using CDGService.Data.Store;
using CDGService.Store;
using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Dto;
using CDGService.Utils;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CDGService.Data.Helper;
namespace CDGService.WebAPI.DataCore
{

    /// <summary>
    /// 物资统计
    /// </summary>
    public class MaterialsStatisticaManager : XmlSql
    {

        #region 构造函数注入调用DAL层的方法
        private readonly string _ClassName;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetUserInfo _getUserInfo;
        /// <summary>
        /// 构造函数注入 
        /// </summary>
        public MaterialsStatisticaManager(IUnitOfWork unitOfWork, IGetUserInfo getUserInfo)
        {
            _ClassName = GetType().Name;//当前类名称
            _unitOfWork = unitOfWork;
            _getUserInfo = getUserInfo;
        }
        private IRepository<CenterDialysis> DialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<SI_CFMX> cfmxStore => _unitOfWork.GetStore<SI_CFMX>();
        private IRepository<CenterDialysis> CenterDialysisStore => _unitOfWork.GetStore<CenterDialysis>();
        private IRepository<PutInStorageBill> PutInStorageBillStore => _unitOfWork.GetStore<PutInStorageBill>();
        private IRepository<ReturnRequest> ReturnRequestStore => _unitOfWork.GetStore<ReturnRequest>();
        #endregion
        #region 物资报表
        /// <summary>
        /// 物品入出库汇总
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<MaterialsConfluenceOutPut[]> GetMaterialsConfluenceAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {

                List<MaterialsConfluenceOutPut> outPuts = new List<MaterialsConfluenceOutPut>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null).ToListAsync();
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                    }
                }
                if (!string.IsNullOrWhiteSpace(model.MedicalItemType) && model.MedicalItemType == "0")
                {
                    model.MedicalItemType = "1,2,3,4";
                }

                int index = 0;
                if (CenterDialysiss.Count > 0)
                {
                    foreach (var center in CenterDialysiss)
                    {
                        var medicalItemType = model.MedicalItemType.Split(',');
                        foreach (var type in medicalItemType)
                        {
                            string medicalItemTypeName = string.Empty;
                            switch (type)
                            {
                                case "1":
                                    medicalItemTypeName = "药品"; break;
                                case "2":
                                    medicalItemTypeName = "卫生耗材"; break;
                                case "3":
                                    medicalItemTypeName = "固定资产"; break;
                                case "4":
                                    medicalItemTypeName = "低值易耗"; break;
                            }
                            var sqlParameterList = new List<SqlParameter>();
                            sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));


                            //时间段查询物品进销存数据  
                            var list = new List<ItemsTalesAndInventorySummarySqlModel>();
                            var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                            string sql = GetQuerySql(xmlSqlParameter1);
                            var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + $" AND a.CenterId=@CenterId AND a.MedicalItemType={type}", sqlParameterList.ToArray());
                            var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);
                            if (putinTuple.Item1)
                            {
                                list.AddRange(putinTuple.Item2);
                            }

                            //获取截止当天期末数据
                            var inventory_list = new List<ItemsTalesAndInventorySummarySqlModel>();
                            var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                            string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                            var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + $" AND a.CenterId=@CenterId AND a.MedicalItemType={type}", sqlParameterList.ToArray()));
                            if (primaryLibraryTuple.Item1)
                            {
                                inventory_list.AddRange(primaryLibraryTuple.Item2);
                            }

                            //查询处方单明细 
                            var presdetailList = new List<PrescriptionDetailModel>();
                            var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                            string priceSql = GetQuerySql(xmlSqlParameter2);
                            var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + $" AND presDetail.CenterId=@CenterId AND mir.MedicalItemType={type}", sqlParameterList.ToArray());
                            var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                            if (presdetailTuple.Item1)
                            {
                                presdetailList.AddRange(presdetailTuple.Item2);
                            }

                            //查询退费明细信息 
                            var RefundList = new List<PrescriptionDetailModel>();
                            var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsRefundByDate", "查询退费明细信息");
                            string refundSql = GetQuerySql(xmlSqlParameter);
                            var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + $" AND presDetail.CenterId=@CenterId AND mir.MedicalItemType={type}", sqlParameterList.ToArray());
                            var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                            if (refundTuple.Item1)
                            {
                                RefundList.AddRange(refundTuple.Item2);
                            }
                            var result_list = new List<MaterialsConfluenceOutPut>();
                            //期初
                            decimal BeginCostPrice = 0;
                            decimal BeginSalePrice = 0;
                            //入库
                            decimal PutInStorageCostPrice = 0;
                            decimal PutInStorageSalePrice = 0;
                            //冲销出库
                            decimal CXPutInStorageCostPrice = 0;
                            decimal CXPutInStorageSalePrice = 0;

                            //出库
                            decimal OutboundCostPrice = 0;
                            decimal OutboundSalePrice = 0;

                            decimal ScrapTotalCostPrice = 0;//报废出库总额
                            decimal ScrapTotalSalePrice = 0;

                            decimal ReturnTotalCostPrice = 0;//退货出库总额
                            decimal ReturnTotalSalePrice = 0;
                            //销售
                            decimal SalesCostPrice = 0;
                            decimal SalesSalePrice = 0;
                            //退费
                            decimal RefundCostPrice = 0;
                            decimal RefundSalePrice = 0;
                            //领用
                            decimal RecipientsCostPrice = 0;
                            decimal RecipientsSalePrice = 0;
                            //冲销领用
                            decimal CXRecipientsCostPrice = 0;
                            decimal CXRecipientsSalePrice = 0;


                            //普通卫材销售
                            decimal NormalSalesCostPrice = 0;
                            decimal NormalSalesSalePrice = 0;

                            //普通卫材退费
                            decimal NormalRefundCostPrice = 0;
                            decimal NormalRefundSalePrice = 0;


                            var now = DateTime.Now.Date.AddDays(1);
                            //出入数据
                            var resultModel = new MaterialsConfluenceOutPut();
                            //获取期末数据
                            if (inventory_list.Count > 0)
                            {
                                var typeData = inventory_list;
                                decimal InPriceSum = typeData.Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                                decimal SalePriceSum = typeData.Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);
                                //期末数据
                                resultModel.ParentBusinessType = "期末";
                                resultModel.ChildBusinessType = "——";
                                resultModel.InPriceSum = InPriceSum;
                                resultModel.SalePriceSum = SalePriceSum;
                                resultModel.DifferencePrice = SalePriceSum - InPriceSum;
                            }
                            if (list.Count > 0)
                            {
                                //如果选择的时间不是当天  则需用当天的期末数据-出入库数据
                                if (model.EndTime < now)
                                {
                                    var dataInventory = list.ToList().FindAll(a => a.putInstorageDate.Value >= model.EndTime && a.putInstorageDate.Value <= now);
                                    //定义
                                    decimal PutInStorageCostPriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageSalePriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);

                                    decimal OutboundCostPriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));

                                    decimal OutboundSalePriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));


                                    //期末数据
                                    resultModel.InPriceSum = resultModel.InPriceSum + OutboundCostPriceInventory - PutInStorageCostPriceInventory;
                                    resultModel.SalePriceSum = resultModel.SalePriceSum + OutboundSalePriceInventory - PutInStorageSalePriceInventory;
                                    resultModel.DifferencePrice = resultModel.SalePriceSum - resultModel.InPriceSum;
                                }
                                var keyDic = new Dictionary<string, List<string>>();
                                decimal ReceivablePrice = 0;
                                decimal RefundPrice = 0;
                                decimal NormalReceivablePrice = 0;
                                decimal NormalRefundPrice = 0;
                                var data = list.ToList().FindAll(a => a.putInstorageDate.Value >= model.BeginTime.Value.Date && a.putInstorageDate.Value <= model.EndTime);
                                var key = "a7d83b6d81104475b7ede8ea467bab1f";//销售
                                foreach (var dicItem in data.FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f"))
                                {
                                    if (keyDic.ContainsKey(key)) { keyDic[key].Add(dicItem.OutInBoundDetailId); }
                                    else { keyDic.Add(key, new List<string>() { dicItem.OutInBoundDetailId }); }

                                }
                                var refundDic = new Dictionary<string, List<string>>();
                                var refundKey = "608399fe9db64480828b3f20c6815fd1";//退费退库
                                foreach (var dicItem in data.FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1"))
                                {
                                    if (refundDic.ContainsKey(refundKey)) { refundDic[refundKey].Add(dicItem.OutInBoundDetailId); }
                                    else { refundDic.Add(refundKey, new List<string>() { dicItem.OutInBoundDetailId }); }

                                }
                                if (keyDic.Count > 0)
                                {
                                    foreach (var item in keyDic.Keys)
                                    {
                                        if (keyDic[item].Count > 0)
                                        {
                                            if (presdetailList.Count > 0)
                                            {
                                                ReceivablePrice = presdetailList.FindAll(a => keyDic[item].Contains(a.OutboundId)).Sum(a => a.TotalQty * a.SalePrice).Value;
                                                NormalReceivablePrice = presdetailList.FindAll(a => keyDic[item].Contains(a.OutboundId) && a.MayCharges == 1 && a.WareHouseParentIds.Contains("c1fe3b7a2e3a495d958062fa81c9aaf6")).Sum(a => a.TotalQty * a.SalePrice).Value;

                                            }
                                        }
                                    }
                                }
                                if (refundDic.Count > 0)
                                {
                                    foreach (var item in refundDic.Keys)
                                    {
                                        if (refundDic[item].Count > 0)
                                        {
                                            if (refundDic.Keys.Contains(item))
                                            {
                                                RefundPrice = RefundList.FindAll(a => refundDic[item].Contains(a.ReturnWarehouseId)).Sum(a => a.TotalQty * a.SalePrice).Value;
                                                NormalRefundPrice = RefundList.FindAll(a => refundDic[item].Contains(a.ReturnWarehouseId) && a.WareHouseParentIds.Contains("c1fe3b7a2e3a495d958062fa81c9aaf6") && a.MayCharges == 1).Sum(a => a.TotalQty * a.SalePrice).Value;
                                            }
                                        }
                                    }
                                }
                                //计算出入库销售数量  
                                //入库
                                PutInStorageCostPrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageSalePrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);

                                //冲销入库
                                CXPutInStorageCostPrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageSalePrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);

                                //其它出库
                                OutboundCostPrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                OutboundSalePrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));


                                //报废出库
                                ScrapTotalCostPrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ScrapTotalSalePrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //退货出库
                                ReturnTotalCostPrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReturnTotalSalePrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //销售
                                SalesCostPrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                SalesSalePrice = ReceivablePrice;
                                NormalSalesCostPrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" && t.MayCharges == 1 && t.WareHouseParentIds.Contains("c1fe3b7a2e3a495d958062fa81c9aaf6")).Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                NormalSalesSalePrice = NormalReceivablePrice;
                                //退费
                                RefundCostPrice = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                RefundSalePrice = RefundPrice;


                                //领用
                                RecipientsCostPrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                RecipientsSalePrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                //冲销领用
                                CXRecipientsCostPrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                CXRecipientsSalePrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //期初数据
                                var saleprice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                BeginCostPrice = resultModel.InPriceSum.Value + OutboundCostPrice + SalesCostPrice + ScrapTotalCostPrice + ReturnTotalCostPrice + RecipientsCostPrice - PutInStorageCostPrice + CXPutInStorageCostPrice - CXRecipientsCostPrice - RefundCostPrice;
                                BeginSalePrice = resultModel.SalePriceSum.Value + OutboundSalePrice + saleprice + RecipientsSalePrice + ScrapTotalSalePrice + ReturnTotalSalePrice - PutInStorageSalePrice + CXPutInStorageSalePrice - CXRecipientsSalePrice;
                            }
                            else
                            {
                                BeginSalePrice = resultModel.SalePriceSum ?? 0.00m;
                                BeginCostPrice = resultModel.InPriceSum ?? 0.00m;

                                if (BeginSalePrice > 0)
                                {
                                    resultModel.SalePriceSum = BeginSalePrice;
                                }
                                else
                                {
                                    resultModel.SalePriceSum = 0;
                                }
                                if (BeginCostPrice > 0)
                                {
                                    resultModel.InPriceSum = BeginCostPrice;
                                }
                                else
                                {
                                    resultModel.InPriceSum = 0;
                                }
                                //期末数据
                                resultModel.DifferencePrice = resultModel.SalePriceSum - resultModel.InPriceSum;
                            }
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "期初",
                                ChildBusinessType = "——",
                                SalePriceSum = BeginSalePrice,
                                InPriceSum = BeginCostPrice,
                                DifferencePrice = BeginSalePrice - BeginCostPrice //差价
                            }); ;
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "入库",
                                ChildBusinessType = "外购入库",
                                SalePriceSum = PutInStorageSalePrice,
                                InPriceSum = PutInStorageCostPrice,
                                DifferencePrice = PutInStorageSalePrice - PutInStorageCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "冲销入库",
                                SalePriceSum = CXPutInStorageSalePrice,
                                InPriceSum = CXPutInStorageCostPrice,
                                DifferencePrice = CXPutInStorageSalePrice - CXPutInStorageCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "入库",
                                ChildBusinessType = "退货",
                                SalePriceSum = -ReturnTotalSalePrice,
                                InPriceSum = -ReturnTotalCostPrice,
                                DifferencePrice = ReturnTotalSalePrice - ReturnTotalCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "领用出库",
                                SalePriceSum = RecipientsSalePrice,
                                InPriceSum = RecipientsCostPrice,
                                DifferencePrice = RecipientsSalePrice - RecipientsCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "入库",
                                ChildBusinessType = "冲销领用出库",
                                SalePriceSum = CXRecipientsSalePrice,
                                InPriceSum = CXRecipientsCostPrice,
                                DifferencePrice = CXRecipientsSalePrice - CXRecipientsCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "收费处方发料",
                                SalePriceSum = SalesSalePrice - NormalSalesSalePrice,
                                InPriceSum = SalesCostPrice - NormalSalesCostPrice,
                                DifferencePrice = (SalesSalePrice - NormalSalesSalePrice) - (SalesCostPrice - NormalSalesCostPrice) //差价
                            });

                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "普通卫材收费处方发料",
                                SalePriceSum = NormalSalesSalePrice,
                                InPriceSum = NormalSalesCostPrice,
                                DifferencePrice = NormalSalesSalePrice - NormalSalesCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "入库",
                                ChildBusinessType = "退费退库",
                                SalePriceSum = RefundSalePrice,
                                InPriceSum = RefundCostPrice,
                                DifferencePrice = RefundSalePrice - RefundCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "其它出库",
                                SalePriceSum = OutboundSalePrice,
                                InPriceSum = OutboundCostPrice,
                                DifferencePrice = OutboundSalePrice - OutboundCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = "出库",
                                ChildBusinessType = "报废出库",
                                SalePriceSum = ScrapTotalSalePrice,
                                InPriceSum = ScrapTotalCostPrice,
                                DifferencePrice = ScrapTotalSalePrice - ScrapTotalCostPrice //差价
                            });
                            outPuts.Add(new MaterialsConfluenceOutPut()
                            {
                                SerialNumber = ++index,
                                DialysisName = center.DialysisName,
                                MedicalItemTypeName = medicalItemTypeName,
                                ParentBusinessType = resultModel.ParentBusinessType,
                                ChildBusinessType = resultModel.ChildBusinessType,
                                SalePriceSum = resultModel.SalePriceSum,
                                InPriceSum = resultModel.InPriceSum,
                                DifferencePrice = resultModel.DifferencePrice//差价
                            });
                        }
                    }
                }

                return outPuts.ToArray();
            });
        }

        /// <summary>
        /// 进销存汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ItemsTalesAndInventorySummaryModel[]> GetMaterialsEntersSellsSavesAsync(MaterialsConfluenceInPuts model)
        {


            return Task.Run(async () =>
            {
                var isAll = false;
                List<MaterialsConfluenceOutPut> outPuts = new List<MaterialsConfluenceOutPut>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                var list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var inventory_list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var presdetail_list = new List<PrescriptionDetailModel>();
                var refund_list = new List<PrescriptionDetailModel>();
                string sqlwhere = string.Empty;
                string sqlStr = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  MedicalItemType in ({model.MedicalItemType})";
                    sqlStr = sqlwhere;
                }
                var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                string sql = GetQuerySql(xmlSqlParameter1);

                //查询处方明细信息
                var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                string priceSql = GetQuerySql(xmlSqlParameter2);

                //查询退费明细信息 
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsRefundByDate", "查询退费明细信息");
                string refundSql = GetQuerySql(xmlSqlParameter);

                var alist = new List<PrescriptionDetailModel>();

                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND a.CenterId=@CenterId";
                    sql = sql + sqlwhere;
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);
                        if (putinTuple.Item1)
                        {
                            list.AddRange(putinTuple.Item2);
                        }
                        //获取截止当天期末数据 
                        var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                        string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                        var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere, sqlParameterList.ToArray()));
                        if (primaryLibraryTuple.Item1)
                        {
                            inventory_list.AddRange(primaryLibraryTuple.Item2);
                        }
                        //查询处方单明细 
                        var sqlParameterList1 = new List<SqlParameter>();
                        sqlParameterList1.Add(new SqlParameter("@CenterId", center.Id));
                        var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + $" AND presDetail.CenterId=@CenterId AND presDetail.FounderDate>='{model.BeginTime.Value.AddMonths(-2).Date}' AND presDetail.FounderDate<='{model.EndTime.Value.AddMonths(2).Date}'" + sqlStr, sqlParameterList1.ToArray());
                        var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                        if (presdetailTuple.Item1)
                        {
                            presdetail_list.AddRange(presdetailTuple.Item2);
                            alist.AddRange(presdetailTuple.Item2);
                        }
                        //查询退费明细
                        var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + $" AND presDetail.CenterId=@CenterId " + sqlStr, sqlParameterList.ToArray());
                        var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                        if (refundTuple.Item1)
                        {
                            refund_list.AddRange(refundTuple.Item2);
                        }
                    }
                }
                else
                {
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere);
                    var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere));
                    if (putinTuple.Item1)
                    {
                        list = putinTuple.Item2;
                    }
                    //获取截止当天期末数据                

                    var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                    string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                    var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere));
                    if (primaryLibraryTuple.Item1)
                    {
                        inventory_list = primaryLibraryTuple.Item2;
                    }
                    //查询处方单明细 
                    var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + $"  AND presDetail.FounderDate>='{model.BeginTime.Value.AddMonths(-2).Date}' AND presDetail.FounderDate<='{model.EndTime.Value.AddMonths(2).Date}'" + sqlStr);
                    var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                    if (presdetailTuple.Item1)
                    {
                        presdetail_list = presdetailTuple.Item2;
                        alist.AddRange(presdetailTuple.Item2);
                    }
                    // //查询退费明细
                    var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + sqlStr);
                    var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                    if (refundTuple.Item1)
                    {
                        refund_list.AddRange(refundTuple.Item2);
                    }
                }

                var result_list = new List<ItemsTalesAndInventorySummaryModel>();
                decimal BeginTotalCostPrice = 0;//期初总额
                decimal BeginTotalSalePrice = 0;
                decimal BeginTotalCount = 0;
                decimal BeginTotalDifference = 0;

                decimal PutInStorageTotalCostPrice = 0;//入库总额
                decimal PutInStorageTotalSalePrice = 0;
                decimal PutInStorageTotalCount = 0;
                decimal PutInStorageTotalDifference = 0;

                decimal CXPutInStorageTotalCostPrice = 0;//入库总额
                decimal CXPutInStorageTotalSalePrice = 0;
                decimal CXPutInStorageTotalCount = 0;
                decimal CXPutInStorageTotalDifference = 0;


                decimal OutboundTotalCostPrice = 0;//其它出库总额
                decimal OutboundTotalSalePrice = 0;
                decimal OutboundTotalCount = 0;
                decimal OutboundTotalDifference = 0;

                decimal ScrapTotalCostPrice = 0;//报废出库总额
                decimal ScrapTotalSalePrice = 0;
                decimal ScrapTotalCount = 0;
                decimal ScrapTotalDifference = 0;


                decimal ReceiveTotalCostPrice = 0;//领用出库总额
                decimal ReceiveTotalSalePrice = 0;
                decimal ReceiveTotalCount = 0;
                decimal ReceiveTotalDifference = 0;

                decimal CXReceiveTotalCostPrice = 0;//冲销领用出库总额
                decimal CXReceiveTotalSalePrice = 0;
                decimal CXReceiveTotalCount = 0;
                decimal CXReceiveTotalDifference = 0;

                decimal ReturnTotalCostPrice = 0;//退货出库总额
                decimal ReturnTotalSalePrice = 0;
                decimal ReturnTotalCount = 0;
                decimal ReturnTotalDifference = 0;

                decimal SalesTotalCostPrice = 0;//销售总额
                decimal SalesTotalSalePrice = 0;
                decimal SalesTotalCount = 0;
                decimal SalesTotalDifference = 0;

                decimal RefundSalesTotalCostPrice = 0;//退费退库
                decimal RefundSalesTotalSalePrice = 0;
                decimal RefundSalesTotalCount = 0;
                decimal RefundSalesTotalDifference = 0;

                decimal EndTotalCostPrice = 0;//期末总额
                decimal EndTotalSalePrice = 0;
                decimal EndTotalCount = 0;
                decimal EndTotalDifference = 0;


                var now = DateTime.Now.Date.AddDays(1);
                if (inventory_list.Count > 0)
                {
                    int i = 0;
                    var allData = inventory_list.OrderBy(a => a.MedicalItemId).GroupBy(a => a.CenterId);
                    foreach (var items in allData)
                    {
                        var itemData = items.OrderBy(a => a.MedicalItemId).GroupBy(a => new { a.MedicalItemId, a.InPrice });
                        foreach (var item in itemData)
                        {
                            var resultModel = new ItemsTalesAndInventorySummaryModel();
                            //获取期末数据
                            var inventoryData = inventory_list.FindAll(a => a.CenterId == items.Key && a.MedicalItemId == item.Key.MedicalItemId && a.InPrice == item.Key.InPrice);
                            decimal totalCount = inventoryData.Sum(t => t.InQty.Value);
                            decimal totalCost = inventoryData.Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                            decimal totalSale = inventoryData.Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);
                            //期末数据
                            resultModel.EndCount = totalCount;
                            resultModel.EndCostPrice = totalCost;
                            resultModel.EndSalePrice = totalSale;
                            resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;

                            decimal BeginCostPrice = 0;
                            decimal BeginSalePrice = 0;
                            decimal BeginCount = 0;

                            decimal PutInStorageCostPrice = 0;
                            decimal PutInStorageSalePrice = 0;
                            decimal PutInStorageCount = 0;

                            decimal CXPutInStorageCostPrice = 0;
                            decimal CXPutInStorageSalePrice = 0;
                            decimal CXPutInStorageCount = 0;

                            decimal OutboundCostPrice = 0;
                            decimal OutboundSalePrice = 0;
                            decimal OutboundCount = 0;

                            decimal ScrapCostPrice = 0;
                            decimal ScrapSalePrice = 0;
                            decimal ScrapCount = 0;

                            decimal ReceiveCostPrice = 0;
                            decimal ReceiveSalePrice = 0;
                            decimal ReceiveCount = 0;

                            decimal CXReceiveCostPrice = 0;
                            decimal CXReceiveSalePrice = 0;
                            decimal CXReceiveCount = 0;

                            decimal ReturnCostPrice = 0;
                            decimal ReturnSalePrice = 0;
                            decimal ReturnCount = 0;

                            decimal SalesCostPrice = 0;
                            decimal SalesSalePrice = 0;
                            decimal SalesCount = 0;

                            decimal RefundSalesCostPrice = 0;
                            decimal RefundSalesSalePrice = 0;
                            decimal RefundSalesCount = 0;
                            var dataList = list.FindAll(a => a.CenterId == items.Key);
                            if (dataList.Count > 0)
                            {
                                //如果选择的时间不是当天  则需用当天的期末数据-出入库数据
                                if (model.EndTime < now)
                                {

                                    var dataInventory = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.EndTime && t.putInstorageDate.Value <= now);

                                    //定义
                                    decimal PutInStorageCostPriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageSalePriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageCountInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);

                                    //出库
                                    decimal OutboundCostPriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                    decimal OutboundSalePriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                    decimal OutboundCountInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);

                                    //期末数据
                                    resultModel.EndCount = resultModel.EndCount + OutboundCountInventory - PutInStorageCountInventory;
                                    resultModel.EndCostPrice = resultModel.EndCostPrice + OutboundCostPriceInventory - PutInStorageCostPriceInventory;
                                    resultModel.EndSalePrice = resultModel.EndSalePrice + OutboundSalePriceInventory - PutInStorageSalePriceInventory;
                                    resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                                }
                                //var keyEndDic = new Dictionary<string, List<string>>();
                                //var priceEndDic = new Dictionary<string, decimal>();
                                //var RefundEndDic = new Dictionary<string, decimal>();
                                //获取流水
                                var data = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.BeginTime.Value.Date && t.putInstorageDate.Value <= model.EndTime).ToList();
                                //GetSalePrice(presdetail_list, refund_list, keyEndDic, priceEndDic, RefundEndDic, data, item.Key.MedicalItemId, items.Key);

                                //计算出入库销售数量
                                PutInStorageCostPrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageSalePrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageCount = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value);

                                //冲销入库
                                CXPutInStorageCostPrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageSalePrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageCount = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);


                                //其它出库
                                OutboundCostPrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                OutboundSalePrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                OutboundCount = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value);
                                //报废出库
                                ScrapCostPrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ScrapSalePrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ScrapCount = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value);
                                //领用出库
                                ReceiveCostPrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReceiveSalePrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ReceiveCount = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value);
                                //冲销领用出库
                                CXReceiveCostPrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                CXReceiveSalePrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                CXReceiveCount = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value);

                                //退货出库
                                ReturnCostPrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReturnSalePrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ReturnCount = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value);

                                var detail = data.Where(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").ToList();
                                //销售
                                SalesCostPrice = detail.Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                SalesSalePrice = 0;
                                //循环出库明细
                                foreach (var result in detail)
                                {
                                    //var salesDetail = alist.ToList().Where(a => a.OutboundId == result.OutInBoundDetailId && a.MaterialId == item.Key.MedicalItemId).ToList();
                                    //if (result.ItemQty >= salesDetail[0].TotalQty)
                                    //{
                                    //    decimal? qty = 0;
                                    //    //循环处方明细
                                    //    foreach (var itemQty in salesDetail)
                                    //    {
                                    //        qty += itemQty.TotalQty;
                                    //        if (qty > result.ItemQty)
                                    //        {
                                    //            continue;
                                    //        }
                                    //        else
                                    //        {
                                    //            SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                    //            alist.ToList().Remove(itemQty);
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    SalesSalePrice += result.ItemQty.Value * salesDetail[0].SalePrice.Value;
                                    //}
                                    var salesDetail = alist.Where(a => a.OutboundId == result.OutInBoundDetailId && a.MaterialId == item.Key.MedicalItemId).OrderBy(a => a.TotalQty).ToList();
                                    //存在处方明细/出库明细  多对一  一对多的情况  就算售价需根据实际情况来 
                                    if (result.ItemQty == salesDetail.Sum(a => a.TotalQty))//多对一  一对一
                                    {
                                        foreach (var itemQty in salesDetail)
                                        {
                                            SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                        }
                                    }
                                    else
                                    {
                                        decimal? qty = 0;
                                        //循环处方明细
                                        foreach (var itemQty in salesDetail)
                                        {
                                            qty += itemQty.TotalQty;
                                            if (qty > result.ItemQty)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                                alist.Remove(itemQty);
                                            }

                                        }
                                    }
                                }
                                SalesCount = detail.Sum(t => t.ItemQty.Value);

                                //退费
                                RefundSalesCostPrice = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                var refundListStr = string.Join(",", data.Where(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Select(a => a.OutInBoundDetailId).ToArray());
                                var refundDetail = refund_list.Where(a => refundListStr.Contains(a.ReturnWarehouseId));
                                RefundSalesSalePrice = refundDetail.Sum(a => a.SalePrice.Value * a.TotalQty.Value);

                                RefundSalesCount = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);


                                var SalePrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //期初数据
                                BeginCount = resultModel.EndCount.Value + OutboundCount + ScrapCount + ReceiveCount + ReturnCount + SalesCount - PutInStorageCount - CXReceiveCount - RefundSalesCount + CXPutInStorageCount;
                                BeginCostPrice = resultModel.EndCostPrice.Value + OutboundCostPrice + ScrapCostPrice + ReceiveCostPrice + ReturnCostPrice + SalesCostPrice - PutInStorageCostPrice - CXReceiveCostPrice - RefundSalesCostPrice + CXPutInStorageCostPrice;
                                BeginSalePrice = resultModel.EndSalePrice.Value + OutboundSalePrice + ScrapSalePrice + ReceiveSalePrice + ReturnSalePrice + SalePrice - PutInStorageSalePrice - CXReceiveSalePrice + CXPutInStorageSalePrice;
                            }
                            i++;
                            var _data = item.First();
                            resultModel.SerialNumber = i.ToString();
                            resultModel.MedicalItemCode = _data.MedicalItemCode;
                            resultModel.NationItemCode = _data.NationItemCode;
                            if (_data.MedicalItemType == 1)
                                resultModel.ApprovalNum = _data.ApprovalNum + "" == "" ? "" : _data.ApprovalNum.Contains("H") ? "西药" : _data.ApprovalNum.Contains("Z") ? "中成药" : _data.ApprovalNum.Contains("J") ? "西药" : _data.ApprovalNum.Contains("S") ? "西药" : "";
                            else
                                resultModel.ApprovalNum = _data.CatalogueName + "" == "" ? "" : _data.CatalogueName;
                            resultModel.MedicalItemName = _data.MedicalItemName;
                            resultModel.DialysisName = _data.DialysisName;
                            resultModel.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                            resultModel.Manufacturer = _data.Manufacturer;
                            resultModel.UnitName = _data.IsSplited.Value == true ? _data.SpecificationsUnitName : _data.PackageUnitName;
                            resultModel.InPrice = _data.InPrice.HasValue ? _data.InPrice : 0;
                            resultModel.ItemTypeName = _data.ItemTypeName;
                            resultModel.BeginCount = BeginCount;
                            resultModel.BeginCostPrice = BeginCostPrice;
                            resultModel.BeginSalePrice = BeginSalePrice;
                            resultModel.BeginDifference = BeginSalePrice - BeginCostPrice;
                            resultModel.PutInStorageCount = PutInStorageCount;
                            resultModel.PutInStorageCostPrice = PutInStorageCostPrice;
                            resultModel.PutInStorageSalePrice = PutInStorageSalePrice;
                            resultModel.PutInStorageDifference = PutInStorageSalePrice - PutInStorageCostPrice;

                            resultModel.CXPutInStorageCount = CXPutInStorageCount;//冲销入库
                            resultModel.CXPutInStorageCostPrice = CXPutInStorageCostPrice;
                            resultModel.CXPutInStorageSalePrice = CXPutInStorageSalePrice;
                            resultModel.CXPutInStorageDifference = CXPutInStorageSalePrice - CXPutInStorageCostPrice;

                            resultModel.OutboundCount = OutboundCount;
                            resultModel.OutboundCostPrice = OutboundCostPrice;
                            resultModel.OutboundSalePrice = OutboundSalePrice;
                            resultModel.OutboundDifference = OutboundSalePrice - OutboundCostPrice;

                            resultModel.ScrapCount = ScrapCount;
                            resultModel.ScrapCostPrice = ScrapCostPrice;
                            resultModel.ScrapSalePrice = ScrapSalePrice;
                            resultModel.ScrapDifference = ScrapSalePrice - ScrapCostPrice;

                            resultModel.ReceiveCount = ReceiveCount;
                            resultModel.ReceiveCostPrice = ReceiveCostPrice;
                            resultModel.ReceiveSalePrice = ReceiveSalePrice;
                            resultModel.ReceiveDifference = ReceiveSalePrice - ReceiveCostPrice;

                            resultModel.CXReceiveCount = CXReceiveCount;//冲销领用出库
                            resultModel.CXReceiveCostPrice = CXReceiveCostPrice;
                            resultModel.CXReceiveSalePrice = CXReceiveSalePrice;
                            resultModel.CXReceiveDifference = CXReceiveSalePrice - CXReceiveCostPrice;

                            resultModel.ReturnCount = ReturnCount;
                            resultModel.ReturnCostPrice = ReturnCostPrice;
                            resultModel.ReturnSalePrice = ReturnSalePrice;
                            resultModel.ReturnDifference = ReturnSalePrice - ReturnCostPrice;

                            resultModel.SalesCount = SalesCount;
                            resultModel.SalesCostPrice = SalesCostPrice;
                            resultModel.SalesSalePrice = SalesSalePrice;
                            resultModel.SalesDifference = SalesSalePrice - SalesCostPrice;


                            resultModel.RefundSalesCount = RefundSalesCount;
                            resultModel.RefundSalesCostPrice = RefundSalesCostPrice;
                            resultModel.RefundSalesSalePrice = RefundSalesSalePrice;
                            resultModel.RefundSalesDifference = RefundSalesSalePrice - RefundSalesCostPrice;
                            //合计
                            BeginTotalCostPrice += BeginCostPrice; //期初成本单价
                            BeginTotalSalePrice += BeginSalePrice; //期初销售单价
                            BeginTotalCount += BeginCount; //期初数量
                            BeginTotalDifference += resultModel.BeginDifference.Value; //期初差价
                            PutInStorageTotalCostPrice += PutInStorageCostPrice; //入库成本价
                            PutInStorageTotalSalePrice += PutInStorageSalePrice; //入库售价
                            PutInStorageTotalCount += PutInStorageCount; //入库数量
                            PutInStorageTotalDifference += resultModel.PutInStorageDifference.Value;//入库差价

                            CXPutInStorageTotalCostPrice += CXPutInStorageCostPrice; //冲销入库成本价
                            CXPutInStorageTotalSalePrice += CXPutInStorageSalePrice; //冲销入库售价
                            CXPutInStorageTotalCount += CXPutInStorageCount; //冲销入库数量
                            CXPutInStorageTotalDifference += resultModel.CXPutInStorageDifference.Value;//冲销入库差价 
                            OutboundTotalCostPrice += OutboundCostPrice; //出库成本价
                            OutboundTotalSalePrice += OutboundSalePrice; //出库售价
                            OutboundTotalCount += OutboundCount; //出库数量
                            OutboundTotalDifference += resultModel.OutboundDifference.Value; //出库差价
                                                                                             //报废
                            ScrapTotalCostPrice += ScrapCostPrice;//报废出库成本价
                            ScrapTotalSalePrice += ScrapSalePrice; //出库售价
                            ScrapTotalCount += ScrapCount; //出库数量
                            ScrapTotalDifference += resultModel.ScrapDifference.Value; //出库差价

                            ReceiveTotalCostPrice += ReceiveCostPrice; ////领用出库成本价
                            ReceiveTotalSalePrice += ReceiveSalePrice; //出库售价
                            ReceiveTotalCount += ReceiveCount; //出库数量
                            ReceiveTotalDifference += resultModel.ReceiveDifference.Value; //出库差价

                            CXReceiveTotalCostPrice += CXReceiveCostPrice; ////冲销领用出库成本价
                            CXReceiveTotalSalePrice += CXReceiveSalePrice; //出库售价
                            CXReceiveTotalCount += CXReceiveCount; //出库数量
                            CXReceiveTotalDifference += resultModel.CXReceiveDifference.Value; //出库差价

                            ReturnTotalCostPrice += ReturnCostPrice; ////退货出库成本价
                            ReturnTotalSalePrice += ReturnSalePrice; //出库售价
                            ReturnTotalCount += ReturnCount; //出库数量
                            ReturnTotalDifference += resultModel.ReturnDifference.Value; //出库差价

                            SalesTotalCostPrice += SalesCostPrice; //销售成本价
                            SalesTotalSalePrice += SalesSalePrice; //销售售价
                            SalesTotalCount += SalesCount; //销售数量
                            SalesTotalDifference += resultModel.SalesDifference.Value; //销售差价

                            RefundSalesTotalCostPrice += RefundSalesCostPrice; //退费成本价
                            RefundSalesTotalSalePrice += RefundSalesSalePrice; //退费售价
                            RefundSalesTotalCount += RefundSalesCount; //退费数量
                            RefundSalesTotalDifference += resultModel.RefundSalesDifference.Value; //退费差价
                            if (dataList.Count <= 0)
                            {
                                resultModel.BeginCount = resultModel.EndCount;
                                resultModel.BeginCostPrice = resultModel.EndCostPrice;
                                resultModel.BeginSalePrice = resultModel.EndSalePrice;
                                resultModel.BeginDifference = resultModel.EndDifference;

                                //合计
                                BeginTotalCostPrice += resultModel.BeginCostPrice.Value; //期初成本单价
                                BeginTotalSalePrice += resultModel.BeginSalePrice.Value; //期初销售单价
                                BeginTotalCount += resultModel.BeginCount.Value; //期初数量
                                BeginTotalDifference += resultModel.BeginDifference.Value; //期初差价
                            }
                            result_list.Add(resultModel);
                            EndTotalCostPrice += resultModel.EndCostPrice.Value; //期末成本价
                            EndTotalSalePrice += resultModel.EndSalePrice.Value; //期末售价
                            EndTotalCount += resultModel.EndCount.Value; //期末数量
                            EndTotalDifference += resultModel.EndDifference.Value; //期末差价
                        }
                    }
                    result_list.Add(new ItemsTalesAndInventorySummaryModel()
                    {
                        SerialNumber = "合计",
                        BeginCount = BeginTotalCount,
                        BeginCostPrice = BeginTotalCostPrice,
                        BeginSalePrice = BeginTotalSalePrice,
                        BeginDifference = BeginTotalDifference,
                        PutInStorageCount = PutInStorageTotalCount,
                        PutInStorageCostPrice = PutInStorageTotalCostPrice,
                        PutInStorageSalePrice = PutInStorageTotalSalePrice,
                        PutInStorageDifference = PutInStorageTotalDifference,
                        CXPutInStorageCount = CXPutInStorageTotalCount,
                        CXPutInStorageCostPrice = CXPutInStorageTotalCostPrice,
                        CXPutInStorageSalePrice = CXPutInStorageTotalSalePrice,
                        CXPutInStorageDifference = CXPutInStorageTotalDifference,
                        OutboundCount = OutboundTotalCount,
                        OutboundCostPrice = OutboundTotalCostPrice,
                        OutboundSalePrice = OutboundTotalSalePrice,
                        OutboundDifference = OutboundTotalDifference,
                        ScrapCount = ScrapTotalCount,
                        ScrapCostPrice = ScrapTotalCostPrice,
                        ScrapSalePrice = ScrapTotalSalePrice,
                        ScrapDifference = ScrapTotalDifference,
                        ReceiveCount = ReceiveTotalCount,
                        ReceiveCostPrice = ReceiveTotalCostPrice,
                        ReceiveSalePrice = ReceiveTotalSalePrice,
                        ReceiveDifference = ReceiveTotalDifference,
                        CXReceiveCount = CXReceiveTotalCount,
                        CXReceiveCostPrice = CXReceiveTotalCostPrice,
                        CXReceiveSalePrice = CXReceiveTotalSalePrice,
                        CXReceiveDifference = CXReceiveTotalDifference,
                        ReturnCount = ReturnTotalCount,
                        ReturnCostPrice = ReturnTotalCostPrice,
                        ReturnSalePrice = ReturnTotalSalePrice,
                        ReturnDifference = ReturnTotalDifference,
                        SalesCount = SalesTotalCount,
                        SalesCostPrice = SalesTotalCostPrice,
                        SalesSalePrice = SalesTotalSalePrice,
                        SalesDifference = SalesTotalDifference,
                        RefundSalesCount = RefundSalesTotalCount,
                        RefundSalesCostPrice = RefundSalesTotalCostPrice,
                        RefundSalesSalePrice = RefundSalesTotalSalePrice,
                        RefundSalesDifference = RefundSalesTotalDifference,
                        EndCount = EndTotalCount,
                        EndCostPrice = EndTotalCostPrice,
                        EndSalePrice = EndTotalSalePrice,
                        EndDifference = EndTotalDifference
                    });
                }
                return result_list.OrderByDescending(t => t.ItemTypeName).ToArray();
            });

        }

        /// <summary>
        /// 进销存汇总表(简化)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ItemsTalesAndInventorySummaryModel[]> GetMaterialsEntersSellsSavesSimplifyAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                var isAll = false;
                List<MaterialsConfluenceOutPut> outPuts = new List<MaterialsConfluenceOutPut>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                var list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var inventory_list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var presdetail_list = new List<PrescriptionDetailModel>();
                var refund_list = new List<PrescriptionDetailModel>();
                string sqlwhere = string.Empty;
                string sqlStr = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  MedicalItemType in ({model.MedicalItemType})";
                    sqlStr = sqlwhere;
                }
                var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                string sql = GetQuerySql(xmlSqlParameter1);

                //查询处方明细信息
                var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                string priceSql = GetQuerySql(xmlSqlParameter2);

                //查询退费明细信息 
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsRefundByDate", "查询退费明细信息");
                string refundSql = GetQuerySql(xmlSqlParameter);
                var alist = new List<PrescriptionDetailModel>();
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND a.CenterId=@CenterId";
                    sql = sql + sqlwhere;
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);
                        if (putinTuple.Item1)
                        {
                            list.AddRange(putinTuple.Item2);
                        }
                        //获取截止当天期末数据
                        var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                        string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                        var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere, sqlParameterList.ToArray()));
                        if (primaryLibraryTuple.Item1)
                        {
                            inventory_list.AddRange(primaryLibraryTuple.Item2);
                        }
                        //查询处方单明细 
                        var sqlParameterList1 = new List<SqlParameter>();
                        sqlParameterList1.Add(new SqlParameter("@CenterId", center.Id));
                        var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + $" AND presDetail.CenterId=@CenterId " + sqlStr, sqlParameterList1.ToArray());
                        var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                        if (presdetailTuple.Item1)
                        {
                            presdetail_list.AddRange(presdetailTuple.Item2);
                            alist.AddRange(presdetailTuple.Item2);
                        }
                        //查询退费明细
                        var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + $" AND presDetail.CenterId=@CenterId " + sqlStr, sqlParameterList.ToArray());
                        var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                        if (refundTuple.Item1)
                        {
                            refund_list.AddRange(refundTuple.Item2);
                        }
                    }
                }
                else
                {
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere);
                    var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere));
                    if (putinTuple.Item1)
                    {
                        list = putinTuple.Item2;
                    }
                    //获取截止当天期末数据
                    var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                    string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                    var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere));
                    if (primaryLibraryTuple.Item1)
                    {
                        inventory_list = primaryLibraryTuple.Item2;
                    }
                    //查询处方单明细 
                    var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + sqlStr);
                    var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                    if (presdetailTuple.Item1)
                    {
                        presdetail_list = presdetailTuple.Item2;
                        alist.AddRange(presdetailTuple.Item2);
                    }
                    // //查询退费明细
                    var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + sqlStr);
                    var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                    if (refundTuple.Item1)
                    {
                        refund_list.AddRange(refundTuple.Item2);
                    }
                }
                //期初  入库 出库 期末
                var result_list = new List<ItemsTalesAndInventorySummaryModel>();
                decimal BeginTotalCostPrice = 0;//期初总额
                decimal BeginTotalSalePrice = 0;
                decimal BeginTotalCount = 0;
                decimal BeginTotalDifference = 0;
                // 入库 = 入库总额-冲销入库  出库= 其他出库+领用出库-冲销领用+报废+退货+销售-退费
                decimal PutInStorageTotalCostPrice = 0;//入库总额
                decimal PutInStorageTotalSalePrice = 0;
                decimal PutInStorageTotalCount = 0;
                decimal PutInStorageTotalDifference = 0;


                decimal OutboundTotalCostPrice = 0;//其它出库总额
                decimal OutboundTotalSalePrice = 0;
                decimal OutboundTotalCount = 0;
                decimal OutboundTotalDifference = 0;

                decimal EndTotalCostPrice = 0;//期末总额
                decimal EndTotalSalePrice = 0;
                decimal EndTotalCount = 0;
                decimal EndTotalDifference = 0;

                var now = DateTime.Now.Date.AddDays(1);
                if (inventory_list.Count > 0)
                {
                    int i = 0;
                    var allData = inventory_list.OrderBy(a => a.MedicalItemId).GroupBy(a => a.CenterId);
                    foreach (var items in allData)
                    {
                        var itemData = items.OrderBy(a => a.MedicalItemId).GroupBy(a => new { a.MedicalItemId, a.InPrice });
                        foreach (var item in itemData)
                        {
                            var resultModel = new ItemsTalesAndInventorySummaryModel();
                            //获取期末数据
                            var inventoryData = inventory_list.FindAll(a => a.CenterId == items.Key && a.MedicalItemId == item.Key.MedicalItemId && a.InPrice == item.Key.InPrice);
                            decimal totalCount = inventoryData.Sum(t => t.InQty.Value);
                            decimal totalCost = inventoryData.Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                            decimal totalSale = inventoryData.Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);
                            //期末数据
                            resultModel.EndCount = totalCount;
                            resultModel.EndCostPrice = totalCost;
                            resultModel.EndSalePrice = totalSale;
                            resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;

                            decimal BeginCostPrice = 0;
                            decimal BeginSalePrice = 0;
                            decimal BeginCount = 0;

                            decimal PutInStorageCostPrice = 0;
                            decimal PutInStorageSalePrice = 0;
                            decimal PutInStorageCount = 0;

                            decimal CXPutInStorageCostPrice = 0;
                            decimal CXPutInStorageSalePrice = 0;
                            decimal CXPutInStorageCount = 0;

                            decimal OutboundCostPrice = 0;
                            decimal OutboundSalePrice = 0;
                            decimal OutboundCount = 0;

                            decimal ScrapCostPrice = 0;
                            decimal ScrapSalePrice = 0;
                            decimal ScrapCount = 0;

                            decimal ReceiveCostPrice = 0;
                            decimal ReceiveSalePrice = 0;
                            decimal ReceiveCount = 0;

                            decimal CXReceiveCostPrice = 0;
                            decimal CXReceiveSalePrice = 0;
                            decimal CXReceiveCount = 0;

                            decimal ReturnCostPrice = 0;
                            decimal ReturnSalePrice = 0;
                            decimal ReturnCount = 0;

                            decimal SalesCostPrice = 0;
                            decimal SalesSalePrice = 0;
                            decimal SalesCount = 0;

                            decimal RefundSalesCostPrice = 0;
                            decimal RefundSalesSalePrice = 0;
                            decimal RefundSalesCount = 0;
                            var dataList = list.FindAll(a => a.CenterId == items.Key);
                            if (dataList.Count > 0)
                            {
                                //如果选择的时间不是当天  则需用当天的期末数据-出入库数据
                                if (model.EndTime < now)
                                {

                                    var dataInventory = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.EndTime && t.putInstorageDate.Value <= now);

                                    //定义
                                    decimal PutInStorageCostPriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageSalePriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageCountInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);

                                    //出库
                                    decimal OutboundCostPriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                    decimal OutboundSalePriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                    decimal OutboundCountInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);

                                    //期末数据
                                    resultModel.EndCount = resultModel.EndCount + OutboundCountInventory - PutInStorageCountInventory;
                                    resultModel.EndCostPrice = resultModel.EndCostPrice + OutboundCostPriceInventory - PutInStorageCostPriceInventory;
                                    resultModel.EndSalePrice = resultModel.EndSalePrice + OutboundSalePriceInventory - PutInStorageSalePriceInventory;
                                    resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                                }
                                //获取流水
                                var data = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.BeginTime.Value.Date && t.putInstorageDate.Value <= model.EndTime).ToList();

                                //计算出入库销售数量
                                PutInStorageCostPrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageSalePrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageCount = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value);

                                //冲销入库
                                CXPutInStorageCostPrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageSalePrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageCount = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);


                                //其它出库
                                OutboundCostPrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                OutboundSalePrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                OutboundCount = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value);
                                //报废出库
                                ScrapCostPrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ScrapSalePrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ScrapCount = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value);
                                //领用出库
                                ReceiveCostPrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReceiveSalePrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ReceiveCount = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value);
                                //冲销领用出库
                                CXReceiveCostPrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                CXReceiveSalePrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                CXReceiveCount = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value);

                                //退货出库
                                ReturnCostPrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReturnSalePrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                                ReturnCount = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value);

                                //销售
                                //销售金额
                                SalesSalePrice = 0;
                                //循环出库明细
                                var detail = data.Where(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").ToList();
                                SalesCostPrice = detail.Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                foreach (var result in detail)
                                {
                                    //var salesDetail = alist.ToList().Where(a => a.OutboundId == result.OutInBoundDetailId && a.MaterialId == item.Key.MedicalItemId).ToList();
                                    //if (result.ItemQty >= salesDetail[0].TotalQty)
                                    //{
                                    //    decimal? qty = 0;
                                    //    //循环处方明细
                                    //    foreach (var itemQty in salesDetail)
                                    //    {
                                    //        qty += itemQty.TotalQty;
                                    //        if (qty > result.ItemQty)
                                    //        {
                                    //            continue;
                                    //        }
                                    //        else
                                    //        {
                                    //            SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                    //            alist.ToList().Remove(itemQty);
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    SalesSalePrice += result.ItemQty.Value * salesDetail[0].SalePrice.Value;
                                    //}
                                    var salesDetail = alist.Where(a => a.OutboundId == result.OutInBoundDetailId && a.MaterialId == item.Key.MedicalItemId).OrderBy(a => a.TotalQty).ToList();
                                    //存在处方明细/出库明细  多对一  一对多的情况  就算售价需根据实际情况来 
                                    if (result.ItemQty == salesDetail.Sum(a => a.TotalQty))//多对一  一对一
                                    {
                                        foreach (var itemQty in salesDetail)
                                        {
                                            SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                        }
                                    }
                                    else
                                    {
                                        decimal? qty = 0;
                                        //循环处方明细
                                        foreach (var itemQty in salesDetail)
                                        {
                                            qty += itemQty.TotalQty;
                                            if (qty > result.ItemQty)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                SalesSalePrice += itemQty.TotalQty.Value * itemQty.SalePrice.Value;
                                                alist.Remove(itemQty);
                                            }

                                        }
                                    }
                                }
                                SalesCount = detail.Sum(t => t.ItemQty.Value);
                                //退费
                                RefundSalesCostPrice = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                var refundListStr = string.Join(",", data.Where(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Select(a => a.OutInBoundDetailId).ToArray());
                                var refundDetail = refund_list.Where(a => refundListStr.Contains(a.ReturnWarehouseId));
                                RefundSalesSalePrice = refundDetail.Sum(a => a.SalePrice.Value * a.TotalQty.Value);

                                RefundSalesCount = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);


                                var SalePrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //期初数据
                                BeginCount = resultModel.EndCount.Value + OutboundCount + ScrapCount + ReceiveCount + ReturnCount + SalesCount - PutInStorageCount - CXReceiveCount - RefundSalesCount + CXPutInStorageCount;
                                BeginCostPrice = resultModel.EndCostPrice.Value + OutboundCostPrice + ScrapCostPrice + ReceiveCostPrice + ReturnCostPrice + SalesCostPrice - PutInStorageCostPrice - CXReceiveCostPrice - RefundSalesCostPrice + CXPutInStorageCostPrice;
                                BeginSalePrice = resultModel.EndSalePrice.Value + OutboundSalePrice + ScrapSalePrice + ReceiveSalePrice + ReturnSalePrice + SalePrice - PutInStorageSalePrice - CXReceiveSalePrice + CXPutInStorageSalePrice;
                            }
                            i++;
                            var _data = item.First();
                            resultModel.SerialNumber = i.ToString();
                            resultModel.MedicalItemName = _data.MedicalItemName;
                            resultModel.NationItemCode = _data.NationItemCode;

                            resultModel.DialysisName = _data.DialysisName;
                            resultModel.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                            resultModel.Manufacturer = _data.Manufacturer;
                            resultModel.UnitName = _data.IsSplited.Value == true ? _data.SpecificationsUnitName : _data.PackageUnitName;
                            resultModel.InPrice = _data.InPrice.HasValue ? _data.InPrice : 0;
                            resultModel.ItemTypeName = _data.ItemTypeName;
                            resultModel.BeginCount = BeginCount;
                            resultModel.BeginCostPrice = BeginCostPrice;
                            resultModel.BeginSalePrice = BeginSalePrice;
                            resultModel.BeginDifference = BeginSalePrice - BeginCostPrice;

                            resultModel.PutInStorageCount = PutInStorageCount - CXPutInStorageCount;
                            resultModel.PutInStorageCostPrice = PutInStorageCostPrice - CXPutInStorageCostPrice;
                            resultModel.PutInStorageSalePrice = PutInStorageSalePrice - CXPutInStorageSalePrice;
                            resultModel.PutInStorageDifference = resultModel.PutInStorageSalePrice - resultModel.PutInStorageCostPrice;

                            resultModel.OutboundCount = OutboundCount + ReceiveCount - CXReceiveCount + ScrapCount + ReturnCount + SalesCount - RefundSalesCount;
                            resultModel.OutboundCostPrice = OutboundCostPrice + ReceiveCostPrice - CXReceiveCostPrice + ScrapCostPrice + ReturnCostPrice + SalesCostPrice - RefundSalesCostPrice;
                            resultModel.OutboundSalePrice = OutboundSalePrice + ReceiveSalePrice - CXReceiveSalePrice + ScrapSalePrice + ReturnSalePrice + SalesSalePrice - RefundSalesSalePrice; ;
                            resultModel.OutboundDifference = resultModel.OutboundSalePrice - resultModel.OutboundCostPrice;

                            //合计
                            BeginTotalCostPrice += BeginCostPrice; //期初成本单价
                            BeginTotalSalePrice += BeginSalePrice; //期初销售单价
                            BeginTotalCount += BeginCount; //期初数量
                            BeginTotalDifference += resultModel.BeginDifference.Value; //期初差价
                            PutInStorageTotalCostPrice += resultModel.PutInStorageCostPrice.Value; //入库成本价
                            PutInStorageTotalSalePrice += resultModel.PutInStorageSalePrice.Value; //入库售价
                            PutInStorageTotalCount += resultModel.PutInStorageCount.Value; //入库数量
                            PutInStorageTotalDifference += resultModel.PutInStorageDifference.Value;//入库差价



                            OutboundTotalCostPrice += resultModel.OutboundCostPrice.Value; //出库成本价
                            OutboundTotalSalePrice += resultModel.OutboundSalePrice.Value; //出库售价
                            OutboundTotalCount += resultModel.OutboundCount.Value; //出库数量
                            OutboundTotalDifference += resultModel.OutboundDifference.Value; //出库差价
                                                                                             //报废
                            if (dataList.Count <= 0)
                            {
                                resultModel.BeginCount = resultModel.EndCount;
                                resultModel.BeginCostPrice = resultModel.EndCostPrice;
                                resultModel.BeginSalePrice = resultModel.EndSalePrice;
                                resultModel.BeginDifference = resultModel.EndDifference;

                                //合计
                                BeginTotalCostPrice += resultModel.BeginCostPrice.Value; //期初成本单价
                                BeginTotalSalePrice += resultModel.BeginSalePrice.Value; //期初销售单价
                                BeginTotalCount += resultModel.BeginCount.Value; //期初数量
                                BeginTotalDifference += resultModel.BeginDifference.Value; //期初差价
                            }
                            result_list.Add(resultModel);
                            EndTotalCostPrice += resultModel.EndCostPrice.Value; //期末成本价
                            EndTotalSalePrice += resultModel.EndSalePrice.Value; //期末售价
                            EndTotalCount += resultModel.EndCount.Value; //期末数量
                            EndTotalDifference += resultModel.EndDifference.Value; //期末差价
                        }
                    }
                    result_list.Add(new ItemsTalesAndInventorySummaryModel()
                    {
                        SerialNumber = "合计",
                        BeginCount = BeginTotalCount,
                        BeginCostPrice = BeginTotalCostPrice,
                        BeginSalePrice = BeginTotalSalePrice,
                        BeginDifference = BeginTotalDifference,
                        PutInStorageCount = PutInStorageTotalCount,
                        PutInStorageCostPrice = PutInStorageTotalCostPrice,
                        PutInStorageSalePrice = PutInStorageTotalSalePrice,
                        PutInStorageDifference = PutInStorageTotalDifference,

                        OutboundCount = OutboundTotalCount,
                        OutboundCostPrice = OutboundTotalCostPrice,
                        OutboundSalePrice = OutboundTotalSalePrice,
                        OutboundDifference = OutboundTotalDifference,

                        EndCount = EndTotalCount,
                        EndCostPrice = EndTotalCostPrice,
                        EndSalePrice = EndTotalSalePrice,
                        EndDifference = EndTotalDifference
                    });
                }
                return result_list.OrderByDescending(t => t.ItemTypeName).ToArray();
            });

        }


        //物品实际入库和销售情况统计 ItemFortheModel
        public Task<ItemFortheModel[]> GetItemFortheModelAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                List<ItemFortheModel> itemForthes = new List<ItemFortheModel>();
                var data = await GetMaterialsEntersSellsSavesAsync(model);
                int i = 1;
                foreach (var item in data)
                {
                    var count = (item.EndCount - item.BeginCount + item.SalesCount);
                    itemForthes.Add(new ItemFortheModel()
                    {
                        BeginCount = item.BeginCount,
                        EndCount = item.EndCount,
                        ItemTypeName = item.ItemTypeName,
                        Manufacturer = item.Manufacturer,
                        MedicalItemName = item.MedicalItemName,
                        SalesCount = item.SalesCount,
                        Specifications = item.Specifications,
                        UnitName = item.UnitName,
                        putInStorageCount = count < 0 ? 0 : count,
                        SerialNumber = i + "",

                    });
                    i++;
                }

                return itemForthes.ToArray();
            });
        }




        private void GetSalePrice(List<PrescriptionDetailModel> presdetail_list, List<PrescriptionDetailModel> refund_list, Dictionary<string, List<string>> keyDic, Dictionary<string, decimal> priceDic, Dictionary<string, decimal> RefundPriceDic, List<ItemsTalesAndInventorySummarySqlModel> dataInventory, string MedicalItemId, string CenterId)
        {
            var refundDic = new Dictionary<string, List<string>>();
            foreach (var dicItem in dataInventory.FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f"))
            {
                dicItem.InPrice = dicItem.InPrice.HasValue ? dicItem.InPrice : 0;
                if (keyDic.ContainsKey(dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId)) { keyDic[dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId].Add(dicItem.OutInBoundDetailId); }
                else { keyDic.Add(dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId, new List<string>() { dicItem.OutInBoundDetailId }); }
            }

            foreach (var dicItem in dataInventory.FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1"))
            {
                dicItem.InPrice = dicItem.InPrice.HasValue ? dicItem.InPrice : 0;
                if (refundDic.ContainsKey(dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId)) { refundDic[dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId].Add(dicItem.OutInBoundDetailId); }
                else { refundDic.Add(dicItem.MedicalItemId + "," + dicItem.InPrice + "," + dicItem.CenterId, new List<string>() { dicItem.OutInBoundDetailId }); }

            }
            if (keyDic.Count > 0)
            {
                foreach (var dicItem in keyDic.Keys)
                {
                    if (keyDic[dicItem].Count > 0)
                    {
                        decimal ReceivablePrice = 0;
                        decimal refundPrice = 0;
                        if (presdetail_list.Count > 0)
                        {
                            ReceivablePrice = presdetail_list.FindAll(a => keyDic[dicItem].Contains(a.OutboundId) && a.MaterialId == MedicalItemId && a.CenterId == CenterId).Sum(a => a.SalePrice * a.TotalQty).Value;
                        }
                        if (refund_list.Count > 0)
                        {
                            if (refundDic.Keys.Contains(dicItem))
                            {
                                refundPrice = refund_list.FindAll(a => refundDic[dicItem].Contains(a.ReturnWarehouseId) && a.MaterialId == MedicalItemId && a.CenterId == CenterId).Sum(a => a.TotalQty * a.SalePrice).Value;
                            }
                        }
                        priceDic.Add(dicItem, ReceivablePrice);
                        RefundPriceDic.Add(dicItem, refundPrice);
                    }
                }
            }
        }


        /// <summary>
        /// 根据供应商查询进销存汇总表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ItemsTalesAndInventorySummaryModel[]> GetMaterialsEntersSellsBySupplierIdSavesAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                var isAll = false;
                List<MaterialsConfluenceOutPut> outPuts = new List<MaterialsConfluenceOutPut>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                var list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var inventory_list = new List<ItemsTalesAndInventorySummarySqlModel>();
                string sqlwhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  MedicalItemType in ({model.MedicalItemType})";
                }
                if (model.SupplierId != "0" && !string.IsNullOrWhiteSpace(model.SupplierId))
                {
                    sqlwhere += $" and  SupplierId ='{model.SupplierId}'";
                }
                var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                string sql = GetQuerySql(xmlSqlParameter1);

                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND a.CenterId=@CenterId";
                    sql = sql + sqlwhere;
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(dt);
                        if (putinTuple.Item1)
                        {
                            list.AddRange(putinTuple.Item2);
                        }
                        //获取截止当天期末数据
                        var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                        string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                        var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere, sqlParameterList.ToArray()));
                        if (primaryLibraryTuple.Item1)
                        {
                            inventory_list.AddRange(primaryLibraryTuple.Item2);
                        }
                    }
                }
                else
                {
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere);
                    var putinTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere));
                    if (putinTuple.Item1)
                    {
                        list = putinTuple.Item2;
                    }
                    //获取截止当天期末数据
                    var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                    string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                    var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere));
                    if (primaryLibraryTuple.Item1)
                    {
                        inventory_list = primaryLibraryTuple.Item2;
                    }
                }

                var result_list = new List<ItemsTalesAndInventorySummaryModel>();
                decimal BeginTotalCostPrice = 0;//期初总额
                decimal BeginTotalCount = 0;

                decimal PutInStorageTotalCostPrice = 0;//入库总额
                decimal PutInStorageTotalCount = 0;

                decimal CXPutInStorageTotalCostPrice = 0;//入库总额
                decimal CXPutInStorageTotalCount = 0;


                decimal OutboundTotalCostPrice = 0;//其它出库总额
                decimal OutboundTotalCount = 0;

                decimal ScrapTotalCostPrice = 0;//报废出库总额
                decimal ScrapTotalCount = 0;


                decimal ReceiveTotalCostPrice = 0;//领用出库总额
                decimal ReceiveTotalCount = 0;

                decimal CXReceiveTotalCostPrice = 0;//冲销领用出库总额
                decimal CXReceiveTotalCount = 0;

                decimal ReturnTotalCostPrice = 0;//退货出库总额
                decimal ReturnTotalCount = 0;

                decimal SalesTotalCostPrice = 0;//销售总额
                decimal SalesTotalCount = 0;

                decimal RefundSalesTotalCostPrice = 0;//退费退库
                decimal RefundSalesTotalCount = 0;

                decimal EndTotalCostPrice = 0;//期末总额
                decimal EndTotalCount = 0;

                var now = DateTime.Now.Date.AddDays(1);
                if (inventory_list.Count > 0)
                {
                    int i = 0;
                    var allData = inventory_list.OrderBy(a => a.MedicalItemId).GroupBy(a => a.CenterId);
                    foreach (var items in allData)
                    {

                        //新增分组时增加供应商分组
                        var itemData = items.OrderBy(a => a.MedicalItemId).GroupBy(a => new { a.MedicalItemId, a.InPrice, a.SupplierId });
                        foreach (var item in itemData)
                        {
                            var resultModel = new ItemsTalesAndInventorySummaryModel();
                            //获取期末数据
                            var inventoryData = inventory_list.FindAll(a => a.CenterId == items.Key && a.MedicalItemId == item.Key.MedicalItemId && a.InPrice == item.Key.InPrice && a.SupplierId == item.Key.SupplierId);
                            decimal totalCount = inventoryData.Sum(t => t.InQty.Value);
                            decimal totalCost = inventoryData.Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                            //期末数据
                            resultModel.EndCount = totalCount;
                            resultModel.EndCostPrice = totalCost;

                            decimal BeginCostPrice = 0;
                            decimal BeginCount = 0;

                            decimal PutInStorageCostPrice = 0;
                            decimal PutInStorageCount = 0;

                            decimal CXPutInStorageCostPrice = 0;
                            decimal CXPutInStorageCount = 0;

                            decimal OutboundCostPrice = 0;
                            decimal OutboundCount = 0;

                            decimal ScrapCostPrice = 0;
                            decimal ScrapCount = 0;

                            decimal ReceiveCostPrice = 0;
                            decimal ReceiveCount = 0;

                            decimal CXReceiveCostPrice = 0;
                            decimal CXReceiveCount = 0;

                            decimal ReturnCostPrice = 0;
                            decimal ReturnCount = 0;

                            decimal SalesCostPrice = 0;
                            decimal SalesCount = 0;

                            decimal RefundSalesCostPrice = 0;
                            decimal RefundSalesCount = 0;
                            var dataList = list.FindAll(a => a.CenterId == items.Key);
                            if (dataList.Count > 0)
                            {
                                //如果选择的时间不是当天  则需用当天的期末数据-出入库数据
                                if (model.EndTime < now)
                                {

                                    var dataInventory = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.EndTime && t.putInstorageDate.Value <= now && t.SupplierId == item.Key.SupplierId);

                                    //定义
                                    decimal PutInStorageCostPriceInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                    decimal PutInStorageCountInventory = dataInventory.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);

                                    //出库
                                    decimal OutboundCostPriceInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                    decimal OutboundCountInventory = dataInventory.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value) - dataInventory.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989" || t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34" || t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);

                                    //期末数据
                                    resultModel.EndCount = resultModel.EndCount + OutboundCountInventory - PutInStorageCountInventory;
                                    resultModel.EndCostPrice = resultModel.EndCostPrice + OutboundCostPriceInventory - PutInStorageCostPriceInventory;
                                    resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                                }
                                //获取流水
                                var data = dataList.Where(t => t.MedicalItemId == item.Key.MedicalItemId && t.InPrice == item.Key.InPrice && t.putInstorageDate.Value >= model.BeginTime.Value.Date && t.putInstorageDate.Value <= model.EndTime && t.SupplierId == item.Key.SupplierId).ToList();
                                //计算出入库销售数量
                                PutInStorageCostPrice = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                PutInStorageCount = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value);

                                //冲销入库
                                CXPutInStorageCostPrice = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                                CXPutInStorageCount = data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);


                                //其它出库
                                OutboundCostPrice = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                OutboundCount = data.Where(t => t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef").Sum(t => t.ItemQty.Value) - data.Where(t => t.ItemType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty.Value);
                                //报废出库
                                ScrapCostPrice = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ScrapCount = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e").Sum(t => t.ItemQty.Value);
                                //领用出库
                                ReceiveCostPrice = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReceiveCount = data.Where(t => t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value);
                                //冲销领用出库
                                CXReceiveCostPrice = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                CXReceiveCount = data.Where(t => t.ItemType == "adb5fdd90a064b25ac080cb3d8d93989").Sum(t => t.ItemQty.Value);

                                //退货出库
                                ReturnCostPrice = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                ReturnCount = data.Where(t => t.ItemType == "20e266be8d0b42679f621b91111e04ab").Sum(t => t.ItemQty.Value);

                                //销售出库
                                SalesCostPrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                SalesCount = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value);
                                //退费退库
                                RefundSalesCostPrice = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                                RefundSalesCount = data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);


                                var SalePrice = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));

                                //期初数据
                                BeginCount = resultModel.EndCount.Value + OutboundCount + ScrapCount + ReceiveCount + ReturnCount + SalesCount - PutInStorageCount - CXReceiveCount - RefundSalesCount + CXPutInStorageCount;
                                BeginCostPrice = resultModel.EndCostPrice.Value + OutboundCostPrice + ScrapCostPrice + ReceiveCostPrice + ReturnCostPrice + SalesCostPrice - PutInStorageCostPrice - CXReceiveCostPrice - RefundSalesCostPrice + CXPutInStorageCostPrice;
                            }
                            i++;
                            var _data = item.First();
                            resultModel.SerialNumber = i.ToString();
                            resultModel.MedicalItemName = _data.MedicalItemName;
                            resultModel.DialysisName = _data.DialysisName;
                            resultModel.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                            resultModel.Manufacturer = _data.Manufacturer;
                            resultModel.UnitName = _data.IsSplited.Value == true ? _data.SpecificationsUnitName : _data.PackageUnitName;
                            resultModel.InPrice = _data.InPrice.HasValue ? _data.InPrice : 0;
                            resultModel.ItemTypeName = _data.ItemTypeName;
                            resultModel.SupplierName = _data.SupplierName;
                            resultModel.BeginCount = BeginCount;
                            resultModel.BeginCostPrice = BeginCostPrice;
                            resultModel.PutInStorageCount = PutInStorageCount;
                            resultModel.PutInStorageCostPrice = PutInStorageCostPrice;

                            resultModel.CXPutInStorageCount = CXPutInStorageCount;//冲销入库
                            resultModel.CXPutInStorageCostPrice = CXPutInStorageCostPrice;

                            resultModel.OutboundCount = OutboundCount;
                            resultModel.OutboundCostPrice = OutboundCostPrice;

                            resultModel.ScrapCount = ScrapCount;
                            resultModel.ScrapCostPrice = ScrapCostPrice;

                            resultModel.ReceiveCount = ReceiveCount;
                            resultModel.ReceiveCostPrice = ReceiveCostPrice;

                            resultModel.CXReceiveCount = CXReceiveCount;//冲销领用出库
                            resultModel.CXReceiveCostPrice = CXReceiveCostPrice;

                            resultModel.ReturnCount = ReturnCount;
                            resultModel.ReturnCostPrice = ReturnCostPrice;

                            resultModel.SalesCount = SalesCount;
                            resultModel.SalesCostPrice = SalesCostPrice;


                            resultModel.RefundSalesCount = RefundSalesCount;
                            resultModel.RefundSalesCostPrice = RefundSalesCostPrice;
                            //合计
                            BeginTotalCostPrice += BeginCostPrice; //期初成本单价
                            BeginTotalCount += BeginCount; //期初数量
                            PutInStorageTotalCostPrice += PutInStorageCostPrice; //入库成本价
                            PutInStorageTotalCount += PutInStorageCount; //入库数量

                            CXPutInStorageTotalCostPrice += CXPutInStorageCostPrice; //冲销入库成本价
                            CXPutInStorageTotalCount += CXPutInStorageCount; //冲销入库数量


                            OutboundTotalCostPrice += OutboundCostPrice; //出库成本价
                            OutboundTotalCount += OutboundCount; //出库数量

                            ScrapTotalCostPrice += ScrapCostPrice;//报废出库成本价
                            ScrapTotalCount += ScrapCount; //出库数量

                            ReceiveTotalCostPrice += ReceiveCostPrice; ////领用出库成本价
                            ReceiveTotalCount += ReceiveCount; //出库数量

                            CXReceiveTotalCostPrice += CXReceiveCostPrice; ////冲销领用出库成本价
                            CXReceiveTotalCount += CXReceiveCount; //出库数量

                            ReturnTotalCostPrice += ReturnCostPrice; ////退货出库成本价
                            ReturnTotalCount += ReturnCount; //出库数量

                            SalesTotalCostPrice += SalesCostPrice; //销售成本价
                            SalesTotalCount += SalesCount; //销售数量

                            RefundSalesTotalCostPrice += RefundSalesCostPrice; //退费成本价
                            RefundSalesTotalCount += RefundSalesCount; //退费数量
                            if (dataList.Count <= 0)
                            {
                                resultModel.BeginCount = resultModel.EndCount;
                                resultModel.BeginCostPrice = resultModel.EndCostPrice;
                                //合计
                                BeginTotalCostPrice += resultModel.BeginCostPrice.Value; //期初成本单价
                                BeginTotalCount += resultModel.BeginCount.Value; //期初数量
                            }
                            result_list.Add(resultModel);
                            EndTotalCostPrice += resultModel.EndCostPrice.Value; //期末成本价
                            EndTotalCount += resultModel.EndCount.Value; //期末数量
                        }
                    }
                    result_list.Add(new ItemsTalesAndInventorySummaryModel()
                    {
                        SerialNumber = "合计",
                        BeginCount = BeginTotalCount,
                        BeginCostPrice = BeginTotalCostPrice,
                        PutInStorageCount = PutInStorageTotalCount,
                        PutInStorageCostPrice = PutInStorageTotalCostPrice,
                        CXPutInStorageCount = CXPutInStorageTotalCount,
                        CXPutInStorageCostPrice = CXPutInStorageTotalCostPrice,
                        OutboundCount = OutboundTotalCount,
                        OutboundCostPrice = OutboundTotalCostPrice,
                        ScrapCount = ScrapTotalCount,
                        ScrapCostPrice = ScrapTotalCostPrice,
                        ReceiveCount = ReceiveTotalCount,
                        ReceiveCostPrice = ReceiveTotalCostPrice,
                        CXReceiveCount = CXReceiveTotalCount,
                        CXReceiveCostPrice = CXReceiveTotalCostPrice,
                        ReturnCount = ReturnTotalCount,
                        ReturnCostPrice = ReturnTotalCostPrice,
                        SalesCount = SalesTotalCount,
                        SalesCostPrice = SalesTotalCostPrice,
                        RefundSalesCount = RefundSalesTotalCount,
                        RefundSalesCostPrice = RefundSalesTotalCostPrice,
                        EndCount = EndTotalCount,
                        EndCostPrice = EndTotalCostPrice
                    });
                }
                return result_list.ToArray();
            });
        }

        /// <summary>
        /// 外购入库开单信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<PutInStorageBillTjPageModel[]> GetPutInStorageAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                int i = 0;
                var result_list = new List<PutInStorageBillTjPageModel>();
                decimal? TotalQty = 0;
                decimal? SalePriceSum = 0;
                decimal? InPriceSum = 0;
                decimal? Difference = 0;
                string sqlwhere = string.Empty;
                string InStorageNo = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  b.MedicalItemType in ({model.MedicalItemType})";
                }
                if (!string.IsNullOrWhiteSpace(model.OrderNo))
                {
                    InStorageNo += $" and  T1.InStorageNo like '%{model.OrderNo}%'";
                }
                if (model.SupplierId + "" != "" && model.SupplierId + "" != "0")
                    InStorageNo += $" and  T1.SupplierId like '%{model.SupplierId}%'";

                if (model.IsSettlement.HasValue && model.IsSettlement != 2 && (model.IsSettlement == 0 || model.IsSettlement == 1))
                {
                    InStorageNo += $" AND T1.IsSettlement= {model.IsSettlement.Value}";
                }
                //3 未勾稽  4 已勾稽
                if (model.IsSettlement.HasValue && model.IsSettlement != 2 && (model.IsSettlement == 3 || model.IsSettlement == 4))
                {
                    InStorageNo += $" AND T1.IsChecked= {model.IsSettlement.Value - 3}";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetPutInStorageBillInfoByDate", "时间段查询入库开单信息");
                string sql = GetQuerySql(xmlSqlParameter);
                var list = new List<PutInStorageBillTjPageModel>();
                var isAll = false;
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    InStorageNo += $" AND T1.CenterId=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere).Replace("@sql", InStorageNo);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<PutInStorageBillTjPageModel>(dt);
                        if (tuple.Item1)
                        {
                            list.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere).Replace("@sql", InStorageNo);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<PutInStorageBillTjPageModel>(dt);
                    if (tuple.Item1)
                    {
                        list = tuple.Item2;
                    }
                }

                if (list.Count > 0)
                {
                    var data = list.FindAll(a => a.TotalQty > 0).OrderBy(a => a.DialysisName);
                    foreach (var item in data)
                    {
                        i++;
                        item.RowNumber = i.ToString();
                        item.InPriceSum = item.InPriceSum.Value.ToRounds(2);
                        item.Difference = item.SalePriceSum - item.InPriceSum;
                        if (item.InTypeId == "0fae04062a3547f8b5597903a639118d")//冲销入库
                        {
                            item.TotalQty = -item.TotalQty;
                            item.SalePriceSum = -item.SalePriceSum;
                            item.InPriceSum = -item.InPriceSum;
                            Difference -= item.Difference;
                        }
                        else
                        {
                            Difference += item.Difference;
                        }
                        item.IsSettlement = item.IsSettlement.HasValue ? item.IsSettlement : 0;
                        item.SettlementDate = item.SettlementDate;
                        TotalQty += item.TotalQty;
                        SalePriceSum += item.SalePriceSum;
                        InPriceSum += item.InPriceSum;
                    }
                    result_list.AddRange(data);
                    result_list.Add(new PutInStorageBillTjPageModel()
                    {
                        RowNumber = "合计",
                        TotalQty = TotalQty,
                        InPriceSum = InPriceSum,
                        SalePriceSum = SalePriceSum,
                        Difference = Difference,
                    });
                }
                //return new PageData<PutInStorageBillTjPageModel[]>(result_list.ToArray(), count);
                // result_list = result_list.OrderBy(a => new { a.DialysisName, a.InStorageNo }).ToList(); ;
                return result_list.ToArray();
            });
        }


        /// <summary>
        /// 外购入库开单结算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> UpdatePutInStorageBillSettlementAsync(SettlemenInPuts model)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (model.Id != null)
                    {
                        var userId = await _getUserInfo.GetCurrentUserIdAsync();
                        foreach (var item in model.Id)
                        {
                            var data = await PutInStorageBillStore.GetFirstOrDefaultAsync(t => t.Id == item);
                            if (data == null)
                                continue;
                            if (data.IsChecked == 0)
                                continue;
                            data.IsSettlement = 1;
                            data.SettlementDate = model.SettlementDate;
                            data.SettlementPeople = userId;
                            PutInStorageBillStore.Update(data);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        /// <summary>
        /// 外购结算前核对勾稽
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> UpdatePutInStorageBillCheckedAsync(SettlemenInPuts model)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (model.Id != null)
                    {
                        var userId = await _getUserInfo.GetCurrentUserIdAsync();
                        foreach (var item in model.Id)
                        {
                            var data = await PutInStorageBillStore.GetFirstOrDefaultAsync(t => t.Id == item);
                            if (data == null)
                                continue;
                            data.CheckedDate = model.SettlementDate;
                            data.CheckedPeople = userId;
                            data.IsChecked = 1;
                            PutInStorageBillStore.Update(data);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        //ReturnSettlementCheckedAsync
        /// <summary>
        /// 退货结算前核对勾稽
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> ReturnSettlementCheckedAsync(SettlemenInPuts model)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (model.Id != null)
                    {
                        var userId = await _getUserInfo.GetCurrentUserIdAsync();
                        foreach (var item in model.Id)
                        {

                            var data = await ReturnRequestStore.GetFirstOrDefaultAsync(t => t.Id == item);
                            if (data == null)
                                continue;
                            data.CheckedDate = model.SettlementDate;
                            data.CheckedPeople = userId;
                            data.IsChecked = 1;
                            ReturnRequestStore.Update(data);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        /// <summary>
        /// 外购入库明细信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<PutInStorageBillDetailsTjTmpModel[]> GetPutInStorageDetailAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                decimal? TotalQty = 0;
                decimal? SalesAmount = 0;
                decimal? CostAmount = 0;
                int? i = 0;
                var result_list = new List<PutInStorageBillDetailsTjTmpModel>();
                string sqlwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.SupplierName))
                {
                    sqlwhere += $" and T1.SupplierName like '%{model.SupplierName}%'";
                }
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  T4.MedicalItemType in ({model.MedicalItemType})";
                }
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlwhere = $" AND T1.PutInStorageBillId= '{model.Id}'";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetPutInStorageBillDetailsInfoByDate", "时间段查询入库开单明细信息");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var list = new List<PutInStorageBillDetailsTjTmpModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND T7.CenterId=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<PutInStorageBillDetailsTjTmpModel>(dt);
                        if (tuple.Item1)
                        {
                            list.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<PutInStorageBillDetailsTjTmpModel>(dt);
                    if (tuple.Item1)
                    {
                        list = tuple.Item2;
                    }
                }
                if (list.Count > 0)
                {
                    list = list.OrderBy(a => a.DialysisName).ToList();
                    foreach (var item in list)
                    {
                        i++;
                        item.RowNumber = i.ToString();
                        item.SalesAmount = item.InQty * item.SalePrice;
                        item.CostAmount = item.CostAmount.Value.ToRounds(2);  //item.InQty * item.InPrice;
                        if (item.InTypeId == "0fae04062a3547f8b5597903a639118d")
                        {
                            item.InQty = -item.InQty;
                            item.SalesAmount = -item.SalesAmount;
                            item.CostAmount = -item.CostAmount;
                        }
                        if (item.Type == 3)
                        {
                            item.Specifications = item.Brand;
                        }
                        if (item.ProcurementPackage == "/" || item.ProcurementPackage == item.Specifications)
                            item.ProcurementPackage = "";
                        TotalQty += item.InQty;
                        SalesAmount += item.SalesAmount;
                        CostAmount += item.CostAmount;
                        if (item.Type == 1)
                            item.ApprovalNum = item.ApprovalNum + "" == "" ? "" : item.ApprovalNum.Contains("H") ? "西药" : item.ApprovalNum.Contains("Z") ? "中成药" : item.ApprovalNum.Contains("J") ? "西药" : item.ApprovalNum.Contains("S") ? "西药" : "";
                        else
                        {
                            item.ApprovalNum = item.CatalogueName + "" == "" ? "" : item.CatalogueName;
                        }
                    }
                    result_list.AddRange(list);
                    result_list.Add(new PutInStorageBillDetailsTjTmpModel()
                    {
                        RowNumber = "合计",
                        InQty = TotalQty,
                        CostAmount = CostAmount,
                        SalesAmount = SalesAmount,
                    });
                }
                //return new PageData<PutInStorageBillDetailsTjTmpModel[]>(result_list.ToArray(), count);
                //
                // result_list = result_list.OrderBy(a => new { a.DialysisName, a.InStorageNo }).ToList();
                return result_list.ToArray();
            });
        }


        /// <summary>
        ///其它出库统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundModel[]> GetOtherOutboundByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                int i = 0;
                var result_list = new List<OtherOutboundModel>();
                var sqlwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.OrderNo))
                {
                    sqlwhere = $" AND outbound.OutboundNo like '%{model.OrderNo}%'";
                }
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  mir.MedicalItemType in ({model.MedicalItemType})";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetOtherOutboundByDate", "时间段查询其它出库信息");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var TupleList = new List<OtherOutboundModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部  
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND center.Id=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundModel>(dt);
                        if (tuple.Item1)
                        {
                            TupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundModel>(dt);
                    if (tuple.Item1)
                    {
                        TupleList = tuple.Item2;
                    }
                }

                if (TupleList.Count > 0)
                {
                    var list = new List<OtherOutboundModel>();
                    foreach (var item in TupleList)
                    {
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;
                        if (item.OutboundType == "92ea5788bb044fb08fa71ce13dfefb34")//冲销   
                        {
                            item.CostAmount = -item.CostAmount;

                            item.SalesAmount = -item.SalesAmount;
                        }

                        var outboundModel = new OtherOutboundModel();
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.CenterId = item.CenterId;
                        outboundModel.Id = item.Id;
                        outboundModel.Name = item.Name;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.AuditorName = item.AuditorName;
                        outboundModel.DifferenceAmount = outboundModel.SalesAmount - outboundModel.CostAmount;
                        outboundModel.FounderDate = item.FounderDate;
                        outboundModel.AuditDate = item.AuditDate;
                        outboundModel.Remark = item.Remark;
                        outboundModel.OutboundType = item.OutboundType == "b8e6cbd359fb4816baf9e59206d4a5ef" ? "其它出库" : "冲销其它出库";
                        list.Add(outboundModel);
                    }
                    var OutboundTuple = list.GroupBy(a => new { a.CenterId, a.OutboundNo });
                    foreach (var item in OutboundTuple)
                    {
                        i++;
                        var CostAmount = item.ToList().Sum(a => a.CostAmount);
                        var SalesAmount = item.ToList().Sum(a => a.SalesAmount);
                        var DifferenceAmount = SalesAmount - CostAmount;
                        var _data = item.First();

                        _data.CostAmount = CostAmount;
                        _data.SalesAmount = SalesAmount;
                        _data.SerialNumber = i.ToString();
                        _data.DifferenceAmount = DifferenceAmount;

                        totalCostAmount += _data.CostAmount;
                        totalSalesAmount += _data.SalesAmount;
                        totalDifferenceAmount += DifferenceAmount;
                        result_list.Add(_data);
                    }
                    result_list.Add(new OtherOutboundModel()
                    {
                        SerialNumber = "合计",
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount,
                    });
                }
                //return new PageData<OtherOutboundModel[]>(result_list.ToArray(), count);
                return result_list.ToArray();
            });
        }



        /// <summary>
        /// 其它出库明细统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundDetailModel[]> GetOtherOutboundDetailByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                var sqlwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlwhere += $" AND outdetail.MaterialOutboundId='{model.Id}' ";
                }
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlwhere += $" and  mir.MedicalItemType in ({model.MedicalItemType})";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetOtherOutboundDetailByDate", "时间段查询其它出库明细信息");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var list = new List<OtherOutboundDetailModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += $" AND center.Id=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                        if (tuple.Item1)
                        {
                            list.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@sqlWhere", sqlwhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                    if (tuple.Item1)
                    {
                        list = tuple.Item2;
                    }
                }

                var result_list = new List<OtherOutboundDetailModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalCount = 0;
                decimal? totalDifferenceAmount = 0;
                int i = 0;
                if (list.Count > 0)
                {
                    list = list.OrderBy(a => a.DialysisName).ToList();
                    foreach (var item in list)
                    {
                        i++;
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;
                        var name = string.IsNullOrWhiteSpace(item.Brand) ? "" : "(" + item.Brand + ")";
                        if (item.OutboundType == "92ea5788bb044fb08fa71ce13dfefb34")//冲销
                        {
                            item.CostAmount = -item.CostAmount;
                            item.SalesAmount = -item.SalesAmount;
                            item.ItemQty = -item.ItemQty;
                        }


                        var outboundModel = new OtherOutboundDetailModel();
                        outboundModel.SerialNumber = i.ToString();
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.MedicalItemName = item.MedicalItemName + name;
                        outboundModel.Specifications = item.IsSplited ? item.Specifications : item.Packaging;
                        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                        outboundModel.Manufacturer = item.Manufacturer;
                        outboundModel.BatchNo = item.BatchNo;
                        outboundModel.QualityDate = item.QualityDate;
                        outboundModel.SalePrice = item.SalePrice.HasValue ? item.SalePrice : 0;
                        outboundModel.InPrice = item.InPrice.HasValue ? item.InPrice : 0;
                        outboundModel.ItemQty = item.ItemQty;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.DifferenceAmount = item.SalesAmount - item.CostAmount;
                        outboundModel.MedicalItemCode = item.MedicalItemCode;
                        outboundModel.MedicalItemWorkCode = item.MedicalItemWorkCode;
                        outboundModel.OutboundType = item.OutboundType == "b8e6cbd359fb4816baf9e59206d4a5ef" ? "其它出库" : "冲销其它出库";
                        result_list.Add(outboundModel);

                        totalCostAmount += outboundModel.CostAmount;
                        totalSalesAmount += outboundModel.SalesAmount;
                        totalCount += outboundModel.ItemQty;
                        totalDifferenceAmount += outboundModel.DifferenceAmount;

                    }
                    result_list.Add(new OtherOutboundDetailModel()
                    {
                        SerialNumber = "合计",
                        ItemQty = totalCount,
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount
                    });
                }
                //  return new PageData<OtherOutboundDetailModel[]>(result_list.ToArray(), count);
                return result_list.ToArray();
            });
        }


        /// <summary>
        ///领用出库统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<AccuratelyRecipientsModel[]> GetAccuratelyRecipientsByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and  mir.MedicalItemType in ({model.MedicalItemType})";
                }
                if (!string.IsNullOrWhiteSpace(model.OrderNo))
                {
                    sqlWhere += $" AND outbound.OutboundNo like '%{model.OrderNo}%'";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAccuratelyRecipientsByDate", "时间段查询领用出库信息");
                string sql = GetQuerySql(xmlSqlParameter);


                var isAll = false;
                var TupleList = new List<AccuratelyRecipientsModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += $" AND center.Id=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<AccuratelyRecipientsModel>(dt);
                        if (tuple.Item1)
                        {
                            TupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<AccuratelyRecipientsModel>(dt);
                    if (tuple.Item1)
                    {
                        TupleList = tuple.Item2;
                    }
                }

                var result_list = new List<AccuratelyRecipientsModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                int i = 0;
                if (TupleList.Count > 0)
                {
                    var list = new List<AccuratelyRecipientsModel>();
                    foreach (var item in TupleList)
                    {
                        item.CostAmount = item.InPrice * item.OutInBoundQty;
                        item.SalesAmount = item.SalePrice * item.OutInBoundQty;
                        if (item.OutboundType == "adb5fdd90a064b25ac080cb3d8d93989")//冲销
                        {
                            item.CostAmount = -item.CostAmount;
                            item.SalesAmount = -item.SalesAmount;
                        }

                        var outboundModel = new AccuratelyRecipientsModel();
                        outboundModel.CenterId = item.CenterId;
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.Id = item.Id;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.DifferenceAmount = item.SalesAmount - item.CostAmount;
                        outboundModel.Name = item.Name;
                        outboundModel.FounderDate = item.FounderDate;
                        outboundModel.AuditorName = item.AuditorName;
                        outboundModel.AuditDate = item.AuditDate;
                        outboundModel.Remark = item.Remark;
                        outboundModel.OutboundType = item.OutboundType == "0c0c27faec0e43d8aa9824925a340b81" ? "领用出库" : "冲销领用出库";
                        list.Add(outboundModel);
                    }
                    var OutboundTuple = list.GroupBy(a => new { a.CenterId, a.OutboundNo });
                    foreach (var item in OutboundTuple)
                    {
                        i++;
                        var CostAmount = item.ToList().Sum(a => a.CostAmount);
                        var SalesAmount = item.ToList().Sum(a => a.SalesAmount);
                        var DifferenceAmount = SalesAmount - CostAmount;
                        var _data = item.First();

                        _data.SerialNumber = i.ToString();
                        _data.CostAmount = CostAmount;
                        _data.SalesAmount = SalesAmount;
                        _data.DifferenceAmount = DifferenceAmount;

                        totalCostAmount += _data.CostAmount;
                        totalSalesAmount += _data.SalesAmount;
                        totalDifferenceAmount += DifferenceAmount;
                        result_list.Add(_data);
                    }
                    result_list.Add(new AccuratelyRecipientsModel()
                    {
                        SerialNumber = "合计",
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount
                    });
                }
                return result_list.ToArray();

            });
        }

        /// <summary>
        ///领用出库明细统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<AccuratelyRecipientsDetailModel[]> GetAccuratelyRecipientsDetailByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and  mir.MedicalItemType in ({model.MedicalItemType})";
                }
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlWhere += $" AND Outbound.Id='{model.Id}'";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAccuratelyRecipientsDetailByDate", "时间段查询领用出库明细信息");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<AccuratelyRecipientsDetailModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += $" AND center.Id=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<AccuratelyRecipientsDetailModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<AccuratelyRecipientsDetailModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList = tuple.Item2;
                    }
                }

                var result_list = new List<AccuratelyRecipientsDetailModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                decimal? totalOutInBoundQty = 0;//实际数量
                decimal? totalMaterialQuantity = 0;//填写数量
                int i = 0;
                if (tupleList.Count > 0)
                {
                    tupleList = tupleList.OrderBy(a => a.DialysisName).ToList();
                    foreach (var item in tupleList)
                    {
                        item.CostAmount = item.InPrice * item.OutInBoundQty;
                        item.SalesAmount = item.SalePrice * item.OutInBoundQty;
                        if (item.MedicalItemType == 1)
                        {
                            var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        else
                        {
                            var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        if (item.OutboundType == "adb5fdd90a064b25ac080cb3d8d93989")//冲销
                        {
                            item.CostAmount = -item.CostAmount;
                            item.SalesAmount = -item.SalesAmount;
                            item.OutInBoundQty = -item.OutInBoundQty;
                        }

                        i++;
                        var outboundModel = new AccuratelyRecipientsDetailModel();
                        outboundModel.SerialNumber = i.ToString();
                        if (item.MedicalItemType == 1)
                            outboundModel.ApprovalNum = item.ApprovalNum + "" == "" ? "" : item.ApprovalNum.Contains("H") ? "西药" : item.ApprovalNum.Contains("Z") ? "中成药" : item.ApprovalNum.Contains("J") ? "西药" : item.ApprovalNum.Contains("S") ? "西药" : "";
                        else
                            outboundModel.ApprovalNum = item.CatalogueName + "" == "" ? "" : item.CatalogueName;
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.MedicalItemName = item.MedicalItemName;
                        outboundModel.Specifications = item.Packaging;
                        outboundModel.Manufacturer = item.Manufacturer;
                        outboundModel.BatchNo = item.BatchNo;
                        outboundModel.QualityDate = item.QualityDate;
                        outboundModel.MaterialQuantity = item.MaterialQuantity;
                        outboundModel.OutInBoundQty = item.OutInBoundQty;
                        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                        outboundModel.InPrice = item.InPrice;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalePrice = item.SalePrice;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.DifferenceAmount = item.SalesAmount - item.CostAmount;
                        outboundModel.MedicalItemCode = item.MedicalItemCode;
                        outboundModel.MedicalItemWorkCode = item.MedicalItemWorkCode;
                        outboundModel.Name = item.Name;
                        outboundModel.FounderDate = item.FounderDate;
                        outboundModel.AuditorName = item.AuditorName;
                        outboundModel.AuditDate = item.AuditDate;
                        outboundModel.OutboundType = item.OutboundType == "0c0c27faec0e43d8aa9824925a340b81" ? "领用出库" : "冲销领用出库";
                        outboundModel.SupplierName = item.SupplierName;
                        result_list.Add(outboundModel);

                        totalCostAmount += outboundModel.CostAmount;
                        totalSalesAmount += outboundModel.SalesAmount;
                        totalDifferenceAmount += outboundModel.DifferenceAmount;
                        totalOutInBoundQty += outboundModel.OutInBoundQty;
                        totalMaterialQuantity += outboundModel.MaterialQuantity;
                    }
                    result_list.Add(new AccuratelyRecipientsDetailModel()
                    {
                        SerialNumber = "合计",
                        MaterialQuantity = totalMaterialQuantity,
                        OutInBoundQty = totalOutInBoundQty,
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount
                    });
                }
                return result_list.ToArray();
            });
        }


        /// <summary>
        ///划价出库统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<AccuratelyOutboundModel[]> GetAccuratelyOutboundByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                string sqlStr = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and  mir.MedicalItemType in ({model.MedicalItemType})";
                    sqlStr = sqlWhere;
                }
                if (!string.IsNullOrWhiteSpace(model.OrderNo))
                {
                    sqlWhere += $" AND Outbound.OutboundNo='{model.OrderNo}'";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAccuratelyOutboundByDate", "时间段查询划价出库信息");
                string sql = GetQuerySql(xmlSqlParameter);
                //查询处方明细信息
                var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                string priceSql = GetQuerySql(xmlSqlParameter2);
                priceSql += $"  AND presDetail.FounderDate>='{model.BeginTime.Value.AddMonths(-1).Date}' AND presDetail.FounderDate<='{model.EndTime.Value.AddMonths(2).Date}'" + sqlStr;

                //查询退费明细信息 
                var xmlSqlParameter3 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsRefundByDate", "查询退费明细信息");
                string refundSql = GetQuerySql(xmlSqlParameter3);


                var isAll = false;
                var tupleList = new List<AccuratelyOutboundSqlModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                var preList = new List<PrescriptionDetailModel>();
                var refundList = new List<PrescriptionDetailModel>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += $" AND center.Id=@CenterId";
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<AccuratelyOutboundSqlModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }

                        //查询处方单明细 
                        var sqlParameterList1 = new List<SqlParameter>();
                        string where = "and presDetail.CenterId=@CenterId";
                        sqlParameterList1.Add(new SqlParameter("@CenterId", center.Id));
                        var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + where, sqlParameterList1.ToArray());
                        var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                        if (presdetailTuple.Item1)
                        {
                            preList.AddRange(presdetailTuple.Item2);
                        }

                        var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + where, sqlParameterList1.ToArray());
                        var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                        if (refundTuple.Item1)
                        {
                            refundList.AddRange(refundTuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime).Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<AccuratelyOutboundSqlModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList = tuple.Item2;
                    }
                    //查询处方单明细 
                    var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql);
                    var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                    if (presdetailTuple.Item1)
                    {
                        preList.AddRange(presdetailTuple.Item2);
                    }

                    var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql);
                    var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                    if (refundTuple.Item1)
                    {
                        refundList.AddRange(refundTuple.Item2);
                    }
                }


                var result_list = new List<AccuratelyOutboundModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                int i = 0;
                var keyDic = new Dictionary<string, List<string>>();
                var priceDic = new Dictionary<string, decimal>();
                if (tupleList.Count > 0)
                {
                    var list = new List<AccuratelyOutboundModel>();
                    foreach (var item in tupleList)
                    {

                        if (keyDic.ContainsKey(item.OutboundNo + "," + item.CenterId)) { keyDic[item.OutboundNo + "," + item.CenterId].Add(item.OutInBoundDetailId); }
                        else { keyDic.Add(item.OutboundNo + "," + item.CenterId, new List<string>() { item.OutInBoundDetailId }); }

                        item.CostAmount = item.InPrice * item.OutInBoundQty;
                        item.SalesAmount = item.SalePrice * item.OutInBoundQty;

                        var outboundModel = new AccuratelyOutboundModel();

                        if (item.OutInBoundType == "608399fe9db64480828b3f20c6815fd1")
                        {
                            item.OutboundType = item.MedicalItemType == 1 ? "药品退费退库" : "耗材退费退库";
                        }
                        else
                        {
                            item.OutboundType = item.MedicalItemType == 1 ? "药品处方出库" : "耗材划价出库";
                        }
                        outboundModel.Id = item.Id;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.OutboundType = item.OutboundType;
                        outboundModel.OutboundDate = item.OutboundDate.ToString();
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.CenterId = item.CenterId;
                        list.Add(outboundModel);
                    }

                    if (keyDic.Count > 0)
                    {
                        foreach (var item in keyDic.Keys)
                        {
                            if (keyDic[item].Count > 0)
                            {
                                decimal ReceivablePrice = 0;
                                decimal refundPrice = 0;
                                if (!item.Contains("TFTK"))
                                {
                                    if (preList.Count > 0)
                                    {
                                        ReceivablePrice = preList.FindAll(a => keyDic[item].Contains(a.OutboundId)).Sum(a => a.TotalQty * a.SalePrice).Value;
                                    }
                                    priceDic.Add(item, ReceivablePrice);
                                }
                                if (item.Contains("TFTK") && refundList.Count > 0)
                                {
                                    refundPrice = refundList.FindAll(a => keyDic[item].Contains(a.ReturnWarehouseId)).Sum(a => a.TotalQty * a.SalePrice).Value;
                                    priceDic.Add(item, refundPrice);
                                }
                            }
                        }
                    }
                    var OutboundTuple = list.GroupBy(a => new { a.CenterId, a.OutboundNo });
                    foreach (var item in OutboundTuple)
                    {
                        i++;
                        var CostAmount = item.ToList().Sum(a => a.CostAmount);
                        var SalesAmount = item.ToList().Sum(a => a.SalesAmount);
                        var _data = item.First();
                        _data.SerialNumber = i.ToString();
                        _data.CostAmount = CostAmount;

                        _data.SalesAmount = _data.OutboundNo.Contains("TFTK") ? -priceDic[_data.OutboundNo + "," + _data.CenterId] : priceDic[_data.OutboundNo + "," + _data.CenterId];
                        _data.CostAmount = _data.OutboundNo.Contains("TFTK") ? -CostAmount : CostAmount;
                        var RefundCostAmount = _data.OutboundNo.Contains("TFTK") ? -CostAmount : CostAmount;

                        totalCostAmount += RefundCostAmount;
                        totalSalesAmount += _data.SalesAmount;
                        result_list.Add(_data);
                    }

                    result_list.Add(new AccuratelyOutboundModel()
                    {
                        SerialNumber = "合计",
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount
                    });
                }
                return result_list.ToArray();
            });
        }

        /// <summary>
        ///划价出库明细统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<PageData<AccuratelyOutboundDetailModel[]>> GetAccuratelyOutboundDetailByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                try
                {


                    string sqlWhere = string.Empty;
                    string sqlStr = string.Empty;
                    if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                    {
                        sqlWhere += $" and MedicalItemType in ({model.MedicalItemType})";
                        sqlStr = sqlWhere;
                    }
                    if (model.BeginTime.HasValue)
                    {
                        sqlWhere += $" and OutboundDate >='{model.BeginTime.Value.Date}' ";
                    }
                    if (model.EndTime.HasValue)
                    {
                        sqlWhere += $" AND OutboundDate  <= '{model.EndTime}'";
                    }
                    var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAccuratelyOutboundDetailByDate", "时间段查询划价出库明细信息");
                    string sql = GetQuerySql(xmlSqlParameter);

                    var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlGetBatchNoSecondaryStoreroom", "查询物品价格");
                    string priceSql = GetQuerySql(xmlSqlParameter1);
                    priceSql += $" AND badetail.FounderDate >= '{model.BeginTime}' and badetail.FounderDate <= '{model.EndTime.Value.AddMonths(2).Date}' " + sqlStr;

                    var isAll = false;
                    var tupleList = new List<AccuratelyOutboundDetailModel>();
                    var datas = new List<SI_CFMX>();
                    var CenterDialysiss = new List<CenterDialysis>();
                    var priceList = new List<PriceModel>();
                    if (model.CenterId != null)
                    {
                        //查询全部
                        if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                        {
                            isAll = true;
                        }
                        else
                        {
                            CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                            isAll = false;
                        }
                    }
                    if (isAll == false && CenterDialysiss.Count > 0)
                    {
                        sqlWhere += " AND CenterId  = @CenterId";
                        sql = sql.Replace("@SqlWhere", sqlWhere);
                        var priceWhere = " AND badetail.CenterId  = @CenterId";
                        priceSql = priceSql + priceWhere;
                        foreach (var center in CenterDialysiss)
                        {
                            var sqlParameterList = new List<SqlParameter>();
                            sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                            var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                            var tuple = GetTupleByList<AccuratelyOutboundDetailModel>(dt);
                            if (tuple.Item1)
                            {
                                tupleList.AddRange(tuple.Item2);
                            }
                            var priceTuple = GetTupleByList<PriceModel>(MsSqlHelper.GetSingleObj().GetDataTable(priceSql, sqlParameterList.ToArray()));
                            if (priceTuple.Item1)
                            {
                                priceList.AddRange(priceTuple.Item2);
                            }
                            var dataList = cfmxStore.Entities.Where(a => a.CenterId == center.Id && a.JSRQ > model.BeginTime.Value.AddMonths(-1) && a.JSRQ < model.EndTime.Value.AddMonths(1)).ToList();
                            datas.AddRange(dataList);
                        }
                    }
                    else
                    {
                        sql = sql.Replace("@SqlWhere", sqlWhere);
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                        var tuple = GetTupleByList<AccuratelyOutboundDetailModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                        var priceTuple = GetTupleByList<PriceModel>(MsSqlHelper.GetSingleObj().GetDataTable(priceSql));
                        if (priceTuple.Item1)
                        {
                            priceList.AddRange(priceTuple.Item2);
                        }
                        var dataList = cfmxStore.Entities.Where(a => a.JSRQ > model.BeginTime.Value.AddMonths(-1) && a.JSRQ < model.EndTime.Value.AddMonths(1)).ToList();
                        datas.AddRange(dataList);
                    }


                    var result_list = new List<AccuratelyOutboundDetailModel>();
                    decimal? totalCostAmount = 0;
                    decimal? totalSalesAmount = 0;
                    decimal? totalCount = 0;
                    int i = 0;

                    if (tupleList.Count > 0)
                    {

                        var result = tupleList.GroupBy(a => new { a.Id, a.MaterialId, a.CenterId });
                        foreach (var items in result)
                        {

                            decimal? qty = 0;
                            decimal? outInBoundQty = 0;
                            var data = priceList.Where(a => a.OutInBoundDetailId == items.Key.Id && a.MedicalItemId == items.Key.MaterialId && a.CenterId == items.Key.CenterId).OrderBy(a => a.Id).ToList();//获取成本金额  
                            int count = data.Count();
                            foreach (var item in items)
                            {

                                decimal inprice = 0;
                                string supplierName = string.Empty;
                                string batchNo = string.Empty;
                                string productionDateStr = string.Empty;
                                string qualityDateStr = string.Empty;
                                qty += item.MaterialTotal;

                                var sl = datas.FindAll(a => a.CFH == item.PrescriptionNo && a.YYNM == item.MedicalItemCode && a.CenterId == item.CenterId).Sum(a => a.SL);

                                outInBoundQty = data[0].OutInBoundQty;
                                if (count > 1 && data.Sum(a => a.OutInBoundQty) == item.MaterialTotal)
                                {
                                    inprice = data[0].InPrice.Value;
                                    productionDateStr = data[0].ProductionDateStr;
                                    batchNo = data[0].BatchNo;
                                    supplierName = data[0].SupplierName;
                                    qualityDateStr = data[0].QualityDateStr;
                                    item.CostAmount = data.Sum(a => a.OutInBoundQty * a.InPrice);
                                }
                                else
                                {

                                    if (qty <= data[0].OutInBoundQty)
                                    {
                                        inprice = data[0].InPrice.Value;
                                        productionDateStr = data[0].ProductionDateStr;
                                        batchNo = data[0].BatchNo;
                                        supplierName = data[0].SupplierName;
                                        qualityDateStr = data[0].QualityDateStr;

                                    }
                                    else if (qty <= (data[0].OutInBoundQty + data[1].OutInBoundQty))
                                    {
                                        inprice = data[1].InPrice.Value;
                                        productionDateStr = data[1].ProductionDateStr;
                                        batchNo = data[1].BatchNo;
                                        supplierName = data[1].SupplierName;
                                        qualityDateStr = data[1].QualityDateStr;

                                    }
                                    else
                                    {
                                        inprice = data[2].InPrice.Value;
                                        productionDateStr = data[2].ProductionDateStr;
                                        batchNo = data[2].BatchNo;
                                        supplierName = data[2].SupplierName;
                                        qualityDateStr = data[2].QualityDateStr;

                                    }
                                    item.CostAmount = item.MaterialTotal * inprice;
                                }
                                item.SalesAmount = item.MaterialTotal * item.SalePrices;
                                if (item.OutboundType == "608399fe9db64480828b3f20c6815fd1")
                                {
                                    item.CostAmount = -(item.MaterialTotal * inprice);
                                    item.SalesAmount = -(item.MaterialTotal * item.SalePrices);
                                    item.MaterialTotal = -item.MaterialTotal;
                                    item.OutboundType = item.MedicalItemType == 1 ? "药品退费退库" : "耗材退费退库";
                                }
                                else
                                {
                                    item.Ybscl = item.SalesAmount > sl ? item.SalesAmount : sl;
                                    item.OutboundType = item.MedicalItemType == 1 ? "药品处方出库" : "耗材划价出库";
                                }
                                if (item.MedicalItemType == 1)
                                {
                                    var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                                    item.MedicalItemName = item.MedicalItemName + name;
                                }
                                else
                                {
                                    var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                                    item.MedicalItemName = item.MedicalItemName + name;
                                }

                                i++;
                                var outboundModel = new AccuratelyOutboundDetailModel();
                                outboundModel.Ybscl = item.Ybscl;
                                outboundModel.PrescriptionNo = item.PrescriptionNo;
                                outboundModel.SerialNumber = i.ToString();
                                outboundModel.PatientName = item.PatientName;
                                outboundModel.OutboundNo = item.OutboundNo;
                                outboundModel.MedicalItemCode = item.MedicalItemCode;
                                outboundModel.MedicalItemName = item.MedicalItemName;
                                outboundModel.Specifications = item.Specifications;
                                outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                                outboundModel.Manufacturer = item.Manufacturer;
                                outboundModel.SupplierName = supplierName;
                                outboundModel.BatchNo = batchNo;
                                outboundModel.ProductionDate = productionDateStr;
                                outboundModel.QualityDate = qualityDateStr;
                                outboundModel.SalePrice = item.SalePrices;
                                outboundModel.UnitPrice = inprice;
                                outboundModel.OutInBoundQty = item.MaterialTotal;
                                outboundModel.CostAmount = item.CostAmount;
                                outboundModel.SalesAmount = item.SalesAmount;
                                outboundModel.OutboundType = item.OutboundType;
                                outboundModel.OutboundDate = item.OutboundDate;
                                outboundModel.DialysisName = item.DialysisName;
                                if (item.MedicalItemType == 1)
                                    outboundModel.ApprovalNum = item.ApprovalNum + "" == "" ? "" : item.ApprovalNum.Contains("H") ? "西药" : item.ApprovalNum.Contains("Z") ? "中成药" : item.ApprovalNum.Contains("J") ? "西药" : item.ApprovalNum.Contains("S") ? "西药" : "";
                                else
                                    outboundModel.ApprovalNum = item.CatalogueName + "" == "" ? "" : item.CatalogueName;
                                result_list.Add(outboundModel);

                                totalCostAmount += outboundModel.CostAmount;
                                totalSalesAmount += outboundModel.SalesAmount;

                                totalCount += item.MaterialTotal;

                            }

                        }

                    }
                    result_list.Add(new AccuratelyOutboundDetailModel()
                    {
                        SerialNumber = "合计",
                        OutInBoundQty = totalCount,
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount
                    });
                    //if (tupleList.Count > 0)
                    //{
                    //    totalCostAmount = tupleList.Where(t => t.OutboundType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.MaterialTotal.Value * priceList.FindAll(a => a.OutInBoundDetailId == t.Id).First().InPrice) - tupleList.Where(t => t.OutboundType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.MaterialTotal.Value * priceList.FindAll(a => a.OutInBoundDetailId == t.Id).First().InPrice);

                    //    totalSalesAmount = tupleList.Where(t => t.OutboundType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.SalePrices * t.MaterialTotal) - tupleList.Where(t => t.OutboundType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.MaterialTotal * t.SalePrices);
                    //    totalCount = tupleList.Sum(a => a.MaterialTotal);

                    //    count = tupleList.Count;
                    //    int size = count % model.PageSize.Value == 0 ? count / model.PageSize.Value : (count / model.PageSize.Value) + 1;
                    //    i = model.PageSize.Value * (model.PageIndex.Value - 1);

                    //    var pageData = tupleList.Skip((model.PageIndex.Value - 1) * model.PageSize.Value).Take(model.PageSize.Value);

                    //    var datas = await cfmxStore.Entities.ToListAsync();
                    //    foreach (var item in pageData)
                    //    {
                    //        var sl = datas.FindAll(a => a.CFH == item.PrescriptionNo && a.YYNM == item.MedicalItemCode && a.CenterId == item.CenterId).Sum(a => a.SL);
                    //        var data = priceList.FindAll(a => a.OutInBoundDetailId == item.Id && a.CenterId == item.CenterId).First();
                    //        decimal inprice = data.InPrice.Value;

                    //        item.CostAmount = item.MaterialTotal * inprice;
                    //        item.SalesAmount = item.SalePrices * item.MaterialTotal;
                    //        if (item.OutboundType == "608399fe9db64480828b3f20c6815fd1")
                    //        {
                    //            item.CostAmount = -(item.MaterialTotal * inprice);
                    //            item.SalesAmount = -(item.SalePrices * item.MaterialTotal);
                    //            item.MaterialTotal = -item.MaterialTotal;
                    //            item.OutboundType = item.MedicalItemType == 1 ? "药品退费退库" : "耗材退费退库";
                    //        }
                    //        else
                    //        {
                    //            item.Ybscl = sl;
                    //            item.OutboundType = item.MedicalItemType == 1 ? "药品处方出库" : "耗材划价出库";
                    //
                    //}
                    //        if (item.MedicalItemType == 1)
                    //        {
                    //            var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                    //            item.MedicalItemName = item.MedicalItemName + name;
                    //        }
                    //        else
                    //        {
                    //            var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                    //            item.MedicalItemName = item.MedicalItemName + name;
                    //        }

                    //        i++;
                    //        var outboundModel = new AccuratelyOutboundDetailModel();
                    //        outboundModel.Ybscl = item.Ybscl;
                    //        outboundModel.PrescriptionNo = item.PrescriptionNo;
                    //        outboundModel.SerialNumber = i.ToString();
                    //        outboundModel.PatientName = item.PatientName;
                    //        outboundModel.OutboundNo = item.OutboundNo;
                    //        outboundModel.MedicalItemCode = item.MedicalItemCode;
                    //        outboundModel.MedicalItemName = item.MedicalItemName;
                    //        outboundModel.Specifications = item.Packaging;
                    //        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                    //        outboundModel.Manufacturer = item.Manufacturer;
                    //        outboundModel.SupplierName = data.SupplierName;
                    //        outboundModel.BatchNo = data.BatchNo;
                    //        outboundModel.ProductionDate = data.ProductionDate;
                    //        outboundModel.QualityDate = data.QualityDate;
                    //        outboundModel.SalePrice = data.SalePrice;
                    //        outboundModel.UnitPrice = data.InPrice;
                    //        outboundModel.OutInBoundQty = item.MaterialTotal;
                    //        outboundModel.CostAmount = item.CostAmount;
                    //        outboundModel.SalesAmount = item.SalesAmount;
                    //        outboundModel.OutboundType = item.OutboundType;
                    //        outboundModel.OutboundDate = item.OutboundDate;
                    //        outboundModel.DialysisName = item.DialysisName;
                    //        result_list.Add(outboundModel);
                    //    }

                    //    if (size == model.PageIndex)
                    //    {
                    //        result_list.Add(new AccuratelyOutboundDetailModel()
                    //        {
                    //            SerialNumber = "合计",
                    //            OutInBoundQty = totalCount,
                    //            CostAmount = totalCostAmount,
                    //            SalesAmount = totalSalesAmount
                    //        });
                    //    }
                    //}

                    // result_list = result_list.OrderBy(a => new { a.DialysisName, a.OutboundNo }).ToList();
                    return new PageData<AccuratelyOutboundDetailModel[]>(result_list.ToArray(), result_list.Count);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex);
                }
            });
        }



        /// <summary>
        ///根据id查询划价出库明细 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<PageData<AccuratelyOutboundDetailModel[]>> GetAccuratelyOutboundDetailByDateAndIdAsync(MaterialsConfluenceInPut model)
        {
            return Task.Run(async () =>
            {
                //int count = 0;
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlWhere += " AND newtable.MaterialOutboundId=@Id  ";
                    sqlParameterList.Add(new SqlParameter("@Id", model.Id));
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetAccuratelyOutboundDetailByDate", "时间段查询划价出库明细信息");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = sql.Replace("@SqlWhere", sqlWhere);
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<AccuratelyOutboundDetailModel>(dt);

                var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlGetBatchNoSecondaryStoreroom", "查询物品价格");
                string priceSql = GetQuerySql(xmlSqlParameter1);
                priceSql += $" AND badetail.FounderDate >= '{model.BeginTime}' and badetail.FounderDate <= '{model.EndTime.Value.AddMonths(2).Date}' ";
                priceSql = string.Format(priceSql, model.CenterId);
                var priceTuple = GetTupleByList<PriceModel>(MsSqlHelper.GetSingleObj().GetDataTable(priceSql));

                var datas = cfmxStore.Entities.Where(a => a.JSRQ > model.BeginTime.Value.AddMonths(-1) && a.JSRQ < model.EndTime.Value.AddMonths(1)).ToList();

                var result_list = new List<AccuratelyOutboundDetailModel>();
                int i = 0;
                if (tuple.Item1)
                {
                    var result = tuple.Item2.GroupBy(a => new { a.Id, a.MaterialId, a.CenterId });
                    foreach (var items in result)
                    {

                        decimal? qty = 0;
                        decimal? outInBoundQty = 0;
                        var data = priceTuple.Item2.Where(a => a.OutInBoundDetailId == items.Key.Id && a.MedicalItemId == items.Key.MaterialId && a.CenterId == items.Key.CenterId).OrderBy(a => a.Id).ToList();//获取成本金额 
                        int count = data.Count();
                        foreach (var item in items)
                        {
                            decimal inprice = 0;
                            string supplierName = string.Empty;
                            string batchNo = string.Empty;
                            string productionDateStr = string.Empty;
                            string qualityDateStr = string.Empty;
                            qty += item.MaterialTotal;

                            var sl = datas.FindAll(a => a.CFH == item.PrescriptionNo && a.YYNM == item.MedicalItemCode && a.CenterId == item.CenterId).Sum(a => a.SL);

                            outInBoundQty = data[0].OutInBoundQty;
                            if (count > 1 && data.Sum(a => a.OutInBoundQty) == item.MaterialTotal)
                            {
                                inprice = data[0].InPrice.Value;
                                productionDateStr = data[0].ProductionDateStr;
                                batchNo = data[0].BatchNo;
                                supplierName = data[0].SupplierName;
                                qualityDateStr = data[0].QualityDateStr;
                                item.CostAmount = data.Sum(a => a.OutInBoundQty * a.InPrice);
                            }
                            else
                            {

                                if (qty <= data[0].OutInBoundQty)
                                {
                                    inprice = data[0].InPrice.Value;
                                    productionDateStr = data[0].ProductionDateStr;
                                    batchNo = data[0].BatchNo;
                                    supplierName = data[0].SupplierName;
                                    qualityDateStr = data[0].QualityDateStr;

                                }
                                else if (qty <= (data[0].OutInBoundQty + data[1].OutInBoundQty))
                                {
                                    inprice = data[1].InPrice.Value;
                                    productionDateStr = data[1].ProductionDateStr;
                                    batchNo = data[1].BatchNo;
                                    supplierName = data[1].SupplierName;
                                    qualityDateStr = data[1].QualityDateStr;

                                }
                                else
                                {
                                    inprice = data[2].InPrice.Value;
                                    productionDateStr = data[2].ProductionDateStr;
                                    batchNo = data[2].BatchNo;
                                    supplierName = data[2].SupplierName;
                                    qualityDateStr = data[2].QualityDateStr;

                                }
                                item.CostAmount = item.MaterialTotal * inprice;
                            }
                            item.SalesAmount = item.MaterialTotal * item.SalePrices;
                            if (item.OutboundType == "608399fe9db64480828b3f20c6815fd1")
                            {
                                item.CostAmount = -(item.MaterialTotal * inprice);
                                item.SalesAmount = -(item.MaterialTotal * item.SalePrices);
                                item.MaterialTotal = -item.MaterialTotal;
                                item.OutboundType = item.MedicalItemType == 1 ? "药品退费退库" : "耗材退费退库";
                            }
                            else
                            {
                                item.Ybscl = sl;
                                item.OutboundType = item.MedicalItemType == 1 ? "药品处方出库" : "耗材划价出库";
                            }
                            if (item.MedicalItemType == 1)
                            {
                                var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                                item.MedicalItemName = item.MedicalItemName + name;
                            }
                            else
                            {
                                var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                                item.MedicalItemName = item.MedicalItemName + name;
                            }

                            i++;
                            var outboundModel = new AccuratelyOutboundDetailModel();
                            outboundModel.Ybscl = item.Ybscl;
                            outboundModel.PrescriptionNo = item.PrescriptionNo;
                            outboundModel.SerialNumber = i.ToString();
                            outboundModel.PatientName = item.PatientName;
                            outboundModel.OutboundNo = item.OutboundNo;
                            outboundModel.MedicalItemCode = item.MedicalItemCode;
                            outboundModel.MedicalItemName = item.MedicalItemName;
                            outboundModel.Specifications = item.Packaging;
                            outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                            outboundModel.Manufacturer = item.Manufacturer;
                            outboundModel.SupplierName = supplierName;
                            outboundModel.BatchNo = batchNo;
                            outboundModel.ProductionDate = productionDateStr;
                            outboundModel.QualityDate = qualityDateStr;
                            outboundModel.SalePrice = item.SalePrices;
                            outboundModel.UnitPrice = inprice;
                            outboundModel.OutInBoundQty = item.MaterialTotal;
                            outboundModel.CostAmount = item.CostAmount;
                            outboundModel.SalesAmount = item.SalesAmount;
                            outboundModel.OutboundType = item.OutboundType;
                            outboundModel.OutboundDate = item.OutboundDate;
                            outboundModel.DialysisName = item.DialysisName;
                            result_list.Add(outboundModel);

                        }

                    }
                }
                //if (tuple.Item1)
                //{
                //    var datas = await cfmxStore.Entities.ToListAsync();
                //    foreach (var item in tuple.Item2)
                //    {
                //        var sl = datas.FindAll(a => a.CFH == item.PrescriptionNo && a.YYNM == item.MedicalItemCode && a.CenterId == item.CenterId).Sum(a => a.SL);
                //        var data = priceTuple.Item2.FindAll(a => a.OutInBoundDetailId == item.Id).First();
                //        decimal inprice = data.InPrice.Value;

                //        item.CostAmount = item.MaterialTotal * inprice;
                //        item.SalesAmount = item.MaterialTotal * item.SalePrices;
                //        if (item.OutboundType == "608399fe9db64480828b3f20c6815fd1")
                //        {
                //            item.CostAmount = -(item.MaterialTotal * inprice);
                //            item.SalesAmount = -(item.MaterialTotal * item.SalePrices);
                //            item.MaterialTotal = -item.MaterialTotal;
                //            item.OutboundType = item.MedicalItemType == 1 ? "药品退费退库" : "耗材退费退库";
                //        }
                //        else
                //        {
                //            item.Ybscl = sl;
                //            item.OutboundType = item.MedicalItemType == 1 ? "药品处方出库" : "耗材划价出库";
                //        }
                //        if (item.MedicalItemType == 1)
                //        {
                //            var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                //            item.MedicalItemName = item.MedicalItemName + name;
                //        }
                //        else
                //        {
                //            var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                //            item.MedicalItemName = item.MedicalItemName + name;
                //        }

                //        i++;
                //        var outboundModel = new AccuratelyOutboundDetailModel();
                //        outboundModel.Ybscl = item.Ybscl;
                //        outboundModel.DialysisName = item.DialysisName;
                //        outboundModel.PrescriptionNo = item.PrescriptionNo;
                //        outboundModel.SerialNumber = i.ToString();
                //        outboundModel.PatientName = item.PatientName;
                //        outboundModel.OutboundNo = item.OutboundNo;
                //        outboundModel.MedicalItemCode = item.MedicalItemCode;
                //        outboundModel.MedicalItemName = item.MedicalItemName;
                //        outboundModel.Specifications = item.Packaging;
                //        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                //        outboundModel.Manufacturer = item.Manufacturer;
                //        outboundModel.SupplierName = data.SupplierName;
                //        outboundModel.BatchNo = data.BatchNo;
                //        outboundModel.ProductionDate = data.ProductionDate;
                //        outboundModel.QualityDate = data.QualityDate;
                //        outboundModel.SalePrice = data.SalePrice;
                //        outboundModel.UnitPrice = data.InPrice;
                //        outboundModel.OutInBoundQty = item.MaterialTotal;
                //        outboundModel.CostAmount = item.CostAmount;
                //        outboundModel.SalesAmount = item.SalesAmount;
                //        outboundModel.OutboundType = item.OutboundType;
                //        outboundModel.OutboundDate = item.OutboundDate;
                //        result_list.Add(outboundModel);
                //    }
                //}
                return new PageData<AccuratelyOutboundDetailModel[]>(result_list.ToArray(), result_list.Count);
            });
        }

        /// <summary>
        ///退货出库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundModel[]> GetReturnOutboundByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and mir.MedicalItemType in ({model.MedicalItemType})";
                }
                if (model.BeginTime.HasValue)
                {
                    sqlWhere += $" AND returnrequest.ReturnDate >='{model.BeginTime.Value.Date}' ";
                }
                if (model.EndTime.HasValue)
                {
                    sqlWhere += $" AND returnrequest.ReturnDate <= '{model.EndTime}'";
                }
                if (!string.IsNullOrWhiteSpace(model.SupplierId) && model.SupplierId != "0")
                    sqlWhere += $" AND returnrequest.SupplierId = '{model.SupplierId}'";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetReturnStatisticsByDate", "时间段查询退货出库");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<OtherOutboundModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && model.CenterId.Contains(t.Id)).ToListAsync();//t.CenterUrl != null &&
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += " AND returnrequest.CenterId  = @CenterId";
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }//
                else
                {
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList.AddRange(tuple.Item2);
                    }
                }


                var result_list = new List<OtherOutboundModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                int i = 0;
                if (tupleList.Count > 0)
                {
                    var list = new List<OtherOutboundModel>();
                    foreach (var item in tupleList)
                    {
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;

                        var outboundModel = new OtherOutboundModel();
                        outboundModel.Id = item.Id;
                        outboundModel.Name = item.Name;
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.CenterId = item.CenterId;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.SupplierName = item.SupplierName;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.AuditorName = item.AuditorName;
                        outboundModel.DifferenceAmount = outboundModel.SalesAmount - outboundModel.CostAmount;
                        outboundModel.FounderDate = item.FounderDate.ToString();
                        outboundModel.AuditDate = item.AuditDate.ToString();
                        outboundModel.Remark = item.Remark;
                        outboundModel.IsSettlement = item.IsSettlement.HasValue ? item.IsSettlement : 0;
                        outboundModel.IsChecked = item.IsChecked.HasValue ? item.IsChecked : 0;
                        outboundModel.CheckedDate = item.CheckedDate;
                        outboundModel.SettlementDate = item.SettlementDate;
                        outboundModel.ReturnAuditor = item.ReturnAuditor;
                        outboundModel.ReturnDate = item.ReturnDate;
                        list.Add(outboundModel);
                    }
                    var OutboundTuple = list.GroupBy(a => new { a.OutboundNo, a.CenterId });
                    foreach (var item in OutboundTuple)
                    {
                        i++;

                        var CostAmount = item.ToList().Sum(a => a.CostAmount);
                        var SalesAmount = item.ToList().Sum(a => a.SalesAmount);
                        var DifferenceAmount = SalesAmount - CostAmount;
                        var _data = item.First();

                        _data.CostAmount = CostAmount;
                        _data.SalesAmount = SalesAmount;
                        _data.SerialNumber = i.ToString();
                        _data.DifferenceAmount = DifferenceAmount;

                        totalCostAmount += _data.CostAmount;
                        totalSalesAmount += _data.SalesAmount;
                        totalDifferenceAmount += DifferenceAmount;
                        result_list.Add(_data);
                    }
                    result_list.Add(new OtherOutboundModel()
                    {
                        SerialNumber = "合计",
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount,
                    });
                }
                return result_list.ToArray();
            });
        }

        /// <summary>
        /// 退货单结算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> UpdateReturnSettlementAsync(SettlemenInPuts model)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (model.Id != null)
                    {
                        var userId = await _getUserInfo.GetCurrentUserIdAsync();
                        foreach (var item in model.Id)
                        {
                            var data = await ReturnRequestStore.GetFirstOrDefaultAsync(t => t.Id == item);
                            if (data == null)
                                continue;
                            data.IsSettlement = 1;
                            data.SettlementDate = model.SettlementDate;
                            data.SettlementPeople = userId;
                            ReturnRequestStore.Update(data);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                return true;
            });
        }
        /// <summary>
        ///退货出库明细统计 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundDetailModel[]> GetReturnOutboundDetailByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and mir.MedicalItemType in ({model.MedicalItemType})";
                }
                if (model.BeginTime.HasValue)
                {
                    sqlWhere += $" AND returnrequest.ReturnDate >='{model.BeginTime.Value.Date}' ";
                }
                if (model.EndTime.HasValue)
                {
                    sqlWhere += $" AND returnrequest.ReturnDate <= '{model.EndTime}'";
                }
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlWhere += $" AND returnrequest.Id = '{model.Id}'";

                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetReturnStatisticsByDate", "时间段查询退货出库");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<OtherOutboundDetailModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && model.CenterId.Contains(t.Id)).ToListAsync();//&& t.CenterUrl != null
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += " AND returnrequest.CenterId  = @CenterId";
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList.AddRange(tuple.Item2);
                    }
                }

                var result_list = new List<OtherOutboundDetailModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                decimal? totalCount = 0;
                int i = 0;
                if (tupleList.Count > 0)
                {
                    foreach (var item in tupleList)
                    {
                        i++;
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;
                        if (item.MedicalItemType == 1)
                        {
                            var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        else if (item.MedicalItemType == 2)
                        {
                            var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        else
                        {
                            var name = string.IsNullOrWhiteSpace(item.Brand) ? "" : "(" + item.Brand + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }

                        var outboundModel = new OtherOutboundDetailModel();
                        outboundModel.SerialNumber = i.ToString();
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.ItemTypeName = item.ItemTypeName;
                        outboundModel.SupplierName = item.SupplierName;
                        outboundModel.MedicalItemName = item.MedicalItemName;
                        outboundModel.Specifications = item.IsSplited ? item.Specifications : item.Packaging;
                        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                        outboundModel.Manufacturer = item.Manufacturer;
                        outboundModel.BatchNo = item.BatchNo;
                        outboundModel.QualityDate = item.QualityDate.ToString();
                        outboundModel.SalePrice = (item.SalePrice.HasValue ? item.SalePrice : 0);
                        outboundModel.InPrice = (item.InPrice.HasValue ? item.InPrice : 0);
                        outboundModel.ItemQty = item.ItemQty;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.DifferenceAmount = item.SalesAmount - item.CostAmount;
                        outboundModel.MedicalItemCode = item.MedicalItemCode;
                        outboundModel.MedicalItemWorkCode = item.MedicalItemWorkCode;
                        result_list.Add(outboundModel);

                        totalCostAmount += outboundModel.CostAmount;
                        totalSalesAmount += outboundModel.SalesAmount;
                        totalCount += outboundModel.ItemQty;
                        totalDifferenceAmount += outboundModel.DifferenceAmount;
                    }
                    result_list.Add(new OtherOutboundDetailModel()
                    {
                        SerialNumber = "合计",
                        ItemQty = totalCount,
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount
                    });
                }
                return result_list.ToArray();
            });
        }

        /// <summary>
        ///报废出库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundModel[]> GetScrapOutboundByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and a.MedicalItemType in ({model.MedicalItemType})";
                }
                if (model.BeginTime.HasValue)
                {
                    sqlWhere += $" AND a.putInstorageDate >='{model.BeginTime.Value.Date}' ";
                }
                if (model.EndTime.HasValue)
                {
                    sqlWhere += $" AND a.putInstorageDate <= '{model.EndTime}'";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetScrapOutboundStatisticsByDate", "时间段查询报废出库");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<OtherOutboundModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += " AND a.CenterId  = @CenterId";
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList.AddRange(tuple.Item2);
                    }
                }

                var result_list = new List<OtherOutboundModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                int i = 0;
                if (tupleList.Count > 0)
                {
                    var list = new List<OtherOutboundModel>();
                    foreach (var item in tupleList)
                    {
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;

                        var outboundModel = new OtherOutboundModel();
                        outboundModel.Id = item.Id;
                        outboundModel.Name = item.Name;
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.WareHouseName = item.WareHouseName;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.AuditorName = item.AuditorName;
                        outboundModel.DifferenceAmount = outboundModel.SalesAmount - outboundModel.CostAmount;
                        outboundModel.FounderDate = item.FounderDate.ToString();
                        outboundModel.AuditDate = item.AuditDate.ToString();
                        outboundModel.Remark = item.Remark;
                        list.Add(outboundModel);
                    }
                    var OutboundTuple = list.GroupBy(a => a.OutboundNo);
                    foreach (var item in OutboundTuple)
                    {
                        i++;
                        var CostAmount = item.ToList().Sum(a => a.CostAmount);
                        var SalesAmount = item.ToList().Sum(a => a.SalesAmount);
                        var DifferenceAmount = SalesAmount - CostAmount;
                        var _data = item.First();

                        _data.CostAmount = CostAmount;
                        _data.SalesAmount = SalesAmount;
                        _data.SerialNumber = i.ToString();
                        _data.DifferenceAmount = DifferenceAmount;

                        totalCostAmount += _data.CostAmount;
                        totalSalesAmount += _data.SalesAmount;
                        totalDifferenceAmount += DifferenceAmount;
                        result_list.Add(_data);
                    }
                    result_list.Add(new OtherOutboundModel()
                    {
                        SerialNumber = "合计",
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount,
                    });
                }
                return result_list.ToArray();
            });
        }


        /// <summary>
        ///报废出库明细统计 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<OtherOutboundDetailModel[]> GetScrapOutboundDetailByDateAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and a.MedicalItemType in ({model.MedicalItemType})";
                }
                if (model.BeginTime.HasValue)
                {
                    sqlWhere += $" AND a.putInstorageDate >='{model.BeginTime.Value.Date}' ";
                }
                if (model.EndTime.HasValue)
                {
                    sqlWhere += $" AND a.putInstorageDate <= '{model.EndTime}'";
                }
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlWhere += $" AND a.Id = '{model.Id}'";

                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetScrapOutboundStatisticsByDate", "时间段查询报废出库");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<OtherOutboundDetailModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlWhere += " AND a.CenterId  = @CenterId";
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sql = sql.Replace("@SqlWhere", sqlWhere);
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql);
                    var tuple = GetTupleByList<OtherOutboundDetailModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList.AddRange(tuple.Item2);
                    }
                }

                var result_list = new List<OtherOutboundDetailModel>();
                decimal? totalCostAmount = 0;
                decimal? totalSalesAmount = 0;
                decimal? totalDifferenceAmount = 0;
                decimal? totalCount = 0;
                int i = 0;
                if (tupleList.Count > 0)
                {
                    foreach (var item in tupleList)
                    {
                        i++;
                        item.CostAmount = (item.InPrice.HasValue ? item.InPrice : 0) * item.ItemQty;
                        item.SalesAmount = (item.SalePrice.HasValue ? item.SalePrice : 0) * item.ItemQty;
                        if (item.MedicalItemType == 1)
                        {
                            var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        else if (item.MedicalItemType == 2)
                        {
                            var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }
                        else
                        {
                            var name = string.IsNullOrWhiteSpace(item.Brand) ? "" : "(" + item.Brand + ")";
                            item.MedicalItemName = item.MedicalItemName + name;
                        }

                        var outboundModel = new OtherOutboundDetailModel();
                        outboundModel.SerialNumber = i.ToString();
                        outboundModel.DialysisName = item.DialysisName;
                        outboundModel.ItemTypeName = item.ItemTypeName;
                        outboundModel.OutboundNo = item.OutboundNo;
                        outboundModel.WareHouseName = item.WareHouseName;
                        outboundModel.MedicalItemName = item.MedicalItemName;
                        outboundModel.Specifications = item.IsSplited ? item.Specifications : item.Packaging;
                        outboundModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                        outboundModel.Manufacturer = item.Manufacturer;
                        outboundModel.BatchNo = item.BatchNo;
                        outboundModel.QualityDate = item.QualityDate.ToString();
                        outboundModel.SalePrice = (item.SalePrice.HasValue ? item.SalePrice : 0);
                        outboundModel.InPrice = (item.InPrice.HasValue ? item.InPrice : 0);
                        outboundModel.ItemQty = item.ItemQty;
                        outboundModel.CostAmount = item.CostAmount;
                        outboundModel.SalesAmount = item.SalesAmount;
                        outboundModel.DifferenceAmount = item.SalesAmount - item.CostAmount;
                        outboundModel.MedicalItemCode = item.MedicalItemCode;
                        outboundModel.MedicalItemWorkCode = item.MedicalItemWorkCode;
                        outboundModel.SupplierName = item.SupplierName;
                        result_list.Add(outboundModel);

                        totalCostAmount += outboundModel.CostAmount;
                        totalSalesAmount += outboundModel.SalesAmount;
                        totalCount += outboundModel.ItemQty;
                        totalDifferenceAmount += outboundModel.DifferenceAmount;
                    }
                    result_list.Add(new OtherOutboundDetailModel()
                    {
                        SerialNumber = "合计",
                        ItemQty = totalCount,
                        CostAmount = totalCostAmount,
                        SalesAmount = totalSalesAmount,
                        DifferenceAmount = totalDifferenceAmount
                    });
                }
                return result_list.ToArray();
            });
        }
        /// <summary>
        ///近效期物品查询
        /// </summary>
        /// <returns></returns>
        public Task<RecentValiditymModel[]> GetRecentValidityStatisticsAsync(MaterialsRequst model)
        {
            return Task.Run(async () =>
            {
                string sqlWhere = string.Empty;
                if (model.MedicalItemType != "0" && !string.IsNullOrWhiteSpace(model.MedicalItemType))
                {
                    sqlWhere += $" and MedicalItemType in ({model.MedicalItemType})";
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetRecentValidityStatistics", "近效期统计");
                string sql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var tupleList = new List<RecentValiditymModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sql = sql + " AND a.CenterId=@CenterId  ORDER BY a.DialysisName,a.Id desc ";
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                        var tuple = GetTupleByList<RecentValiditymModel>(dt);
                        if (tuple.Item1)
                        {
                            tupleList.AddRange(tuple.Item2);
                        }
                    }
                }
                else
                {
                    sqlWhere += "  ORDER BY a.DialysisName,a.Id desc";
                    var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlWhere);
                    var tuple = GetTupleByList<RecentValiditymModel>(dt);
                    if (tuple.Item1)
                    {
                        tupleList.AddRange(tuple.Item2);
                    }
                }

                var now = DateTime.Now;
                var result_List = new List<RecentValiditymModel>();
                if (tupleList.Count > 0)
                {
                    var data = tupleList.GroupBy(a => new { a.CenterId, a.Id, a.BatchNo, a.InPrice, a.SalePrice, a.SupplierId, a.QualityDate });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        var _data = item.First();
                        _data.InQty = sumQty;
                        result_List.Add(_data);
                    }
                    if (model.Month.HasValue)
                    {
                        result_List = result_List.FindAll(a => a.QualityDate.Value.AddMonths(-model.Month.Value).Date <= now);
                    }
                    else
                    {
                        result_List = result_List.FindAll(a => a.QualityDate.Value.AddMonths(-6).Date <= now);
                    }
                    int i = 0;
                    if (result_List.Count > 0)
                    {
                        result_List = result_List.OrderBy(a => a.DialysisName).ToList();
                        foreach (var item in result_List)
                        {
                            i++;
                            item.SerialNumber = i.ToString();
                            item.CostAmount = item.InPrice * item.InQty;
                            item.SalesAmount = item.SalePrice * item.InQty;
                            if (item.MedicalItemType == 1)
                            {
                                var name = string.IsNullOrWhiteSpace(item.GoodsName) ? "" : "(" + item.GoodsName + ")";
                                item.MedicalItemName = item.MedicalItemName + name;
                            }
                            else
                            {
                                var name = string.IsNullOrWhiteSpace(item.Specifications) ? "" : "(" + item.Specifications + ")";
                                item.MedicalItemName = item.MedicalItemName + name;
                            }
                            item.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                        }
                    }
                }
                return result_List.ToArray();
            });
        }
        /// <summary>
        ///指定物品入出库台账查询
        /// </summary>
        /// <returns></returns>
        public Task<ItemStandingBookModel[]> GetItemStandingBookByIdAsync(MaterialsConfluenceInPuts model)
        {
            return Task.Run(async () =>
            {

                var list = new List<ItemStandingBookModel>();
                var xmlSqlParameter1 = GetXmlSqlParameter(_ClassName, "SqlQueryArticleDistributionByDate", "时间段查询物品进销存数据");
                string sql = GetQuerySql(xmlSqlParameter1);

                string sqlwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    sqlwhere = $" and  MedicalItemId='{model.Id}'";
                }

                //查询退费明细信息 
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsRefundByDate", "查询退费明细信息");
                string refundSql = GetQuerySql(xmlSqlParameter);

                var isAll = false;
                var CenterDialysiss = new List<CenterDialysis>();
                var inventory_list = new List<ItemsTalesAndInventorySummarySqlModel>();
                var preList = new List<PrescriptionDetailModel>();
                var refund_list = new List<PrescriptionDetailModel>();
                if (model.CenterId != null)
                {
                    //查询全部
                    if (model.CenterId.Count == 1 && model.CenterId[0] == "0")
                    {
                        isAll = true;
                    }
                    else
                    {
                        CenterDialysiss = await CenterDialysisStore.Entities.Where(t => t.IsDelete == false && t.CenterUrl != null && model.CenterId.Contains(t.Id)).ToListAsync();
                        isAll = false;
                    }
                }
                if (isAll == false && CenterDialysiss.Count > 0)
                {
                    sqlwhere += " AND a.CenterId=@CenterId";
                    sql = sql + sqlwhere;
                    foreach (var center in CenterDialysiss)
                    {
                        var sqlParameterList = new List<SqlParameter>();
                        sqlParameterList.Add(new SqlParameter("@CenterId", center.Id));
                        var putinTuple = GetTupleByList<ItemStandingBookModel>(MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray()));
                        if (putinTuple.Item1)
                        {
                            list.AddRange(putinTuple.Item2);
                        }
                        var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                        string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                        var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere, sqlParameterList.ToArray()));
                        if (primaryLibraryTuple.Item1)
                        {
                            inventory_list.AddRange(primaryLibraryTuple.Item2);
                        }

                        //查询处方单明细 
                        var sqlParameterList1 = new List<SqlParameter>();
                        string where = " and presDetail.CenterId=@CenterId  and presDetail.MaterialId=@MaterialId";
                        var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                        string priceSql = GetQuerySql(xmlSqlParameter2);
                        sqlParameterList1.Add(new SqlParameter("@CenterId", center.Id));
                        sqlParameterList1.Add(new SqlParameter("@MaterialId", model.Id));
                        var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + where, sqlParameterList1.ToArray());
                        var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                        if (presdetailTuple.Item1)
                        {
                            preList.AddRange(presdetailTuple.Item2);
                        }

                        var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + " AND presDetail.CenterId=@CenterId and  presDetail.MaterialId=@MaterialId", sqlParameterList1.ToArray());
                        var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                        if (refundTuple.Item1)
                        {
                            refund_list.AddRange(refundTuple.Item2);
                        }
                    }
                }
                else
                {
                    var putinTuple = GetTupleByList<ItemStandingBookModel>(MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlwhere));
                    if (putinTuple.Item1)
                    {
                        list.AddRange(putinTuple.Item2);
                    }
                    var xmlSqlParameterInventory = GetXmlSqlParameter(_ClassName, "SqlGetItemsPrimaryAndSecondaryLibraryLibraryInventoryByDate", "时间段查询一级库,二级库库存数据");
                    string primaryLibrarySql = GetQuerySql(xmlSqlParameterInventory);
                    var primaryLibraryTuple = GetTupleByList<ItemsTalesAndInventorySummarySqlModel>(MsSqlHelper.GetSingleObj().GetDataTable(primaryLibrarySql + sqlwhere));
                    if (primaryLibraryTuple.Item1)
                    {
                        inventory_list.AddRange(primaryLibraryTuple.Item2);
                    }

                    //查询处方单明细 
                    var sqlParameterList1 = new List<SqlParameter>();
                    string where = " and  presDetail.MaterialId=@MaterialId";
                    var xmlSqlParameter2 = GetXmlSqlParameter(_ClassName, "SqlGetpPescriptionsByDate", "查询处方明细信息");
                    string priceSql = GetQuerySql(xmlSqlParameter2);
                    sqlParameterList1.Add(new SqlParameter("@MaterialId", model.Id));
                    var priceDt = MsSqlHelper.GetSingleObj().GetDataTable(priceSql + where, sqlParameterList1.ToArray());
                    var presdetailTuple = GetTupleByList<PrescriptionDetailModel>(priceDt);
                    if (presdetailTuple.Item1)
                    {
                        preList.AddRange(presdetailTuple.Item2);
                    }

                    var refundDt = MsSqlHelper.GetSingleObj().GetDataTable(refundSql + "  and  presDetail.MaterialId=@MaterialId", sqlParameterList1.ToArray());
                    var refundTuple = GetTupleByList<PrescriptionDetailModel>(refundDt);
                    if (refundTuple.Item1)
                    {
                        refund_list.AddRange(refundTuple.Item2);
                    }
                }
                var resultList = new List<ItemStandingBookModel>();
                if (list.Count > 0)
                {
                    int i = 0;
                    var datas = list.ToList().FindAll(a => a.putInstorageDate.Value >= model.BeginTime && a.putInstorageDate.Value <= model.EndTime.Value.Date.AddDays(1));
                    var dataInventory = datas.OrderBy(a => a.putInstorageDate).GroupBy(a => a.CenterId).ToList();
                    var sameList = new List<ItemStandingBookModel>();
                    foreach (var itemData in dataInventory)
                    {
                        decimal EndCount = 0;
                        decimal EndCostPrice = 0;
                        decimal EndSalePrice = 0;
                        decimal EndDifference = 0;
                        int j = 0;
                        //获取期末数据
                        if (inventory_list.Count > 0)
                        {
                            decimal totalCount = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => t.InQty.Value);
                            decimal totalCost = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                            decimal totalSale = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);

                            //期末数据
                            EndCount = totalCount;
                            EndCostPrice = totalCost;
                            EndSalePrice = totalSale;
                            EndDifference = EndSalePrice - EndCostPrice;
                        }
                        foreach (var item in itemData)
                        {
                            i++;
                            j++;
                            var resultModel = new ItemStandingBookModel();
                            resultModel = item;

                            //最终销售总价
                            decimal ReceivablePrice = 0;
                            decimal SalePrice = 0;
                            decimal TotalQty = 0;
                            if (preList.Count > 0)
                            {
                                ReceivablePrice = preList.FindAll(a => a.OutboundId == item.OutInBoundDetailId && a.MaterialId == item.MedicalItemId && a.CenterId == item.CenterId).Sum(a => a.TotalQty * a.SalePrice).Value;
                                TotalQty = preList.FindAll(a => a.OutboundId == item.OutInBoundDetailId && a.MaterialId == item.MedicalItemId && a.CenterId == item.CenterId).Sum(a => a.TotalQty).Value;
                                SalePrice = preList.FindAll(a => a.OutboundId == item.OutInBoundDetailId && a.MaterialId == item.MedicalItemId && a.CenterId == item.CenterId).Sum(a => a.SalePrice).Value;

                            }
                            if (item.ItemType == "608399fe9db64480828b3f20c6815fd1")
                            {
                                ReceivablePrice = refund_list.FindAll(a => a.ReturnWarehouseId == item.OutInBoundDetailId).Sum(a => a.TotalQty * a.SalePrice).Value;
                                SalePrice = refund_list.FindAll(a => a.ReturnWarehouseId == item.OutInBoundDetailId && a.MaterialId == item.MedicalItemId && a.CenterId == item.CenterId).Sum(a => a.SalePrice).Value;

                            }
                            var data = list.ToList().FindAll(a => a.putInstorageDate > item.putInstorageDate && a.CenterId == itemData.Key);
                            bool add = false;
                            sameList = datas.FindAll(a => a.putInstorageDate == item.putInstorageDate && a.CenterId == itemData.Key).OrderBy(a => a.putInstorageDate).ToList();
                            if (sameList.Count() > 1)
                            {
                                add = true;
                            }
                            //定义
                            decimal PutInStorageCostPriceInventory = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value) - data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.ItemQty.Value);
                            decimal PutInStorageSalePriceInventory = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value) - data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.ItemQty.Value);
                            decimal PutInStorageCountInventory = data.Where(t => t.ItemType == "8090f58cdc95404fbd779c99947df953").Sum(t => t.ItemQty.Value) - data.Where(t => t.ItemType == "0fae04062a3547f8b5597903a639118d").Sum(t => t.ItemQty.Value);

                            //出库
                            decimal OutboundCostPriceInventory = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                            decimal OutboundSalePriceInventory = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                            decimal OutboundCountInventory = data.Where(t => t.ItemType == "01f769807f9a4736be41db800606447e" || t.ItemType == "b8e6cbd359fb4816baf9e59206d4a5ef" || t.ItemType == "20e266be8d0b42679f621b91111e04ab" || t.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(t => t.ItemQty.Value);

                            //销售出库
                            decimal SalesCostPriceInventory = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.InPrice.HasValue ? t.InPrice.Value : 0));
                            decimal SalesSalePriceInventory = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0)) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value * (t.SalePrice.HasValue ? t.SalePrice.Value : 0));
                            decimal SalesCountInventory = data.Where(t => t.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(t => t.ItemQty.Value) - data.Where(t => t.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(t => t.ItemQty.Value);

                            //期末数据
                            resultModel.EndCount = EndCount + OutboundCountInventory + SalesCountInventory - PutInStorageCountInventory;
                            resultModel.EndCostPrice = EndCostPrice + OutboundCostPriceInventory + SalesCostPriceInventory - PutInStorageCostPriceInventory;
                            resultModel.EndSalePrice = EndSalePrice + OutboundSalePriceInventory + SalesSalePriceInventory - PutInStorageSalePriceInventory;
                            resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;

                            if (add)
                            {
                                int b = 0;
                                for (int k = 0; k < sameList.Count; k++)
                                {
                                    if (sameList[k].Id == item.Id)
                                    {
                                        b = k;
                                    }
                                }
                                resultModel.EndCount = resultModel.EndCount + sameList.Take(b + 1).Sum(a => a.ItemQty) - sameList.Sum(a => a.ItemQty);
                                resultModel.EndCostPrice = resultModel.EndCostPrice + sameList.Take(b + 1).Sum(a => a.ItemQty * (a.InPrice.HasValue ? a.InPrice.Value : 0)) - sameList.Sum(a => a.ItemQty * (a.InPrice.HasValue ? a.InPrice.Value : 0));
                                resultModel.EndSalePrice = resultModel.EndSalePrice + sameList.Take(b + 1).Sum(a => a.ItemQty * (a.SalePrice.HasValue ? a.SalePrice.Value : 0)) - sameList.Sum(a => a.ItemQty * (a.SalePrice.HasValue ? a.SalePrice.Value : 0));
                                resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                            }


                            var ItemQty = item.ItemQty;

                            if (item.ItemType == "01f769807f9a4736be41db800606447e" || item.ItemType == "0c0c27faec0e43d8aa9824925a340b81" || item.ItemType == "608399fe9db64480828b3f20c6815fd1" || item.ItemType == "20e266be8d0b42679f621b91111e04ab" || item.ItemType == "a7d83b6d81104475b7ede8ea467bab1f")
                            {
                                resultModel.OutboundCount = item.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" ? TotalQty : ItemQty;
                                resultModel.OutboundCost = item.InPrice;
                                resultModel.OutboundCostPrice = (item.InPrice.HasValue ? item.InPrice : 0) * resultModel.OutboundCount;
                                resultModel.OutboundSale = item.SalePrice;//SalePrice
                                resultModel.OutboundSalePrice = (item.ItemType == "a7d83b6d81104475b7ede8ea467bab1f" || item.ItemType == "608399fe9db64480828b3f20c6815fd1") ? ReceivablePrice :
                                (item.SalePrice.HasValue ? item.SalePrice : 0) * resultModel.OutboundCount;
                                resultModel.OutboundDifference = resultModel.OutboundSalePrice - resultModel.OutboundCostPrice;


                            }
                            if (item.ItemType == "8090f58cdc95404fbd779c99947df953" || item.ItemType == "0fae04062a3547f8b5597903a639118d")
                            {
                                resultModel.PutInStorageCount = item.ItemQty;
                                resultModel.PutInStorageCost = item.InPrice;
                                resultModel.PutInStorageCostPrice = (item.InPrice.HasValue ? item.InPrice : 0) * ItemQty;
                                resultModel.PutInStorageSale = item.SalePrice;
                                resultModel.PutInStorageSalePrice = (item.SalePrice.HasValue ? item.SalePrice : 0) * ItemQty;
                                resultModel.PutInStorageDifference = resultModel.PutInStorageSalePrice - resultModel.PutInStorageSalePrice;
                            }
                            if (item.ItemType == "608399fe9db64480828b3f20c6815fd1")
                            {
                                resultModel.OutboundCount = -resultModel.OutboundCount;
                                resultModel.OutboundCost = -resultModel.OutboundCost;
                                resultModel.OutboundCostPrice = -resultModel.OutboundCostPrice;
                                resultModel.OutboundSalePrice = -resultModel.OutboundSalePrice;
                                resultModel.OutboundSale = -resultModel.OutboundSale;
                                resultModel.OutboundDifference = resultModel.OutboundSalePrice - resultModel.OutboundCostPrice;
                            }
                            switch (item.ItemType)
                            {
                                case "8090f58cdc95404fbd779c99947df953"://正常入库
                                    resultModel.ItemTypeName = "正常入库";
                                    break;
                                case "0fae04062a3547f8b5597903a639118d"://冲销入库
                                    resultModel.ItemTypeName = "冲销入库";
                                    break;
                                case "01f769807f9a4736be41db800606447e"://报废出库
                                    resultModel.ItemTypeName = "报废出库";
                                    break;
                                case "0c0c27faec0e43d8aa9824925a340b81"://领用出库
                                    resultModel.ItemTypeName = "领用出库";
                                    break;
                                case "608399fe9db64480828b3f20c6815fd1"://退费退库
                                    resultModel.ItemTypeName = "退费退库";
                                    break;
                                case "a7d83b6d81104475b7ede8ea467bab1f"://销售出库
                                    resultModel.ItemTypeName = "销售出库";
                                    break;
                                case "20e266be8d0b42679f621b91111e04ab"://退货出库
                                    resultModel.ItemTypeName = "退货出库";
                                    break;

                            }
                            resultModel.UnitName = item.IsSplited ? item.SpecificationsUnitName : item.PackageUnitName;
                            resultModel.Specifications = item.IsSplited ? item.Specifications : item.Packaging;
                            if (j == 1)
                            {
                                //期初
                                var beginModel = new ItemStandingBookModel();
                                beginModel.MedicalItemName = resultModel.MedicalItemName;
                                beginModel.DialysisName = resultModel.DialysisName;
                                beginModel.UnitName = resultModel.UnitName;
                                beginModel.Specifications = resultModel.Specifications;
                                beginModel.SerialNumber = i.ToString();
                                beginModel.ItemTypeName = "期初数据";
                                beginModel.EndCount = resultModel.EndCount + (resultModel.OutboundCount.HasValue ? resultModel.OutboundCount : 0) - (resultModel.PutInStorageCount.HasValue ? resultModel.PutInStorageCount : 0);
                                beginModel.EndCostPrice = resultModel.EndCostPrice + (resultModel.OutboundCostPrice.HasValue ? resultModel.OutboundCostPrice : 0) - (resultModel.PutInStorageCostPrice.HasValue ? resultModel.PutInStorageCostPrice : 0);
                                beginModel.EndSalePrice = resultModel.EndSalePrice + ((item.SalePrice.HasValue ? item.SalePrice : 0) * ItemQty) - (resultModel.PutInStorageSalePrice.HasValue ? resultModel.PutInStorageSalePrice : 0);
                                //beginModel.EndSalePrice = resultModel.EndSalePrice + (resultModel.OutboundSalePrice.HasValue ? resultModel.OutboundSalePrice : 0) - (resultModel.PutInStorageSalePrice.HasValue ? resultModel.PutInStorageSalePrice : 0);

                                beginModel.EndDifference = beginModel.EndSalePrice - beginModel.EndCostPrice;
                                resultList.Add(beginModel);

                                i++;
                            }
                            resultModel.SerialNumber = (i).ToString();
                            resultList.Add(resultModel);

                        }
                        if (datas.Count == 0)
                        {
                            var resultModel = new ItemStandingBookModel();
                            var inventoryData = inventory_list.FindAll(a => a.CenterId == itemData.Key).First();
                            decimal totalCount = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => t.InQty.Value);
                            decimal totalCost = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                            decimal totalSale = inventory_list.FindAll(a => a.CenterId == itemData.Key).Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);

                            //期初数据
                            resultModel.SerialNumber = "1";
                            resultModel.MedicalItemName = inventoryData.MedicalItemName;
                            resultModel.DialysisName = inventoryData.DialysisName;
                            resultModel.ItemTypeName = "期初数据";
                            resultModel.Remark = "";
                            resultModel.RuChuKuNo = "";
                            resultModel.BatchNo = "";
                            resultModel.EndCount = totalCount;
                            resultModel.EndCostPrice = totalCost;
                            resultModel.EndSalePrice = totalSale;
                            resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                            resultModel.UnitName = inventoryData.IsSplited.Value ? inventoryData.SpecificationsUnitName : inventoryData.PackageUnitName;
                            resultModel.Specifications = inventoryData.IsSplited.Value ? inventoryData.Specifications : inventoryData.Packaging;
                            resultList.Add(resultModel);
                        }
                    }
                }
                else//C:\Users\Administrator\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
                {
                    var resultModel = new ItemStandingBookModel();
                    var inventoryData = inventory_list.GroupBy(a => a.CenterId);
                    foreach (var item in inventoryData)
                    {
                        decimal totalCount = item.Sum(t => t.InQty.Value);
                        decimal totalCost = item.Sum(t => (t.InPrice.HasValue ? t.InPrice.Value : 0) * t.InQty.Value);
                        decimal totalSale = item.Sum(t => (t.SalePrice.HasValue ? t.SalePrice.Value : 0) * t.InQty.Value);

                        //期初数据
                        resultModel.SerialNumber = "1";
                        resultModel.DialysisName = item.First().DialysisName;
                        resultModel.MedicalItemName = item.First().MedicalItemName;
                        resultModel.ItemTypeName = "期初数据";
                        resultModel.Remark = "";
                        resultModel.RuChuKuNo = "";
                        resultModel.BatchNo = "";
                        resultModel.EndCount = totalCount;
                        resultModel.EndCostPrice = totalCost;
                        resultModel.EndSalePrice = totalSale;
                        resultModel.EndDifference = resultModel.EndSalePrice - resultModel.EndCostPrice;
                        resultModel.UnitName = item.First().IsSplited.Value ? item.First().SpecificationsUnitName : item.First().PackageUnitName;
                        resultModel.Specifications = item.First().IsSplited.Value ? item.First().Specifications : item.First().Packaging;
                        resultList.Add(resultModel);
                    }
                }
                return resultList.ToArray();
            });
        }
        #endregion
        #region  库存管理（物资统计）
        /// <summary>
        ///低库存预警列表  
        /// </summary>
        /// <returns></returns>
        public Task<LowInventoryWarningModel[]> GetLowInventoryWarningAsync(MaterialsConfluenceInPut model)
        {
            return Task.Run(async () =>
            {
                var datas = await DialysisStore.Entities.Where(t => t.IsDelete == false && t.Id == model.CenterId).ToListAsync();

                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.MedicalItemName))
                {
                    sqlWhere += "  and a.MedicalItemName like  @MedicalItemName";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                }
                if (!string.IsNullOrWhiteSpace(model.CenterId) && model.CenterId != "0")
                {
                    sqlWhere += " and a.CenterId=@CenterId ";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetLowInventoryWarning", "低库存预警统计");
                string sql = GetQuerySql(xmlSqlParameter);
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql + sqlWhere, sqlParameterList.ToArray());
                var tuple = GetTupleByList<LowInventoryWarningModel>(dt);

                var result_list = new List<LowInventoryWarningModel>();
                if (tuple.Item1)
                {
                    var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.MedicalItemId });
                    foreach (var item in data)
                    {
                        decimal totalQty = item.Sum(a => a.InQty.Value);
                        var _data = item.First();
                        if (_data.MinInventory >= totalQty)
                        {
                            if (_data.MedicalItemType == 1)
                            {
                                var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")";
                                _data.MedicalItemName = _data.MedicalItemName + name;
                            }
                            else
                            {
                                var name = string.IsNullOrWhiteSpace(_data.Specifications) ? "" : "(" + _data.Specifications + ")";
                                _data.MedicalItemName = _data.MedicalItemName + name;
                            }
                            _data.UnitName = _data.IsSplited ? _data.SpecificationsUnitName : _data.PackageUnitName;
                            _data.DialysisName = _data.DialysisName;
                            _data.Inventory = totalQty;
                            result_list.Add(_data);
                        }
                    }
                }
                return result_list.ToArray();
            });
        }

        /// <summary>
        ///供应商入库统计
        /// </summary>
        /// <returns></returns>
        public Task<SupplierPutInStorageModel[]> GetSupplierPutInStorageStatistics(MaterialsConfluenceInPut model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.MedicalItemType) && model.MedicalItemType != "0")
                {
                    sqlWhere += " and mir.MedicalItemType=@MedicalItemType";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                }
                if (!string.IsNullOrWhiteSpace(model.CenterId) && model.CenterId != "0")
                {
                    sqlWhere += " and putinstorage.CenterId=@CenterId ";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }
                sqlWhere += "  order by mir.MedicalItemName asc";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetSupplierPutInStorageStatistics", "供应商入库统计");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime) + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<SupplierPutInStorageModel>(dt);

                var result_List = new List<SupplierPutInStorageModel>();
                if (tuple.Item1)
                {
                    var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.SupplierId, a.Id });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        var _data = item.First();
                        _data.InQty = sumQty;
                        result_List.Add(_data);
                    }
                }
                if (!string.IsNullOrWhiteSpace(model.SupplierId))
                {
                    result_List = result_List.FindAll(a => a.SupplierId == model.SupplierId);
                }

                return result_List.ToArray();
            });
        }
        /// <summary>
        ///物品库存信息
        /// </summary>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetGoodsInStockdInfo(MaterialsInPut model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                sqlWhere += "  and 1=1 ";
                if (model.MedicalItemName + "" != "")
                {
                    sqlWhere += "  and a.MedicalItemName like  @MedicalItemName";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                }
                if (model.MedicalItemType + "" != "" && model.MedicalItemType + "" != "0")
                {
                    sqlWhere += "  and a.MedicalItemType=@MedicalItemType";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                }
                if (model.CenterId + "" != "" && model.CenterId + "" != "0")
                {
                    sqlWhere += "  and a.CenterId=@CenterId";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }
                sqlWhere += "  order by a.MedicalItemName asc";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetGoodsInStockdInfo", "获取物品库存信息");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = sql + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);

                var result_List = new List<GoodsInStockdModel>(); //   
                if (tuple.Item1)
                {
                    var data = tuple.Item2.OrderBy(a => a.CenterId).GroupBy(a => new { a.CenterId, a.MedicalItemId, a.SupplierId, a.InPrice, a.BatchNo, a.SalePrice });//(a => new { a.CenterId, a.MedicalItemId, a.SupplierId, a.InPrice, a.BatchNo, a.SalePrice });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        string MName = item.First().MedicalItemName;
                        var _data = item.First();
                        var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")";// + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")")
                        _data.MedicalItemName = _data.MedicalItemName + name;

                        _data.InQty = sumQty;
                        _data.Specifications = (_data.IsSplited.Value || _data.Packaging == "/") ? _data.Specifications : _data.Packaging;
                        _data.UnitName = _data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName;
                        _data.PurchaseAmount = _data.InQty * _data.InPrice;

                        if (_data.InPrice > 0 && _data.MedicalItemType == 1)
                        {
                            decimal price = _data.InPrice.Value;
                            if (price <= 10)
                                _data.DualPrice = (price * Convert.ToDecimal(1.3)).ToFloorRound();
                            if (price > 10 && price <= 100)
                                _data.DualPrice = (price * Convert.ToDecimal(1.25)).ToFloorRound();
                            if (price > 100 && price <= 300)
                                _data.DualPrice = (price * Convert.ToDecimal(1.2)).ToFloorRound();
                            if (price > 300 && price <= 500)
                                _data.DualPrice = (price * Convert.ToDecimal(1.15)).ToFloorRound();
                            if (price > 500)
                                _data.DualPrice = (price + Convert.ToDecimal(75)).ToFloorRound();
                        }
                        if (_data.InPrice > 0 && _data.MedicalItemType == 2)
                            _data.DualPrice = _data.InPrice.Value * Convert.ToDecimal(1.15);


                        int? MonthData = 0;
                        MonthData = CenterDockingManger.listMonthGoodsInStockdData.Where(t => t.MedicalItemName == MName && t.Specifications == _data.Specifications && t.CenterId == _data.CenterId && t.Manufacturer == _data.Manufacturer).Sum(t => t.MonthInQty);
                        //上月用量获取   ,
                        _data.MonthAverage = MonthData;

                        //医保限价
                        //  _data.SocialSecurityPrice = 0; 
                        result_List.Add(_data);
                    }
                }
                return result_List.ToArray();
            });
        }

        /// <summary>
        ///物品用量统计
        /// </summary>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetUsageStatistics(MaterialsConfluenceInPut model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (model.MedicalItemName + "" != "")
                {
                    sqlWhere += "  and mir.MedicalItemName like  @MedicalItemName";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                }
                if (model.MedicalItemType + "" != "" && model.MedicalItemType != "0")
                {
                    sqlWhere += " and mir.MedicalItemType=@MedicalItemType";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                }
                if (model.CenterId + "" != "" && model.CenterId != "0")
                {
                    sqlWhere += " AND outbound.CenterId=@CenterId";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }//	order by  
                sqlWhere += "  order by mir.MedicalItemName,outbound.AuditDate asc";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetItemUsageStatistics", "物品用量统计");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = string.Format(sql, model.BeginTime.Value.Date, model.EndTime) + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);

                var result_List = new List<GoodsInStockdModel>();
                if (tuple.Item1)
                {
                    if (model.GroupType == 0 || !model.GroupType.HasValue)
                    {
                        var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.Manufacturer, a.MedicalItemName, a.Specifications, a.InPrice, a.SalePrice });
                        foreach (var item in data)
                        {
                            //if (item.First().MedicalItemName == "一次性使用动静脉穿刺针")
                            //{   
                            // 
                            //}
                            var sumQty = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty);

                            var TotalInPrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.InPrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.InPrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.InPrice);


                            var TotalSalePrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.SalePrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.SalePrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.SalePrice);
                            var _data = item.First();
                            var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                            _data.MedicalItemName = _data.MedicalItemName + name;

                            _data.TotalInQty = sumQty;
                            _data.TotalInPrice = TotalInPrice;
                            _data.TotalSalePrice = TotalSalePrice;
                            _data.Specifications = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.Specifications : _data.Packaging) : "";
                            _data.UnitName = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName) : "";
                            _data.outDateValue = "";
                            result_List.Add(_data);
                        }
                    }
                    if (model.GroupType == 1)
                    {
                        var Daydata = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.Manufacturer, a.MedicalItemName, a.Specifications, a.InPrice, a.SalePrice, a.outDate.Date });
                        foreach (var item in Daydata)
                        {
                            //if (item.First().MedicalItemName == "一次性使用动静脉穿刺针")
                            //{    

                            //}
                            var sumQty = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty);

                            var TotalInPrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.InPrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.InPrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.InPrice);


                            var TotalSalePrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.SalePrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.SalePrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.SalePrice);
                            var _data = item.First();
                            var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                            _data.MedicalItemName = _data.MedicalItemName + name;

                            _data.TotalInQty = sumQty;
                            _data.TotalInPrice = TotalInPrice;
                            _data.TotalSalePrice = TotalSalePrice;
                            _data.Specifications = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.Specifications : _data.Packaging) : "";
                            _data.UnitName = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName) : "";
                            _data.outDateValue = item.FirstOrDefault().outDate.ToString("yyyy-MM-dd");
                            result_List.Add(_data);
                        }
                    }
                    if (model.GroupType == 2)
                    {
                        var Montydata = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.Manufacturer, a.MedicalItemName, a.Specifications, a.InPrice, a.SalePrice, a.outDateMonty });
                        foreach (var item in Montydata)
                        {
                            //if (item.First().MedicalItemName == "一次性使用动静脉穿刺针")
                            //{ 
                            //}
                            var sumQty = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty);

                            var TotalInPrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.InPrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.InPrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.InPrice);


                            var TotalSalePrice = item.ToList().FindAll(a => a.ItemType == "a7d83b6d81104475b7ede8ea467bab1f").Sum(a => a.InQty * a.SalePrice) - item.ToList().FindAll(a => a.ItemType == "608399fe9db64480828b3f20c6815fd1").Sum(a => a.InQty * a.SalePrice) + item.ToList().FindAll(a => a.ItemType == "0c0c27faec0e43d8aa9824925a340b81").Sum(a => a.InQty * a.SalePrice);
                            var _data = item.First();
                            var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                            _data.MedicalItemName = _data.MedicalItemName + name;

                            _data.TotalInQty = sumQty;
                            _data.TotalInPrice = TotalInPrice;
                            _data.TotalSalePrice = TotalSalePrice;
                            _data.Specifications = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.Specifications : _data.Packaging) : "";
                            _data.UnitName = _data.MedicalItemId != null ? (_data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName) : "";
                            _data.outDateValue = item.FirstOrDefault().outDateMonty;
                            result_List.Add(_data);
                        }
                    }

                }
                return result_List.ToArray();
            });
        }
        /// <summary>
        ///高积压库存统计
        /// </summary>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetHighOverstockStatistics(MaterialsInPut model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (model.MedicalItemName + "" != "")
                {
                    sqlWhere += "  and a.MedicalItemName like  @MedicalItemName";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                }
                if (model.MedicalItemType + "" != "" && model.MedicalItemType + "" != "0")
                {
                    sqlWhere += "  and a.MedicalItemType=@MedicalItemType";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                }
                if (model.CenterId + "" != "" && model.CenterId != "0")
                {
                    sqlWhere += "  and a.CenterId=@CenterId";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }
                sqlWhere += "  and a.InQty>0 order by a.MedicalItemName asc";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetGoodsInStockdInfo", "获取物品库存信息");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = sql + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);

                var result_List = new List<GoodsInStockdModel>();
                if (tuple.Item1)
                {
                    var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.MedicalItemId });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        var _data = item.First();
                        var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                        _data.MedicalItemName = _data.MedicalItemName + name;

                        _data.InQty = sumQty;
                        _data.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                        _data.UnitName = _data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName;
                        if (_data.InQty >= 200)
                        {
                            result_List.Add(_data);
                        }
                    }
                }
                return result_List.ToArray();
            });
        }

        /// <summary>
        /// 近效期统计
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<GoodsInStockdModel[]> GetValidDateStatistics(MaterialsInPut model)
        {
            return Task.Run(() =>
            {
                var sqlParameterList = new List<SqlParameter>();
                string sqlWhere = string.Empty;
                if (model.MedicalItemName + "" != "")
                {
                    sqlWhere += "  and a.MedicalItemName like  @MedicalItemName";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemName", $"%{model.MedicalItemName}%"));
                }
                if (model.MedicalItemType + "" != "" && model.MedicalItemType + "" != "0")
                {
                    sqlWhere += "  and a.MedicalItemType=@MedicalItemType";
                    sqlParameterList.Add(new SqlParameter("@MedicalItemType", model.MedicalItemType));
                }
                if (model.CenterId + "" != "" && model.CenterId != "0")
                {
                    sqlWhere += "  and a.CenterId=@CenterId";
                    sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                }
                if (model.ValidData.HasValue)
                {
                    sqlWhere += "  and a.QualityDate <=@QualityDate";
                    sqlParameterList.Add(new SqlParameter("@QualityDate", model.ValidData));
                }
                sqlWhere += "  and a.InQty>0 order by a.MedicalItemName asc";
                var xmlSqlParameter = GetXmlSqlParameter(_ClassName, "SqlGetGoodsInStockdInfo", "获取物品库存信息");
                string sql = GetQuerySql(xmlSqlParameter);
                sql = sql + sqlWhere;
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);

                var result_List = new List<GoodsInStockdModel>();
                if (tuple.Item1)
                {
                    var data = tuple.Item2.OrderBy(a => a.DialysisName).GroupBy(a => new { a.CenterId, a.MedicalItemId });
                    foreach (var item in data)
                    {
                        var sumQty = item.Sum(a => a.InQty);
                        var _data = item.First();
                        var name = string.IsNullOrWhiteSpace(_data.GoodsName) ? "" : "(" + _data.GoodsName + ")" + (string.IsNullOrWhiteSpace(_data.Brand) ? "" : "(" + _data.Brand + ")");
                        _data.MedicalItemName = _data.MedicalItemName + name;

                        _data.InQty = sumQty;
                        _data.Specifications = _data.IsSplited.Value ? _data.Specifications : _data.Packaging;
                        _data.UnitName = _data.IsSplited.Value ? _data.SpecificationsUnitName : _data.PackageUnitName;
                        //if (_data.InQty >= 200)
                        //{
                        result_List.Add(_data);
                        // }
                    }
                }
                return result_List.ToArray();
            });
        }


        //月均用量统计

        /// <summary>
        ///物品用量统计
        /// </summary>
        /// <returns></returns>
        public Task<MonthGoodsInStockdModel[]> GetMonthUsageStatistics(MaterialsConfluenceInPut model)
        {
            return Task.Run(async () =>
            {
                List<MonthGoodsInStockdModel> DZMonthStock = new List<MonthGoodsInStockdModel>();

                if (model.MedicalItemType == "4" || model.MedicalItemType == "0")
                {

                    var tempda = await GetOtherOutboundDetailByDateAsync(model);
                    DZMonthStock = tempda.ToList();
                }
                if (model.MedicalItemType == "4")
                    return DZMonthStock.ToArray();
                var data = await GetUsageStatistics(model);

                List<MonthGoodsInStockdModel> MonthStock = new List<MonthGoodsInStockdModel>();

                //  int month = ((model.EndTime.Value.Year - model.BeginTime.Value.Year) * 12) + model.EndTime.Value.Month - model.BeginTime.Value.Month+1;
                int month = (model.EndTime - model.BeginTime).Value.Days / 29;//()
                if (month == 0) month = 1;
                if (data.Length > 0)
                {
                    if (model.MedicalItemType == "2")
                    {

                        var GroupData = data.GroupBy(t => new { t.CenterId, t.MedicalItemName, t.Specifications, t.Manufacturer });
                        foreach (var item in GroupData)
                        {
                            string DicName = item.First().DialysisName;
                            var qty = (int)(item.Sum(t => t.TotalInQty) / month);
                            if ((item.Sum(t => t.TotalInQty) % month) > 0) qty = qty + 1;
                            MonthStock.Add(new MonthGoodsInStockdModel()
                            {
                                CenterId = item.First().CenterId,
                                DialysisName = DicName,
                                Id = item.First().Id,
                                ItemType = item.First().ItemType,
                                ItemTypeName = item.First().ItemTypeName,
                                MedicalItemName = item.First().MedicalItemName,
                                MonthInQty = qty,
                                TotalInQty = item.Sum(t => t.TotalInQty),
                                Specifications = item.First().Specifications,
                                SpecificationsUnitName = item.First().UnitName,
                                Manufacturer = item.First().Manufacturer,
                                MonthInMoney = item.First().InPrice * qty,
                                InPrice = item.First().InPrice,

                            });//rebirth
                        }
                    }
                    else
                    {
                        var GroupData = data.GroupBy(t => new { t.CenterId, t.MedicalItemName, t.Specifications, t.Manufacturer });
                        foreach (var item in GroupData)
                        {
                            var qty = (int)(item.Sum(t => t.TotalInQty) / month);
                            if ((item.Sum(t => t.TotalInQty) % month) > 0) qty = qty + 1;
                            string DicName = item.First().DialysisName;
                            MonthStock.Add(new MonthGoodsInStockdModel()
                            {
                                CenterId = item.First().CenterId,
                                DialysisName = DicName,
                                Id = item.First().Id,
                                ItemType = item.First().ItemType,
                                ItemTypeName = item.First().ItemTypeName,
                                MedicalItemName = item.First().MedicalItemName,
                                MonthInQty = qty,
                                TotalInQty = item.Sum(t => t.TotalInQty),
                                Specifications = item.First().Specifications,
                                SpecificationsUnitName = item.First().UnitName,
                                Manufacturer = item.First().Manufacturer,
                                InPrice = item.First().InPrice,
                                MonthInMoney = item.First().InPrice * qty,
                            });
                        }
                    }
                    if (model.MedicalItemType == "0")
                        MonthStock.AddRange(DZMonthStock);
                }
                MonthStock.Add(new MonthGoodsInStockdModel()
                {
                    DialysisName = "合计",
                    MonthInMoney = MonthStock.Sum(t => t.MonthInMoney),
                });

                return MonthStock.ToArray();




            });
        }



        /// <summary>
        /// 低值易耗品用量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<MonthGoodsInStockdModel[]> GetOtherOutboundDetailByDateAsync(MaterialsConfluenceInPut model)
        {
            return Task.Run(() =>
            {

                var sqlwhere = string.Empty;


                sqlwhere += $" and  mir.MedicalItemType in (4)";

                var xmlSqlParameter = GetXmlSqlParameter("MaterialsStatisticaManager", "SqlGetOtherOutboundDetailByDate", "时间段查询其它出库明细信息");
                string sql = GetQuerySql(xmlSqlParameter);

                var list = new List<GoodsInStockdModel>();
                var CenterDialysiss = new List<CenterDialysis>();
                DateTime bgtime = model.BeginTime.Value;
                DateTime endtime = model.EndTime.Value;
                if (!string.IsNullOrWhiteSpace(model.CenterId) && model.CenterId != "0")
                    sqlwhere += $" AND center.Id=@CenterId";

                sql = string.Format(sql, bgtime, endtime).Replace("@sqlWhere", sqlwhere);
                var sqlParameterList = new List<SqlParameter>();
                sqlParameterList.Add(new SqlParameter("@CenterId", model.CenterId));
                var dt = MsSqlHelper.GetSingleObj().GetDataTable(sql, sqlParameterList.ToArray());
                var tuple = GetTupleByList<GoodsInStockdModel>(dt);
                if (tuple.Item1)
                {
                    list = tuple.Item2;
                }
                var result_list = new List<MonthGoodsInStockdModel>();

                if (list.Count > 0)
                {
                    var data = list.GroupBy(a => new { a.CenterId, a.MedicalItemName, a.Specifications, a.Manufacturer });
                    int month = (model.EndTime - model.BeginTime).Value.Days / 30;//()
                    if (month == 0) month = 1; ;
                    foreach (var item in data)
                    {
                        var outboundModel = item.First();
                        outboundModel.ItemQty = item.Where(t => t.OutboundType != "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty) - item.Where(t => t.OutboundType == "92ea5788bb044fb08fa71ce13dfefb34").Sum(t => t.ItemQty);

                        string DicName = item.First().DialysisName;
                        result_list.Add(new MonthGoodsInStockdModel()
                        {
                            CenterId = item.First().CenterId,
                            DialysisName = DicName,
                            Id = item.First().Id,
                            ItemType = item.First().ItemType,
                            ItemTypeName = item.First().ItemTypeName,
                            MedicalItemName = item.First().MedicalItemName,
                            MonthInQty = Convert.ToInt32((item.Sum(t => t.ItemQty) / month)),
                            TotalInQty = item.Sum(t => t.ItemQty),
                            Specifications = item.First().Specifications,

                            Manufacturer = item.First().Manufacturer,
                            MonthInMoney = item.First().InPrice * Convert.ToInt32((item.Sum(t => t.ItemQty) / month)),
                            InPrice = item.First().InPrice,
                            SpecificationsUnitName = item.First().IsSplited.Value ? item.First().Specifications : item.First().Packaging,
                            //   item.First().IsSplited.Value ? item.First().SpecificationsUnitName : item.First().PackageUnitName,

                        });
                    }
                }
                return result_list.OrderBy(t => t.DialysisName).ToArray();
            });
        }




        #endregion
    }
}
