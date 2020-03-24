using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BlockSms.Mobile.Core.Infrastructure;
using BlockSms.Mobile.Core.Infrastructure.AutofacModules;
using BlockSms.Core.DependencyInjection;
using BlockSms.Core.EntityFrameworkCore;
using BlockSms.Core.Extension;
using BlockSms.Core.Filters;
using BlockSms.Core.Helper;
using BlockSms.Core.Model;
using BlockSms.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace BlockSms.Mobile.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabaseServices(services, Configuration["ConnectionString"]);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
            services.AddAutoMapper();
            services.AddCors();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WebApiResultMiddleware));
                options.RespectBrowserAcceptHeader = true;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<ApiOptions>(Configuration)
                .AddCustomSwagger(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var container = new ContainerBuilder();
            container.Populate(services);
            container.RegisterType<LogFilter>().PropertiesAutowired();
            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseErrorHandling();
            app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            app.UseMvc();

            if (Configuration["Consul"] != null)
            {
                app.RegisterConsul(lifetime, new ServiceEntity
                {
                    IP = NetworkHelper.LocalIPAddress,
                    Port = Convert.ToInt32(Configuration["Port"]),
                    ServiceName = Configuration["Name"],
                    ConsulIP = Configuration["Consul:IP"],
                    ConsulPort = Convert.ToInt32(Configuration["Consul:Port"])
                });
            }
            app.UseSwagger(c => { c.RouteTemplate = "doc/{documentName}/swagger.json"; })
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"/doc/{Configuration["Name"]}/swagger.json", $"{Configuration["Name"]} {Configuration["Version"]}");
               });
        }

        private static void ConfigureDatabaseServices(IServiceCollection services, string connectionString)
        {
            services.Configure<DbConnectionOptions>(o => o.ConnectionStrings.Default = connectionString);

            services.Configure<EPTDbContextOptions>(options =>
            {
                options.PreConfigure(dbContextConfigurationContext =>
                {
                    dbContextConfigurationContext.DbContextOptions
                        .ConfigureWarnings(w => w.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
                });
                options.UseSqlServer();
            });
            services.AddEPTDbContext<MobileContext>(o => o.AddDefaultRepositories(true));
            services.AddAssemblyOf<IRepository>();
            services.AddAssemblyOf<MobileContext>();
            services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        }
    }
}
