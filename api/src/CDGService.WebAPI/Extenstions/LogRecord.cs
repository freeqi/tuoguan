using Castle.DynamicProxy;
using CDGService.Data.Datas;
using CDGService.Data.Enums;
using CDGService.Data.Threading;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Datas;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text;

namespace CDGService.WebAPI.Extenstions
{
    /// <summary>
    /// 日志记录 
    /// </summary>
    public class LogRecordAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="operationObj">操作对象</param>
        /// <param name="logType">操作类型</param>
        public LogRecordAttribute(string operationObj, LogType logType = Data.Enums.LogType.DataAccess)
        {
            OperationObj = operationObj;
            LogType = logType;
        }

        /// <summary>
        /// 操作对象
        /// </summary>
        public string OperationObj { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public LogType LogType { get; set; }

        ///<inheritdoc/>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);

            var logManager = context.HttpContext.RequestServices.GetRequiredService<LogManager>();
            var sbContent = new StringBuilder();
            switch (LogType)
            {
                case LogType.DataAccess:
                    await logManager.WriteDataAccessLogAsync(OperationObj);
                    break;
                case LogType.DataUpdate:
                    break;
                case LogType.DataDelete:
                    if (context.Result is ServiceMessage<bool> result && result.Result)
                    {
                        var argumentKey = context.ActionArguments.Keys.FirstOrDefault();
                        var deletedId = argumentKey == null ? string.Empty : context.ActionArguments[argumentKey]?.ToString();
                        sbContent.Append(deletedId);
                        await logManager.WriteDeleteLogAsync(OperationObj, deletedId);
                    }
                    break;
                case LogType.UserLogin:
                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Obsolete("已弃用，其职能由" + nameof(LogRecordAttribute) + "替代")]
    public class LogRecordInterceptor : IInterceptor
    {
        private readonly TokenChecker _tokenChecker;
        private readonly LogManager _logManager;
        private readonly IGetUserInfo _userinfo;

        public LogRecordInterceptor(IGetUserInfo userInfo, LogManager logManager)
        {
            _userinfo = userInfo;
            _logManager = logManager;
        }


        public void Intercept(IInvocation invocation)
        {
            var isLog = invocation.Method.GetCustomAttributes().Any(t => t is LogRecordAttribute);
            if (!isLog)
            {
                invocation.Proceed();
            }
            else
            {
                try
                {
                    invocation.Proceed();
                    Task.WaitAll(((Task)invocation.ReturnValue));
                    var task = Log(invocation);
                    Task.WaitAll(task);

                }
                catch (Exception exception)
                {
                    var d = invocation.MethodInvocationTarget.ReturnType;
                    var gtype = d.GetGenericArguments()[0];

                    if (exception.InnerException != null)
                        exception = exception.InnerException;
                    var s = Activator.CreateInstance(gtype, exception);
                    var ee = Convert.ChangeType(s, gtype);
                    var adviceTaskSource = Data.Threading.TaskCompletionSource.Create(d.GetTaskType());
                    adviceTaskSource.SetResult(ee);

                    invocation.ReturnValue = adviceTaskSource.Task;
                }
            }

        }

        private async Task Log(IInvocation invocation)
        {
            var logAttribute = invocation.Method.GetCustomAttribute<LogRecordAttribute>();
            if (logAttribute == null)
            {
                return;
            }
            var sbContent = new StringBuilder();
            switch (logAttribute.LogType)
            {
                case LogType.DataAccess:
                    await _logManager.WriteDataAccessLogAsync(logAttribute.OperationObj);
                    break;
                case LogType.DataUpdate:
                    break;
                case LogType.DataDelete:
                    var invokeResult = invocation.ReturnValue as Task<ServiceMessage<bool>>;
                    if (invokeResult != null && invokeResult.Result.Result)
                    {
                        var deletedId = (string)invocation.Arguments[0];
                        sbContent.Append(deletedId);
                        await _logManager.WriteDeleteLogAsync(logAttribute.OperationObj, deletedId);
                    }
                    break;
                case LogType.UserLogin:
                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
