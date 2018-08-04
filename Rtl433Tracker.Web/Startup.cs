using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rtl433Tracker.Data;
using Rtl433Tracker.Services;
using Rtl433Tracker.Services.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace Rtl433Tracker.Web
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
            services.AddMvc();

            services.AddDbContext<Rtl433TrackerContext>(options => options.UseSqlite("Data Source=Rtl433Tracker.db"));

            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IEventService, EventService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "rtl_433 Tracker API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "rtl_433 Tracker API V1");
            });

            app.UseBlazor<Client.Program>();
        }
    }
}
