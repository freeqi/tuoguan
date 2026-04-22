using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 建卡贫困户信息表
    /// </summary>
    public class CardPatient
    {
        public int Id { get; set; }
        public string IdCardNo { get; set; }
        public string PatientName { get; set; }

    }
    /// <summary>
    /// 建卡贫困户兜底记录表
    /// </summary>
    public class BalanceDDInfo
    {
        public int Id { get; set; }
        public string BalanceNos { get; set; }
        public decimal? DDPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? OwnPrice { get; set; }
        public DateTime? DDDate { get; set; }
        public string CreatePerson { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsCal { get; set; }
        public DateTime? CalDate { get; set; }
        public string PatientName { get; set; }
        public string PatientNo { get; set; }
        public DateTime CancelCalDate { get; set; }
        public decimal? ZHZF { get; set; }
        public decimal? XJZF { get; set; }


    }
}
