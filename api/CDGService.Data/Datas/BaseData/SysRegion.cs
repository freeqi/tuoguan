using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 地区表
    /// </summary>
    public class SysRegion
    {
        public string Id { get; set; }
        [Required]
        public string  RegionName { get; set; }
        /// <summary>
        /// 地区缩写
        /// </summary>

        public string  RegionShot { get; set; }
        /// <summary>
        /// 地区行政编码
        /// </summary>
        public string  RegionCode { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        [Required]
        public string  RegionParentId { get; set; }
        /// <summary>
        /// 地区级别
        /// 1-省、自治区、直辖市 2地级市、地区、治州、盟 3市辖区、级市、县'
        /// </summary>
        [Required]
        public int RegionLevel { get; set; }
    }
}
