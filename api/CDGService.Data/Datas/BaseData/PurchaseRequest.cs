using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas.BaseData
{
    public class PurchaseRequest
    {

        public string Id { get; set; }
        public int WarehouseId { get; set; }
        public int PurchaseNo { get; set; }
        public int ItemType { get; set; }
        public int TotalQty { get; set; }
        public int TotalCost { get; set; }
        public int AuditConditionId { get; set; }
        public int AuditDate { get; set; }
        public int Advice { get; set; }
        public int Auditor { get; set; }
        public int IsERP { get; set; }
        public int Remark { get; set; }
        public string Founder { get; set; }
        public int FounderDate { get; set; }
        public string Modifier { get; set; }
        public int ModifierDate { get; set; }
        public int DataState { get; set; }
    }
}
