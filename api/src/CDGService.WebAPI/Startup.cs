using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CDGService.Store;
using CDGService.Store.Extensions;
using CDGService.WebAPI.Controllers;
using CDGService.WebAPI.DataCore;
using CDGService.WebAPI.Extenstions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using CDGService.Data;
using CDGService.Application.Service.Net;
using System.Data;
using System.Linq;
using CDGService.Utils;

namespace CDGService.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("服务启动中……");

            string ConnString = Configuration.GetConnectionString("sqlserver");

            //配置数据库
            MsSqlHelper._ConnString = ConnString;
            services.AddDbContext<AppDb>(t =>
                t.UseSqlServer(ConnString,
                b => b.MigrationsAssembly("CDGService.WebAPI")));

            services.AddOptions()
                .Configure<ApplicationSetting>(Configuration.GetSection("ApplicationSetting"));
            services.AddOptions().Configure<CenterApiUrl>(Configuration.GetSection("CenterApiUrl"));
            services.AddOptions().Configure<Data.DocumentSetting>(Configuration.GetSection("DocumentSetting"));
            services.AddOptions().Configure<Data.DictionaryCode>(Configuration.GetSection("DictionaryCode"));
            services.AddOptions().Configure<System.Collections.Generic.List<CenterUser>>(Configuration.GetSection("CenterUser"));
            services.AddOptions().Configure<System.Collections.Generic.List<ApprovalProcess>>(Configuration.GetSection("ApprovalProcess"));
            services.AddOptions().Configure<System.Collections.Generic.List<PurchSubmit>>(Configuration.GetSection("PurchSubmit"));
            services.AddOptions().Configure<System.Collections.Generic.List<PurchApproval>>(Configuration.GetSection("PurchApproval"));
            services.AddOptions().Configure<System.Collections.Generic.List<OrderPricingRole>>(Configuration.GetSection("OrderPricingRole"));
            services.AddOptions().Configure<System.Collections.Generic.List<BusinessTarget>>(Configuration.GetSection("BusinessTarget"));
            
            //配置日志
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
            });
            
            services.AddDbStore();
            services.AddWebDataReceive();
            
            #region 配置跨域处理 
            services.AddCors(options =>
            {
                options.AddPolicy("any", builders =>
                {
                    builders.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            #endregion

            var basePath = AppContext.BaseDirectory;
            var xmlPath = Path.Combine(basePath, "CDGService.WebAPI.xml");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "CDGService API",
                    Version = "v1",
                    Description = "血透中心集团信息管理服务接口",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "SWS",
                        Email = "ran.guangdi@swskj.com",
                        Url = new Uri("https://example.com")
                    }
                });
                c.IncludeXmlComments(xmlPath);
            });

            //配置AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));
            
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddDis();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            using (var scope = app.ApplicationServices.CreateScope())
            {
                var appdb = scope.ServiceProvider.GetService<AppDb>();
                appdb.Database.Migrate();
                appdb.InitEmployeeInfo();
                appdb.InitUserInfo();
                appdb.ClearExpireTokens();
                
                // 初始化静态Mapper
                var mapper = scope.ServiceProvider.GetService<AutoMapper.IMapper>();
                CDGService.WebAPI.Extenstions.AutoMapperHelper.Initialize(mapper);
            }

            app.UseRouting();
            app.UseCors("any");
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CDGService API V1");
                c.RoutePrefix = "help";
            });

            //JSON全局设置 
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                return settings;
            };

            ConfigLog(loggerFactory);

            bool IsCollent = Convert.ToBoolean(Configuration.GetConnectionString("IsCollent"));
            
            //采集
            Task.Run(async () =>
            {
                while (IsCollent)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000 * 60 * 60 * 1);
                        while (DateTime.Now.Hour < 2)
                        {
                            System.Threading.Thread.Sleep(1000 * 60 * 60 * 1);
                        }
                        using (var scope = app.ApplicationServices.CreateScope())
                        {
                            var CollentData = scope.ServiceProvider.GetService<Application.DataCollect.CollentStartup>();
                            await CollentData.StartSyncCollentData();
                        }
                    }
                    catch (Exception exp)
                    {
                        CDGService.Utils.FileHelper.WriteLog("", $"同步数据失败，等待下次同步:{exp}");
                    }
                    finally
                    {
                        System.Threading.Thread.Sleep(1000 * 60 * 60 * 1);
                    }
                }
            });

            //每日自动检测职工预调动
            Task.Run(async () =>
            {
                int first = 1;
                while (true)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000 * 60 * 60 * 1);

                        if (DateTime.Now.Hour == 3)
                            first = 1;

                        if (DateTime.Now.Hour == 4 && first == 1)
                        {
                            using (var scope = app.ApplicationServices.CreateScope())
                            {
                                var EmployeeManger = scope.ServiceProvider.GetService<EmployeeManger>();
                                await EmployeeManger.AutoEmpTransferAsync();
                                string filename = Path.Combine(Directory.GetCurrentDirectory(), "Logs/Common",
                                    $"CDGServiceWebApi{DateTime.Now.ToString("yyyyMMdd")}.log");
                                CDGService.Utils.FileHelper.WriteLog(filename, $"完成职工调动"); 
                                var DockingManger = scope.ServiceProvider.GetService<CenterDockingManger>();
                                await DockingManger.UpdateMedicalDrugExtensions();
                                await DockingManger.GetGoodsInStockdInfo();

                                first = 2;
                            }
                        }

                    }
                    catch (Exception exp)
                    {
                        CDGService.Utils.FileHelper.WriteLog("", $"员工调动出错:{exp}");
                    }
                    finally
                    {
                    }
                }
            });
        }

        private static void ConfigLog(ILoggerFactory loggerFactory)
        {
            var fileloggerpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            //日志过滤逻辑
            Func<string, LogLevel, LogLevel, bool> logFilter = (content, level, minLevel) =>
            {
                if (!string.IsNullOrEmpty(content) &&
                    content.StartsWith("Microsoft.EntityFrameworkCore"))
                {
                    return false;
                }
                if (level < minLevel)
                {
                    return false;
                }
                return true;
            };

            // 在.NET 6中，日志配置已移至ConfigureServices
        }
    }
}
