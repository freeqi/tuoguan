using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using CDGService.Data.Datas;
using CDGService.WebAPI.Controllers;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Dto;
using CDGService.Data.Helper;

namespace CDGService.WebAPI.Extenstions
{

    /// <summary>
    /// 实现此接口，会自动注入为InstancePerLifetimeScope
    /// </summary>
    public interface IDependency { }


    public static class DiExtension
    {

        public static void AddDis(this ContainerBuilder builder)
        {
            builder.RegisterType<ServiceMessageTryCatchInterceptor>();
            builder.RegisterType<LogRecordInterceptor>();

            builder.RegisterType<UserManager>().AsSelf();

            builder.RegisterType<GetUserInfo>().As<IGetUserInfo>().InstancePerLifetimeScope();

            builder.RegisterType<LogManager>().AsSelf();
            builder.RegisterType<CenterDialysisManger>().AsSelf();
            builder.RegisterType<EmployeeManger>().AsSelf();
            builder.RegisterType<DictionaryTypeManager>().AsSelf();
            builder.RegisterType<SystemDictionaryManger>().AsSelf();
            builder.RegisterType<RoleManger>().AsSelf();
            builder.RegisterType<DataManager>().AsSelf();
            builder.RegisterType<DocumentManager>().AsSelf();

            builder.RegisterType<PatientsManager>().AsSelf();

            builder.RegisterType<EquipmentManager>().AsSelf();
            builder.RegisterType<MaintenanceRecordManger>().AsSelf();

            builder.RegisterType<MenuManager>().AsSelf();

            builder.RegisterType<CenterDockingManger>().AsSelf();
            //
            builder.RegisterType<HospitalFeelingManager>().AsSelf();

            builder.RegisterType<Application.BisStatistical.PatientStatistical>().AsSelf();

            builder.RegisterType<QualityControlManager>().AsSelf();
            builder.RegisterType<ResultControlManager>().AsSelf();


            builder.RegisterType<MedicalIndexesManger>().AsSelf();
            builder.RegisterType<BusinessTargetStatisticalManager>().AsSelf();

            builder.RegisterType<MaterialsStatisticaManager>().AsSelf();

            builder.RegisterType<MedicalRcordManager>().AsSelf();
            builder.RegisterType<WaterFuelManager>().AsSelf();

            builder.RegisterType<InformationManager>().AsSelf();


            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .Where(t => (t.IsSubclassOf(typeof(Controller)) || t.IsSubclassOf(typeof(ControllerBase))) && !t.IsAbstract)
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ServiceMessageTryCatchInterceptor))
                .InterceptedBy(typeof(LogRecordInterceptor));

            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                       .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract)
                       .AsSelf()
                       .InstancePerLifetimeScope();
        }
    }
}