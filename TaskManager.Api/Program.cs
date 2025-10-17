using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Filters;
using TaskManager.Infrastructure.Mappings;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Validators;

/*
Scaffold-DbContext "Server=DESKTOP-MI7GISC;Database=TaskManagerDB;Trusted_Connection=true;TrustServerCertificate=true;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data -Context "TaskManagerContext" -Force
*/

namespace TaskManager.Api // usamos namespace para organizar el codigo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD SqlServer
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<TaskManagerContext>(options => options.UseSqlServer(connectionString));
            #endregion

            #region AutoMapper
            // Mapea entre entidades (TaskEntity) y DTOs (TaskDto)
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            #endregion

            #region Inyección de dependencias
            // Servicios (Capa de lógica de negocio)
            builder.Services.AddTransient<ITaskEntityService, TaskEntityService>();
            builder.Services.AddTransient<ITaskAssignmentService, TaskAssignmentService>();
            builder.Services.AddTransient<IUserService, UserService>();


            // Repositorios (Capa de acceso a datos)
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            
            #region Configuración de controladores y manejo JSON
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                // Evita referencias circulares en entidades relacionadas
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Evita que ASP.NET rechace modelos inválidos automáticamente
                // para poder controlarlo manualmente con nuestros filtros
                options.SuppressModelStateInvalidFilter = true;
            });
            #endregion

            #region Filtros personalizados
            // Aplica el filtro de validación a todos los controladores
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
            #endregion

            #region FluentValidation
            // Registra los validadores de DTOs
            builder.Services.AddValidatorsFromAssemblyContaining<TaskEntityDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();
            #endregion

            // Servicio genérico de validación
            builder.Services.AddScoped<IValidationService, ValidationService>();
            #endregion

            var app = builder.Build();

            #region Configuración del pipeline HTTP
            // Redirige a HTTPS
            app.UseHttpsRedirection();

            // Autorización básica (si se agrega autenticación)
            app.UseAuthorization();

            // Mapeo de controladores (rutas API)
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}