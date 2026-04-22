using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    public class PatientStatis
    {
        public string Id { get; set; }

        public DateTime PatientDate { get; set; }
        public int ManCount { get; set; }
        public int WomanCount { get; set; }
        public int SumCount { get; set; }
        public string  CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public virtual CenterDialysis centerDialysis { get; set; }
    }


    public class TempTab
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CostMoney { get; set; }
        public decimal ReceivableMoney { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CenterName { get; set; }
        public string CardNum { get; set; }
        public string ContactAddress { get; set; }
        public DateTime? Birthday { get; set; }
        public string BalanceNo { get; set; }
        public int DataState { get; set; }
        public string CenterId { get; set; }
        public int DataType { get; set; }

    }
}
