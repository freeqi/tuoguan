using Microsoft.Extensions.DependencyInjection;
using CDGService.Data.Store;
using CDGService.Store.Store;

namespace CDGService.Store.Extensions
{
    public static class StoreHelper
    {
        /// <summary>
        /// 加入数据存储的
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddDbStore(this IServiceCollection service)
        { 
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddTransient<IDataFactory, DataFactory>() ;
            return service;
        }

    }
}
