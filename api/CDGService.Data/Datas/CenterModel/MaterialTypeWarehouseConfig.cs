using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 物品类库房配置表
    /// </summary>
    public class MaterialTypeWarehouseConfig
    {
        public string Id { get; set; }
        public string WarehouseTypeId { get; set; }
        public string MaterialTypeId { get; set; }
        public string Remark { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }
    }
}
