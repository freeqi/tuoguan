using CDGService.WebAPI.Datas;
using CDGService.WebAPI.Extenstions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CDGService.WebAPI
{
    /// <summary>
    /// 全局异常过滤器
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        ///<inheritdoc/>
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true; //标记异常已处理

            LogHelper.WriteErrLog($"调用方法{context.ActionDescriptor.DisplayName}出现异常：{context.Exception.Message}{Environment.NewLine}StackTrace: {context.Exception.StackTrace}");

            var result = new ServiceMessage<object>(context.Exception);
            context.Result = new ObjectResult(result);
        }
    }
}
