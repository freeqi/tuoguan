using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class WaterFuelOutPut
    {

        public string Id { get; set; }

        public string CenterId { get; set; }
        public string CenterName { get; set; }
        /// <summary>
        /// 门诊部租金
        /// </summary>
        public decimal? mzbzj { get; set; } = 0;
        /// <summary>
        /// 宿舍租金
        /// </summary>
        public decimal? sszj { get; set; } = 0;
        /// <summary>
        /// 物管
        /// </summary>
        public decimal? wg { get; set; } = 0;
        /// <summary>
        /// 水电
        /// </summary>
        public decimal? sd { get; set; } = 0;
        /// <summary>
        /// 工资
        /// </summary>
        public decimal? gz { get; set; } = 0;
        /// <summary>
        /// 社保
        /// </summary>
        public decimal? sb { get; set; } = 0;
        /// <summary>
        /// 福利
        /// </summary>
        public decimal? fl { get; set; } = 0;
        /// <summary>
        /// 爱心基金
        /// </summary>
        public decimal? axjj { get; set; } = 0;
        /// <summary>
        /// 折旧费
        /// </summary>
        public decimal? zjf { get; set; } = 0;
        /// <summary>
        /// 装修
        /// </summary>
        public decimal? zx { get; set; } = 0;
        /// <summary>
        /// 车辆费用
        /// </summary>
        public decimal? clfy { get; set; } = 0;
        /// <summary>
        /// 其他
        /// </summary>
        public decimal? qt { get; set; } = 0;

        // public string ItemCardNum { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public DateTime? Month { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalAmountPriec { get; set; } = 0;
        
        public string Remarks { get; set; }
        public int? DataState { get; set; }


    }


    public class WaterFuelStatisOutPut
    {
        public string CenterName { get; set; }

        public decimal? WaterSum { get; set; }
        public decimal? WaterPrice { get; set; }
        public decimal? FuelSum { get; set; }
        public decimal? FuelPrice { get; set; }
        public decimal? SumPrice { get; set; }
    }

    public class WaterFuelInPut
    {
        public string Id { get; set; }

        public string CenterId { get; set; }
        public List<ItemDetail> itemDetails { get; set; }

        // public string ItemCardNum { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public DateTime? Month { get; set; }
        public string Remarks { get; set; }
        public int? DataState { get; set; }

    }

    public class ItemDetail
    {
        /// <summary>
        /// 1租金（门诊部）、2租金（宿舍）、3物管、4水电、5工资、6社保、7福利、
        /// 8爱心基金、9折旧费、10装修/无形资产摊销、11车辆费用（含维修保养）、12 其他
        /// </summary>
        public string  ItemType { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? AmountPriec { get; set; } = 0;
    }

    public class WaterFuelQueryInPut
    {
        public string Id { get; set; }
        public string CenterId { get; set; }
        public int ItemType { get; set; }
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string KeywordValue { get; set; }

        public int PageNum { get; set; }

        public int PageSize { get; set; }



    }
}
