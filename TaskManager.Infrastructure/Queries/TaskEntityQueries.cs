namespace TaskManager.Infrastructure.Queries
{
    /// <summary>
    /// Contiene las consultas SQL utilizadas por Dapper para la entidad <see cref="TaskEntity"/>.
    /// </summary>
    /// <remarks>
    /// Las consultas están separadas por proveedor de base de datos (SQL Server y MySQL)
    /// y son utilizadas por <see cref="TaskEntityRepository"/> para recuperar información
    /// de manera eficiente y tipada.
    /// </remarks>
    public static class TaskQueries
    {
        /// <summary>
        /// Consulta SQL para obtener todas las tareas en SQL Server.
        /// </summary>
        public static string GetAllTasksSqlServer = @"
            SELECT 
                T.Id,
                T.projectId,
                T.StatusId,
                T.Title,
                T.Description,
                T.DueDate
            FROM TaskEntity T;
        ";

        /// <summary>
        /// Consulta SQL para obtener todas las tareas en MySQL.
        /// </summary>
        public static string GetAllTasksMySql = @"
            SELECT 
                T.Id,
                T.projectId,
                T.StatusId,
                T.Title,
                T.Description,
                T.DueDate
            FROM TaskEntity T;
        ";

        /// <summary>
        /// Consulta SQL (SQL Server) que obtiene una tarea por su Id incluyendo el nombre del proyecto y el estado.
        /// </summary>
        public static string GetTaskByIdSqlServer = @"
            SELECT 
                T.Id,
                T.Title,
                T.Description,
                T.DueDate,
                S.StatusName,
                P.Name AS ProjectName
            FROM TaskEntity T
            INNER JOIN Status S ON T.StatusId = S.Id
            INNER JOIN Project P ON T.ProjectId = P.Id
            WHERE T.Id = @TaskId;
        ";

        /// <summary>
        /// Consulta SQL (MySQL) que obtiene una tarea por su Id incluyendo el nombre del proyecto y el estado.
        /// </summary>
        public static string GetTaskByIdMySql = @"
            SELECT 
                T.Id,
                T.Title,
                T.Description,
                T.DueDate,
                S.StatusName,
                P.Name AS ProjectName
            FROM TaskEntity T
            INNER JOIN Status S ON T.StatusId = S.Id
            INNER JOIN Project P ON T.ProjectId = P.Id
            WHERE T.Id = @TaskId;
        ";

        /// <summary>
        /// Consulta SQL (SQL Server) que obtiene el conteo de tareas agrupadas por estado en un proyecto.
        /// </summary>
        public static string CountTasksByStatusSqlServer = @"
            SELECT 
                S.StatusName,
                COUNT(*) AS TotalTasks
            FROM [TaskEntity] T
            INNER JOIN [Status] S ON T.StatusId = S.Id
            WHERE T.ProjectId = @ProjectId
            GROUP BY S.StatusName;
        ";

        /// <summary>
        /// Consulta SQL (MySQL) que obtiene el conteo de tareas agrupadas por estado en un proyecto.
        /// </summary>
        public static string CountTasksByStatusMySql = @"
            SELECT 
                S.StatusName,
                COUNT(*) AS TotalTasks
            FROM TaskEntity T
            INNER JOIN Status S ON T.StatusId = S.Id
            WHERE T.ProjectId = @ProjectId
            GROUP BY S.StatusName;
        ";
        public static string GetUsersByProjectSqlServer = @"
            SELECT 
                U.Id,
                U.FirstName,
                U.LastName,
                U.Email,
                COUNT(DISTINCT T.Id) AS TaskCount,
                P.Name AS ProjectName
            FROM [User] U
            INNER JOIN TaskAssignment TA ON U.Id = TA.UserId
            INNER JOIN TaskEntity T ON TA.TaskId = T.Id
            INNER JOIN Project P ON T.ProjectId = P.Id
            WHERE T.ProjectId = @ProjectId
            GROUP BY U.Id, U.FirstName, U.LastName, U.Email, P.Name;
        ";

        public static string GetUsersByProjectMySql = @"
            
        ";
    }
}
