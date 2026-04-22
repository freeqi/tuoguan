using CDGService.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class EquipmentInPut
    {
        public string  id { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string  EqType { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public EquiState EqState { get; set; }
        /// <summary>
        /// 中心ID
        /// </summary>
        public string  CenterId { get; set; }

        public int PageNum { get; set; }
        public int PageSize { get; set; }
    }

}
