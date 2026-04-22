using CDGService.Data.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{

    /// <summary>
    /// 报废出库申请列表
    /// </summary>
    public class MaterialApplyApproveOutPut: MaterialApplyApprove
    {
        /// <summary>
        /// 添加人
        /// </summary>
        public string FounderName { get; set; }
        /// <summary>
        /// 中心端审核人
        /// </summary>
        public string AuditorName { get; set; }
        /// <summary>
        /// 库房
        /// </summary>
        public string WarehouseName { get; set; } 
        /// <summary>
        /// 机构
        /// </summary>
         public string CenterName { get; set; }
        /// <summary>
        /// 集团端审核人
        /// </summary>
        public string GroupApprovalName { get; set; }
    }


    public class mapDetailOutPut
    {
        public OrderApproveOutPut[] orderApproveOutPuts { get; set; }
        public MaterialApplyApproveOutPut approveOutPut { get; set; }
        public MaterialApplyApproveDetailOutPut[] MaterialApplyApproveDetailOutPuts { get; set; }
    }



    /// <summary>
    /// 报废出库申请详情
    /// </summary>
    public class MaterialApplyApproveDetailOutPut : MaterialApplyApproveDetail
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
         /// <summary>
         /// 厂家
         /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string SpeUnitCHS { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SuppName { get; set; }

        public string Specifications { get; set; }


    }













    /// <summary>
    /// 报废出库申请列表
    /// </summary>
    public class MMaterialsWarningApplyListOutPut : MaterialsWarningApplyList
    {
        /// <summary>
        /// 添加人
        /// </summary>
        public string FounderName { get; set; }
        /// <summary>
        /// 中心端审核人
        /// </summary>
        public string AuditorName { get; set; }
        /// <summary>
        /// 库房
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 机构
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// 集团端审核人
        /// </summary>
        public string GroupApprovalName { get; set; }
    }


    public class MaterialsWarningOutPut
    {
        public OrderApproveOutPut[] orderApproveOutPuts { get; set; }
        public MMaterialsWarningApplyListOutPut  applyListOutPut { get; set; }
        public MaterialsWarningApplyDetailOutPut[]  detailOutPuts { get; set; }
    }



    /// <summary>
    /// 报废出库申请详情
    /// </summary>
    public class MaterialsWarningApplyDetailOutPut : MaterialsWarningApplyDetailList
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string MedicalItemName { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 厂家
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string SpeUnitCHS { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SuppName { get; set; }
        public decimal? SumPrice { get; set; }


    }







}
