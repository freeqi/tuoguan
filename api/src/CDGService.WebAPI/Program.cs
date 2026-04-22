using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CDGService.WebAPI.Extenstions;
using CDGService.Application.Service.Net;
using CDGService.Data.Helper;

namespace CDGService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exp)
            {
                CDGService.WebAPI.Extenstions.LogHelper.WriteErrLog($"未处理异常：{exp}{Environment.NewLine}StackTrace: {exp.StackTrace}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(option =>
                    {
                        option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
                        option.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(20);
                    })
                    .UseUrls("http://*:12345")//设置服务地址
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
                });

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception == null)
            {
                return;
            }
            CDGService.WebAPI.Extenstions.LogHelper.WriteErrLog($"未处理异常：{exception.Message}{Environment.NewLine}StackTrace: {exception.StackTrace}");
        }
    }
}