using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ReMIND.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using ReMIND.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ReMIND.Server
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
            services.AddDbContext<ReMindContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DatabaseCS"));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReMIND.Server", Version = "v1" });
                c.OperationFilter<CustomHeaderSwaggerAttribute>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CORS", builder =>
                {
                    builder.AllowAnyOrigin()
                    //builder.WithOrigins(new string[]
                    //{
                    //   "http://localhost:5001",
                    //   "https://localhost:5001",
                    //   "http://127.0.0.1:5001",
                    //   "https://127.0.0.1:5001",
                    //   "http://127.0.0.1:5001",
                       
                    //   "http://localhost:5000",
                    //   "https://localhost:5000",
                    //   "http://127.0.0.1:5000",
                    //   "https://127.0.0.1:5000",
                    //   "http://127.0.0.1:5000",

                    //   "ws://localhost:5000",
                    //   "ws://localhost:5000",
                    //   "ws://127.0.0.1:5000",
                    //   "ws://127.0.0.1:5000",
                    //   "ws://127.0.0.1:5000"
                    //})
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddScoped<Utility, Utility>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReMIND.Server v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("CORS");

            app.UseMiddleware<Auth>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}
