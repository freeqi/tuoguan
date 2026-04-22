using System;
using System.Collections.Generic;
using System.Text;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// tableName--WarehouseCatalogs
    /// </summary>
    public class WareHouseCatalogs
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public int? SortNo { get; set; }
        public int? Category { get; set; }
        public string Founder { get; set; }
        public DateTime? FounderDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifierDate { get; set; }
        public int? DataState { get; set; }
        public string CenterId { get; set; }
        public DateTime? CollectData { get; set; }

    }

}
