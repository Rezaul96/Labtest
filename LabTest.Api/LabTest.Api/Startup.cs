using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabTest.Api.Helpers;
using LabTest.Repository.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LabTest.Api
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

            services.AddMvc(a =>
            {
                a.Filters.Add<GlobalExceptionFilter>();
            }).AddLabTestJsonOptions();

            services.AddSingleton(Configuration);

            services.AddResponseCompression();
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Enable cross-origin requests 
            services.AddCors(options =>
            {
                options.AddPolicy("Admin", builder => { builder.WithOrigins("https://localhost:44363", "https://localhost:44376").AllowAnyHeader().AllowAnyMethod(); });
            });
            services.AddDbContext<LabTestDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<LabTestDbContext>()
              .AddDefaultTokenProviders();

            services.AddReposotories();

            
            //services.AddApiVersioning(options =>
            //{
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = ApiVersion.Default;
            //    options.ApiVersionReader = new HeaderApiVersionReader("api-version");

            //});

            services.AddHttpContextAccessor();
            services.AddLabtestAuthotization(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           
           // loggerFactory.AddFile($"{env.ContentRootPath}/Logs/info-{{Date}}.txt").AddFile($"{env.ContentRootPath}/Logs/error-{{Date}}.txt", LogLevel.Error);

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/Content")
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();



            //Identity database init
            //var serviceProvider = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;
            //var context = serviceProvider.GetRequiredService<AppDBContext>();
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //context.Database.EnsureCreated();
            //var result = context.Users;
            //if (!context.Users.Any())
            //{
            //    var user = new ApplicationUser
            //    {
            //        Email = "admin@hateemtai.com",
            //        UserName = "admin@hateemtai.com",
            //        PhoneNumber = "01712443308",
            //        SecurityStamp = Guid.NewGuid().ToString()
            //    };
            //    userManager.CreateAsync(user, "@bC123dd");
            //}


            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseHsts();
            //}

            app.UseResponseCompression();          
            app.UseMvc();
        }
    }
}
