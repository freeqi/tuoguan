using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 收入日结（收费项目）
    /// </summary>
    public class DayItemIncome
    {

        public string Id { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
        /// <summary>
        /// 收费项目
        /// </summary>
        public string Cost { get; set; }
        [ForeignKey(nameof(Cost))]
        public virtual SystemDictionary SysDicCost { get; set; }

        /// <summary>
        /// 治疗模式 字典ID
        /// </summary>
        public string CurePattern { get; set; }
        /// <summary>
        /// 来源 （药品-药房 材料 - 库房  透析费-肾内科）
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        public decimal IncomeMoney { get; set; }

        /// <summary>
        /// 成本金额
        /// </summary>
        public decimal CostMoney { get; set; }
        /// <summary>
        /// 毛利
        /// </summary>
        public decimal GrossProfitMoney { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal DiscountsMoney { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime SettlementDate { get; set; }

    }
}
