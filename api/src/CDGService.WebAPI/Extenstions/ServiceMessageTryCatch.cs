using System;
using System.ComponentModel;
using System.Linq;
using Castle.DynamicProxy;
using System.Reflection;
using CDGService.WebAPI.Datas;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel;
using CDGService.Data.Datas;
using CDGService.Data.Threading;
using CDGService.WebAPI.DataCore;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CDGService.WebAPI.Extenstions
{

    /// <summary>
    /// 这是返回ServerMessage的api，会自动注入到容器
    /// </summary>
    public interface IServerMessageApi
    {

    }

    /// <summary>
    /// 包装返回的结果，如果出现异常，会自动返回异常信息。
    /// 返回对象必须为Task<ServiceMessage<>>
    /// 方法必须被标记为 virtual
    /// </summary>
    public class ServiceMessageTryCatchAttribute : Attribute
    {
    }

    /// <summary>
    /// 检查是否登录
    /// </summary>
    public class CheckLoginAttribute : Attribute
    {

    }

    public class ServiceMessageTryCatchInterceptor : IInterceptor
    {
        private readonly TokenChecker _tokenChecker;
        private readonly IGetUserInfo _userinfo;

        public ServiceMessageTryCatchInterceptor(TokenChecker tokenChecker, IGetUserInfo userinfo)
        {
            _tokenChecker = tokenChecker;
            _userinfo = userinfo;
        }


        public void Intercept(IInvocation invocation)
        {
            var istrycatch = invocation.Method.GetCustomAttributes().Any(t => t is ServiceMessageTryCatchAttribute);
            if (!istrycatch)
            {
                invocation.Proceed();
            }
            else
            {
                checkType(invocation.Method.ReturnType);
                try
                {
                    CheckLogin(invocation);
                    invocation.Proceed();
                    Task.WaitAll(((Task)invocation.ReturnValue));
                }
                catch (Exception exception)
                {
                    var d = invocation.MethodInvocationTarget.ReturnType;
                    var gtype = d.GetGenericArguments()[0];
                    if (exception.InnerException != null)
                        exception = exception.InnerException;
                    var s = Activator.CreateInstance(gtype, exception);
                    var ee = Convert.ChangeType(s, gtype);
                    var adviceTaskSource = CDGService.Data.Threading.TaskCompletionSource.Create(d.GetTaskType());
                    adviceTaskSource.SetResult(ee);
                    LogHelper.WriteErrLog($"调用方法{invocation.Method?.Name}出现异常：{exception.Message}{Environment.NewLine}StackTrace: {exception.StackTrace}");
                    invocation.ReturnValue = adviceTaskSource.Task;
                }
            }
        }

        private void CheckLogin(IInvocation invocation)
        {
            var isneedcheck = invocation.Method.GetCustomAttributes().Any(t => t is CheckLoginAttribute);
            if (!isneedcheck)
            {
                var ip = (invocation.Proxy as Controller)?.HttpContext?.GetUserIp();

                if (!string.IsNullOrEmpty(ip) && (ip.Contains("172.16.7") || ip.Contains("172.16.6")))
                { 
                    throw new Exception("禁止访问");
                }
                else
                    return;

            }

            var request = (invocation.Proxy as Controller)?.HttpContext.Request;
            var token = request?.Headers["token"];

            //  token = "764dafb2c1aa59482f583812b07e4234b2a488fa890fa0b5";//测试用 dd669205cad8860e ab29aa0ae0644549a6bfb9639889f54b  铜梁：tl29aa0ae0644549a6bfb9639889f54d
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("未登录的请求");
            var userId = _tokenChecker.IsValid(token);
            if (userId == "")
            {
                throw new Exception("未授权的请求或授权已过期");
            }
            _userinfo.SetUserInfo(token);

            //所有提交的参数包写入日志 

            request.EnableBuffering();
            request.Body.Position = 0;
            var requestReader = new StreamReader(request.Body);
            var requestContent = requestReader.ReadToEnd();
            request.Body.Position = 0;
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Operation",
                $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");
            string Content = $"======================================================\r\n操作人：{userId}\r\n请求方式：{request.Method}\r\n路由：{request.Path}\r\n数据包：{requestContent}\r\n时间：{DateTime.Now}";
            LogHelper.WriteCommLog(Content, filename);



        }

        private void checkType(Type methodReturnType)
        {
            if (!(methodReturnType.IsGenericType) || methodReturnType.GetGenericTypeDefinition() != typeof(Task<>)) { throw new Exception("返回对象必须为Task<ServiceMessage<T>>"); }
            var type = methodReturnType.GetGenericArguments()[0];
            if (!type.IsGenericType) throw new Exception("返回对象必须为Task<ServiceMessage<T>>");

            var isserver = type.GetGenericTypeDefinition() == typeof(ServiceMessage<>);
            if (!isserver)
                throw new Exception("返回对象必须为Task<ServiceMessage<T>>");
        }
    }


}
