using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Database.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using Database.Repository.Generic;
using Microsoft.Net.Http.Headers;
using API.Hypermedia.Filters;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Rewrite;
using API.Services;
using API.Services.Implementations;
using API.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using API.Business.Implementations;
using API.Business;
using Database.Model.Context;
using API.Hypermedia.Enricher;
using API.Middleware;
using API.Extension;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                    Configuration.GetSection("TokenConfigurations")
                )
                .Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "consultingapi");
                });
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = tokenConfigurations.Issuer,
            //        ValidAudience = tokenConfigurations.Audience,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
            //    };
            //});

            //services.AddAuthorization(auth =>
            //{
            //    auth.AddPolicy("Bearer",
            //        new AuthorizationPolicyBuilder()
            //        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireAuthenticatedUser().Build());
            //});

            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services
                .AddControllers();
                //.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
                //.AddNewtonsoftJson(options =>
                //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                //);

            //IDBConnection dbCon = new MySqlDBConnection();
            var connection = Configuration["DbConnection:ConnectionString"];
            services.AddDbContext<DatabaseContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            //services.AddScoped(x => new DatabaseContext(connection));

            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                //options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
            });
            //.AddXmlSerializerFormatters();

            var filterOptions = new HyperMediaFilterOptions();
            filterOptions.ContentResponseEnricherList.Add(new StepEnricher());
            //filterOptions.ContentResponseEnricherList.Add(new BookEnricher());

            services.AddSingleton(filterOptions);

            services.AddApiVersioning();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1.0",
                    new OpenApiInfo
                    {
                        Title = "Consulting API",
                        Version = "v1.0",
                        Description = "API Restful for consultants to make online appointments",
                        Contact = new OpenApiContact
                        {
                            Name = "Giovani Espindola",
                            Url = new Uri("mailto:giovani.milanez@gmail.com")
                        }
                    });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5001/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"consultingapi", "Consulting API"}
                            }                            
                        }
                    }
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // add current logged in user
            services.AddScoped(x => x.GetService<IHttpContextAccessor>().HttpContext.GetLoggedInUser());

            //services.AddScoped<ILoginBusiness, LoginBusinessImplementation>();
            services.AddScoped<IFileBusiness, FileBusinessImplementation>();
            services.AddScoped<IStepBusiness, StepBusinessImplementation>();
            services.AddScoped<IServiceBusiness, ServiceBusinessImplementation>();
            services.AddScoped<IAppointmentBusiness, AppointmentBusinessImplementation>();

            services.AddTransient<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStepRepository, StepRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
            }
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json",
                    "API Restful for consultants to make online appointments");
                c.OAuthClientId("swagger");
                c.OAuthAppName("Consulting API");
                c.OAuthUsePkce();
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");
                endpoints.MapControllers().RequireAuthorization("ApiScope");
            });
        }

        //private void MigrateDatabase(string connection)
        //{
        //    try
        //    {
        //        var evolveConnection = new MySqlConnection(connection);
        //        var evolve = new Evolve.Evolve(evolveConnection, msg => Log.Information(msg))
        //        {
        //            Locations = new List<string> { "db/migrations", "db/dataset" },
        //            IsEraseDisabled = true
        //        };
        //        evolve.Migrate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Database migraation failed", ex);
        //    }
        //}
    }
}
