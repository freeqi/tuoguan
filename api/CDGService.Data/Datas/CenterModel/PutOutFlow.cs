using System;

namespace CDGService.Data.Datas
{
    public class PutOutFlow
    {
        public string Id { get; set; }
        public string PutOutId { get; set; }
        public string ItemDetailId { get; set; }
        public string ItemType { get; set; }
        public int? ItemState { get; set; }
        public decimal? ItemQty { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
