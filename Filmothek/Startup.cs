using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Filmothek.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmothek
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //adds JWT auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //all of the follwing need to be fulfilled for the token to be valid
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //Issuer is one of the listed below
                    ValidateAudience = true, //receiver is one of the listed below
                    ValidateLifetime = true, //hasnt expired
                    ValidateIssuerSigningKey = true, //actual key is valid... kind of the whole point

                    ValidIssuer = "http://localhost:50000",
                    ValidAudience = "http://localhost:4200",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
                };
            });

            //enabling of EFC /w SQL DB
            services.AddDbContext<VideoContext>(opt =>
              opt.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Filmothek;Trusted_Connection=True;ConnectRetryCount=0")
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //CORS is more a 'guideline' than a rule
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

            //required for JWT auth
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}

