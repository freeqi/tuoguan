using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Data
{
    public class ServiceMessage<T>
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据包
        /// </summary>
        public T Data { get; set; }

    }
    public class ServiceMessage 
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据包
        /// </summary>
        public string Data { get; set; }

    }
}
