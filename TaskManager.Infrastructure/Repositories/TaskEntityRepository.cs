using TaskManager.Core.Enum;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Queries;

namespace TaskManager.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio que gestiona las operaciones relacionadas con las tareas (<see cref="TaskEntity"/>).
    /// </summary>
    /// <remarks>
    /// Esta clase implementa <see cref="ITaskEntityRepository"/> y extiende de <see cref="BaseRepository{TaskEntity}"/>.
    /// 
    /// Se encarga de realizar operaciones directas con la base de datos utilizando Dapper
    /// para optimizar el acceso a los datos de las tareas, proyectos y estados asociados.
    /// </remarks>
    public class TaskEntityRepository : BaseRepository<TaskEntity>, ITaskEntityRepository
    {
        private readonly IDapperContext _dapper;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de tareas con el contexto de datos y Dapper.
        /// </summary>
        /// <param name="context">Contexto de base de datos de Entity Framework Core.</param>
        /// <param name="dapper">Contexto de Dapper para ejecutar consultas SQL personalizadas.</param>
        public TaskEntityRepository(TaskManagerContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        /*
        public async Task<IEnumerable<TaskEntity>> Find(Expression<Func<TaskEntity, bool>> predicate)
        {
            var taskEntities = await _entities.Where(predicate).ToListAsync();
            return taskEntities;
        }
        */

        /// <summary>
        /// Obtiene todas las tareas de la base de datos utilizando Dapper.
        /// </summary>
        /// <remarks>
        /// Este método selecciona la consulta SQL adecuada según el proveedor de base de datos 
        /// (SQL Server o MySQL) definido en la configuración.
        /// </remarks>
        /// <returns>Colección enumerable de objetos <see cref="TaskEntity"/>.</returns>
        /// <exception cref="NotSupportedException">Se lanza si el proveedor de base de datos no es compatible.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error al ejecutar la consulta.</exception>
        public async Task<IEnumerable<TaskEntity>> GetAllTasksDapperAsync()
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskQueries.GetAllTasksSqlServer,
                    DatabaseProvider.MySql => TaskQueries.GetAllTasksMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<TaskEntity>(sql);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Obtiene una tarea por su identificador incluyendo el nombre del proyecto y del estado.
        /// </summary>
        /// <remarks>
        /// Ejecuta una consulta SQL específica para recuperar los datos de una tarea determinada,
        /// junto con los nombres descriptivos del estado y proyecto relacionados.
        /// </remarks>
        /// <param name="taskId">Identificador de la tarea a consultar.</param>
        /// <returns>
        /// Un objeto <see cref="TaskDetailResponse"/> con los datos de la tarea,
        /// el nombre del estado y del proyecto, o <c>null</c> si no se encuentra.
        /// </returns>
        /// <exception cref="NotSupportedException">Si el proveedor de base de datos no es soportado.</exception>
        /// <exception cref="Exception">Si ocurre un error durante la consulta.</exception>
        public async Task<TaskDetailResponse?> GetTaskByIdDapperAsync(int taskId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskQueries.GetTaskByIdSqlServer,
                    DatabaseProvider.MySql => TaskQueries.GetTaskByIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<TaskDetailResponse>(sql, new { TaskId = taskId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la tarea con ID {taskId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el conteo de tareas agrupadas por estado dentro de un proyecto específico.
        /// </summary>
        /// <remarks>
        /// Devuelve una colección de objetos que indican cuántas tareas hay en cada estado
        /// (por ejemplo: "Pendiente", "En progreso", "Completada") para el proyecto especificado.
        /// </remarks>
        /// <param name="projectId">Identificador del proyecto.</param>
        /// <returns>Colección de <see cref="TaskStatusCountResponse"/> con el conteo por estado.</returns>
        /// <exception cref="NotSupportedException">Si el proveedor de base de datos no es soportado.</exception>
        /// <exception cref="Exception">Si ocurre un error durante la consulta SQL.</exception>
        public async Task<IEnumerable<TaskStatusCountResponse>> GetTaskCountByStatusAsync(int projectId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskQueries.CountTasksByStatusSqlServer,
                    DatabaseProvider.MySql => TaskQueries.CountTasksByStatusMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                var parameters = new { ProjectId = projectId };

                return await _dapper.QueryAsync<TaskStatusCountResponse>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener conteo de tareas por estado: {ex.Message}");
            }
        }

        public async Task<IEnumerable<UserTaskCountResponse>> GetUsersByProjectDapperAsync(int projectId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskQueries.GetUsersByProjectSqlServer,
                    DatabaseProvider.MySql => TaskQueries.GetUsersByProjectMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<UserTaskCountResponse>(sql, new { ProjectId = projectId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuarios por proyecto: {ex.Message}");
            }
        }
    }
}
