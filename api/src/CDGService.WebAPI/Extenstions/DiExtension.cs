using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
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

            //菜单 权限
            builder.RegisterType<MenuManager>().AsSelf();

            builder.RegisterType<CenterDockingManger>().AsSelf();
            //
            builder.RegisterType<HospitalFeelingManager>().AsSelf();

            builder.RegisterType<Application.BisStatistical.PatientStatistical>().AsSelf();
            //

            //builder.RegisterType<Application.APPConversionToPDF.ConversionMain>().SingleInstance();
            //质控
            builder.RegisterType<QualityControlManager>().AsSelf();
            builder.RegisterType<ResultControlManager>().AsSelf();


            builder.RegisterType<MedicalIndexesManger>().AsSelf();
            builder.RegisterType<BusinessTargetStatisticalManager>().AsSelf();

            builder.RegisterType<MaterialsStatisticaManager>().AsSelf();

            builder.RegisterType<MedicalRcordManager>().AsSelf();
            builder.RegisterType<WaterFuelManager>().AsSelf();

            builder.RegisterType<InformationManager>().AsSelf();


            //注入所有controller，并启用ServiceMessageTryCatchInterceptor
            var assembly = typeof(ServiceMessageTryCatchInterceptor).GetTypeInfo().Assembly;
            //var manager = new ApplicationPartManager();
            //manager.ApplicationParts.Add(new AssemblyPart(assembly));
            //manager.FeatureProviders.Add(new ControllerFeatureProvider());

            //var feature = new ControllerFeature();
            //manager.PopulateFeature(feature);
            //builder.RegisterType<ApplicationPartManager>().AsSelf().SingleInstance();
            //builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray())
            //    .EnableClassInterceptors().InterceptedBy(typeof(ServiceMessageTryCatchInterceptor))
            //    .InterceptedBy(typeof(LogRecordInterceptor));


            builder.RegisterAssemblyTypes(assembly)
                       .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract)
                       .AsSelf()
                       .InstancePerLifetimeScope();
        }
    }
}
