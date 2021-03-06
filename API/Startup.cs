using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ordering of calling these function from services is not neccessary in this function 'ConfigureServices'

            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddCors();
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ordering of calling these function from app is very important in this function 'Configure'
            // whichever order we haved called the below functions in should be in this same order 

            if (env.IsDevelopment())
            {
                // if application encounters problems then we use the developer exception page
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            // if we did come in on a HTTP  address then we get redirected to HTTPS endpoints
            app.UseHttpsRedirection();

            // It routing action because we were able to go from our browser, the WeatherForecast endpoint and get to our WeatherForecast controller
            app.UseRouting();

            // here x is defined as cors policy
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

            // Added this to enable authencating our http endpoints
            app.UseAuthentication();

            app.UseAuthorization();

            // This is middlerware to actually use the endpoints
            app.UseEndpoints(endpoints =>
            {
                // to Map the controller. This takes a look inside our controllers to see what endpoints are available
                endpoints.MapControllers();
            });
        }
    }
}
