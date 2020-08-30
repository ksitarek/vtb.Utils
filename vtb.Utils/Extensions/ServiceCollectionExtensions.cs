using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vtb.Utils.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVtbPrerequisites(this IServiceCollection services)
        {
            services.AddSingleton<ISystemClock>(new SystemClock());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITenantIdProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext != null)
                {
                    var userClaims = httpContext.User.Claims;
                    var tenantIdClaim = userClaims.First(x => x.Type == "tenant_id");
                    return new ValueTenantIdProvider(Guid.Parse(tenantIdClaim.Value));
                }
                else
                {
                    return new ValueTenantIdProvider();
                }
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secret)
        {
            Check.NotEmpty(secret, nameof(secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,

                        LifetimeValidator = (notBefore, expires, securityToken, tokenValidationParameters) =>
                        {
                            var utcNow = DateTime.UtcNow;
                            return notBefore <= utcNow
                                   && expires > utcNow;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddCorsPolicyWithOrigins(this IServiceCollection services, string[] origins)
        {
            Check.NotEmpty(origins, nameof(origins));

            return services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static IServiceCollection AddSwaggerGen(this IServiceCollection services, string title,
            bool requiresJwtAuth = true)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = title
                });

                if (requiresJwtAuth)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
                }
            });
        }
    }
}