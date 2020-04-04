using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
using System.Reflection;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using DynamicEdmModelCreation.Extensions;

namespace ODataTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOData();
            services.AddODataQueryFilter();
            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(b =>
            {
                b.CustomMapODataServiceRoute("odata", "odata/{dataSource}");
            });
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsEnum && t.Namespace == "ODataTest"
                    select t;

            foreach (var item in q)
            {
                builder.EntitySet<EnumTable>($"{item.Name}s");
            }

            return builder.GetEdmModel();
        }
    }
}
