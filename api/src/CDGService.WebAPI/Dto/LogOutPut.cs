using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDGService.WebAPI.Dto
{
    public class LogOutPut
    {

        public string Id { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public Data.Enums.LogType LogCode { get; set; }
        public string LogType { get; set; }
        /// <summary>
        /// 操作账号ID
        /// </summary>
        public string  OperatorId { get; set; }
        /// <summary>
        /// 操作用户名
        /// </summary>
        public string OperatorUserName { get; set; }
        /// <summary>
        /// 操作人员
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>        
        public string CreateTime { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string LogContent { get; set; }
        /// <summary>
        /// 日志简介，格式：
        /// <para>{数据类型}+{日志代码中文说明}</para>
        /// </summary>
        //public string LogProfile { get; set; }
        ///// <summary>
        ///// 备注
        ///// </summary>
        //public string Note { get; set; }
    }
}
