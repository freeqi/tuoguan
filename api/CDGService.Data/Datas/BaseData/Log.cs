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
    /// 日志信息表
    /// </summary>
    public class Log
    {
        public string  Id { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public Enums.LogType LogCode { get; set; }
        /// <summary>
        /// 操作账号ID
        /// </summary>
        public string  OperatorId { get; set; }
        [ForeignKey(nameof(OperatorId))]
        public virtual User OperatorUser { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>        
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string LogContent { get; set; }
        /// <summary>
        /// 日志简介，格式：
        /// <para>{数据类型}+{日志代码中文说明}</para>
        /// </summary>
        public string LogProfile { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }
}
