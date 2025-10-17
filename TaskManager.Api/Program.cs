using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

namespace TaskManager.Api
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
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            #endregion

            #region Inyección de dependencias
            // Servicios
            builder.Services.AddTransient<ITaskEntityService, TaskEntityService>();
            builder.Services.AddTransient<ITaskAssignmentService, TaskAssignmentService>();
            builder.Services.AddTransient<IUserService, UserService>();


            // Repositorios
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            
            #region Configuración de controladores y manejo JSON
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            #endregion

            #region Filtros personalizados
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

            builder.Services.AddScoped<IValidationService, ValidationService>();
            #endregion

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}