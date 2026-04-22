using CDGService.Application.DataCollect;
using CDGService.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDGService.Application.Service.Net
{
    public static class DiHelper
    {

        /// <summary>
        /// 采集 加入数据存储的
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebDataReceive(this IServiceCollection service)
        {
           
            service.AddScoped<WebDataUrl>();

            service.AddScoped<IDataReceive, WebServerData>();
            service.AddScoped<CollentStartup>(); 
            //采购申请
            service.AddScoped<IPurchasManagerService, PurchasManagerService>();

            //service.AddScoped<CollectService.PatientRecords>();

            service.AddScoped<BisStatistical.IndexesMonthStatistical>();
            service.AddScoped<BisStatistical.IndexesYearStatistical>();
            return service;
        }
    }
}
