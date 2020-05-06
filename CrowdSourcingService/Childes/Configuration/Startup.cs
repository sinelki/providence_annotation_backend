using System;
using System.Data;
using System.Linq;
using Childes.Binders;
using Childes.Core;
using CrowdSourcing.Core;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Childes.Configuration
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IMvcCoreBuilder serviceBuilder = services.AddMvcCore();

            serviceBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            serviceBuilder.AddJsonFormatters().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            });
            serviceBuilder.AddFormatterMappings();

            services.AddTransient<IDbConnection, MySql.Data.MySqlClient.MySqlConnection>((serviceProvider) =>
            {
                string connectionString = this.Configuration.GetSection(
                    Childes.Configuration.Environment.SQLConnectionString
                ).Value;
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            });

            services.AddTransient<IDbDataAdapter, MySql.Data.MySqlClient.MySqlDataAdapter>();
            services.AddTransient<IDataProvider, DataProvider>((serviceProvider) =>
            {
                return new DataProvider(
                    serviceProvider.GetService<IDbConnection>(),
                    serviceProvider.GetService<IDbDataAdapter>());
            });

            services.AddTransient<IMediaStreamer, VideoStreamer>();
            services.AddTransient<ITaskGenerator, AnnotationTaskGenerator>();
            services.AddTransient<ITaskResolver, AnnotationTaskResolver>();
            services.AddTransient<IAnnotationService, AnnotationService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            services.AddMvcCore(options =>
            {
                var collectionBinder = options.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(BodyModelBinderProvider));
                if (collectionBinder == null)
                    return;

                options.ModelBinderProviders.Insert(
                    options.ModelBinderProviders.IndexOf(collectionBinder),
                    new AnnotationTaskCollectionModelBinderProvider());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors();

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=TasksController}/{action=Index}/{id?}");
            });
        }
    }
}
