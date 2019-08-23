using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LabTest.Web.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LabTest.Web
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
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //Configuration.GetSection("AppSettings").Bind(WebConfiguration.Instance, options => options.BindNonPublicProperties = true);
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMemoryCache();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            Configuration.GetSection("AppSettings").Bind(WebConfiguration.Instance, options => options.BindNonPublicProperties = true);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    var key = Convert.FromBase64String(WebConfiguration.Instance.Key);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = "http://oec.com",
                        ValidIssuer = "http://oec.com",
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Debug.WriteLine("Failed");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole("User")
                    .Build();

                //options.Filters.Add(new HateemtaiAuthorizationFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddFile($"{env.ContentRootPath}/Logs/info-{{Date}}.txt").AddFile($"{env.ContentRootPath}/Logs/error-{{Date}}.txt", LogLevel.Error);

            app.Use(async (ctx, next) =>
            {
                if (string.IsNullOrEmpty(ctx.Request.Headers["Authorization"]))
                {
                    if (ctx.Request.Cookies.ContainsKey(ConfigKeys.AuthCookieKey))
                    {
                        ctx.Request.Headers["Authorization"] = $"Bearer {ctx.Request.Cookies[ConfigKeys.AuthCookieKey]}";
                    }
                }
                await next.Invoke();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseStatusCodePages(context =>
            {
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    context.HttpContext.Response.Redirect("/Home/Login");
                }

                return Task.CompletedTask;
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
