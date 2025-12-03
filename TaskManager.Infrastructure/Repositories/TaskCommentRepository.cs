using TaskManager.Core.Enum;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Queries;

namespace TaskManager.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio que gestiona las operaciones relacionadas con los comentarios de tareas (<see cref="TaskComment"/>).
    /// </summary>
    /// <remarks>
    /// Esta clase implementa <see cref="ITaskCommentRepository"/> y extiende de <see cref="BaseRepository{TaskComment}"/>.
    /// Se encarga de realizar operaciones directas con la base de datos utilizando Dapper
    /// para optimizar el acceso a los datos de los comentarios y usuarios relacionados.
    /// </remarks>
    public class TaskCommentRepository : BaseRepository<TaskComment>, ITaskCommentRepository
    {
        private readonly IDapperContext _dapper;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de comentarios con el contexto de datos y Dapper.
        /// </summary>
        /// <param name="context">Contexto de base de datos de Entity Framework Core.</param>
        /// <param name="dapper">Contexto de Dapper para ejecutar consultas SQL personalizadas.</param>
        public TaskCommentRepository(TaskManagerContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        /// <summary>
        /// Obtiene todos los comentarios de tareas utilizando Dapper.
        /// </summary>
        /// <remarks>
        /// Selecciona la consulta SQL adecuada según el proveedor de base de datos (SQL Server o MySQL).
        /// </remarks>
        /// <returns>Colección enumerable de objetos <see cref="TaskComment"/>.</returns>
        /// <exception cref="NotSupportedException">Si el proveedor de base de datos no es soportado.</exception>
        /// <exception cref="Exception">Si ocurre un error al ejecutar la consulta.</exception>
        public async Task<IEnumerable<TaskComment>> GetAllCommentsDapperAsync()
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskCommentQueries.GetAllCommentsSqlServer,
                    DatabaseProvider.MySql => TaskCommentQueries.GetAllCommentsMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<TaskComment>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener comentarios: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un comentario por su identificador incluyendo el usuario y la tarea asociada.
        /// </summary>
        /// <remarks>
        /// Ejecuta una consulta SQL específica para recuperar un comentario determinado.
        /// </remarks>
        /// <param name="commentId">Identificador del comentario a consultar.</param>
        /// <returns>
        /// Un objeto <see cref="TaskComment"/> con los datos completos, o <c>null</c> si no se encuentra.
        /// </returns>
        /// <exception cref="NotSupportedException">Si el proveedor de base de datos no es soportado.</exception>
        /// <exception cref="Exception">Si ocurre un error durante la consulta.</exception>
        public async Task<TaskComment?> GetCommentByIdDapperAsync(int commentId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskCommentQueries.GetCommentByIdSqlServer,
                    DatabaseProvider.MySql => TaskCommentQueries.GetCommentByIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<TaskComment>(sql, new { CommentId = commentId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el comentario con ID {commentId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene los comentarios de una tarea específica.
        /// </summary>
        /// <param name="taskId">Identificador de la tarea.</param>
        /// <returns>Colección enumerable de <see cref="TaskComment"/> asociados a la tarea.</returns>
        public async Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => TaskCommentQueries.GetCommentsByTaskIdSqlServer,
                    DatabaseProvider.MySql => TaskCommentQueries.GetCommentsByTaskIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<TaskComment>(sql, new { TaskId = taskId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener comentarios de la tarea con ID {taskId}: {ex.Message}");
            }
        }
    }
}
