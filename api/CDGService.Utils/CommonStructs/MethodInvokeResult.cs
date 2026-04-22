using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Utils.CommonStructs
{
    /// <summary>
    /// 方法调用结果
    /// </summary>
    public class MethodInvokeResult
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public MethodInvokeResult()
        {
            
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="errMsg">出错信息</param>
        public MethodInvokeResult(bool isSuccess, string errMsg,TypeCode code)
        {
            IsSuccess = isSuccess;
            ErrMsg = errMsg;
            Code = code;
        }

        /// <summary>
        /// 调用是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 出错信息
        /// </summary>
        public string ErrMsg { get; set; }
       /// <summary>
       /// 状态码
       /// </summary>
        public TypeCode Code { get; set; }
    }

    /// <summary>
    /// 方法调用结果（泛型）
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    public class MethodInvokeResult<T> : MethodInvokeResult
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public MethodInvokeResult()
        {
            
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="errMsg">出错信息</param>
        /// <param name="result">返回结果</param>
        public MethodInvokeResult(bool isSuccess, string errMsg, TypeCode code, T result)
            : base(isSuccess, errMsg,code)
        {
            Result = result;
        }

        /// <summary>
        /// 结果对象
        /// </summary>
        public T Result { get; set; }
    }
}
