using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data.Datas
{
    /// <summary>
    /// 系统参数
    /// </summary>
    [Table("SysParameter")]
    public class SysParameter
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [Required]
        public string ParamName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        [Required]
        public string ParamValue { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }
}
