using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Filters;
using TaskManager.Infrastructure.Mappings;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Validators;

namespace TaskManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Conguracion base
            builder.Configuration.Sources.Clear();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            //Configurar los secretos de usuario
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }
            //En Produccion los secretos vendran de Entornos globales

            #region Configurar la BD SqlServer
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<TaskManagerContext>(options => options.UseSqlServer(connectionString));
            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            
            // Inyectar las dependencias
            // Servicios
            builder.Services.AddTransient<ITaskEntityService, TaskEntityService>();
            builder.Services.AddTransient<ITaskAssignmentService, TaskAssignmentService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ITaskCommentService, TaskCommentService>();
            builder.Services.AddScoped<IValidationService, ValidationService>();
            builder.Services.AddSingleton<IPasswordService, PasswordService>();
            builder.Services.AddScoped<ISecurityServices, SecurityServices>();

            // Repositorios
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();
            builder.Services.AddTransient<ISecurityRepository, SecurityRepository>();

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Validaciones
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            builder.Services.Configure<PasswordOptions>
                (builder.Configuration.GetSection("PasswordOptions"));

            //Configuracion de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Backend Task Manager API",
                    Version = "v1",
                    Description = "Documentacion de la API de Task Manager - NET 9",
                    Contact = new()
                    {
                        Name = "Nahuel Jose Pairumani Saavedra",
                        Email = "nahuel.pairumani@ucb.edu.bo"
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });

            builder.Services.AddApiVersioning(options =>
            {
                // Reporta las versiones soportadas y obsoletas en encabezados de respuesta
                options.ReportApiVersions = true;

                // Version por defecto si no se especifica
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Soporta versionado mediante URL, Header o QueryString
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), 
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("api-version")
                );
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = builder.Configuration["Authentication:SecretKey"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    secretKey = builder.Configuration["Authentication_SecretKey"];
                }

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new Exception("El SecretKey de autenticación no está configurado. Verifique 'Authentication:SecretKey' o 'Authentication_SecretKey' en las variables de entorno.");
                }
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(secretKey)
                    )
                };
            });

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<TaskEntityDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

            var app = builder.Build();

            //Usar Swagger
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Task Manager API v1");
                    options.RoutePrefix = string.Empty;
                });
            //}

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}