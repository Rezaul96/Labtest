using LabTest.Repository.Registration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Api.Helpers
{
    public static class StartupExtensions
    {
        public static IMvcBuilder AddLabTestJsonOptions(this IMvcBuilder builder)
        {
            return builder.AddJsonOptions(a =>
            {
                //a.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                a.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                a.SerializerSettings.Formatting = Formatting.None;
                a.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                a.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //a.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                a.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                a.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                a.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }

        public static IServiceCollection AddReposotories(this IServiceCollection services)
        {
            return services.AddScoped<IRegistrationRepository, RegistrationRepository>();
              
        }

        public static IServiceCollection AddLabtestAuthotization(this IServiceCollection services, IConfiguration configuration)
        {
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
                    var key = Convert.FromBase64String(configuration["AppSettings:Key"]);
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
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };

                });

            services.ConfigureApplicationCookie(a => a.LoginPath = new PathString("/Home/Login"));

            return services;
        }
    }
}
