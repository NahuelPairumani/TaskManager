namespace TaskManager.Infrastructure.Queries
{
    public static class UserQueries
    {
        /// <summary>
        /// Consulta SQL para obtener todos los usuarios en SQL Server.
        /// </summary>
        public static string GetAllUsersSqlServer = @"
                SELECT 
                    Id,
                    FirstName,
                    LastName,
                    Email
                FROM [User]
                ORDER BY FirstName ASC;
            ";

        /// <summary>
        /// Consulta SQL para obtener todos los usuarios en MySQL.
        /// </summary>
        public static string GetAllUsersMySql = @"
                SELECT 
                    Id,
                    FirstName,
                    LastName,
                    Email
                FROM User
                ORDER BY FirstName ASC;
            ";

        /// <summary>
        /// Consulta SQL para obtener un usuario por su Id en SQL Server.
        /// </summary>
        public static string GetUserByIdSqlServer = @"
            SELECT 
                Id,
                FirstName,
                LastName,
                Email
            FROM [User]
            WHERE Id = @Id;
        ";

        /// <summary>
        /// Consulta SQL para obtener un usuario por su Id en MySQL.
        /// </summary>
        public static string GetUserByIdMySql = @"
            SELECT 
                Id,
                FirstName,
                LastName,
                Email
            FROM User
            WHERE Id = @Id;
        ";
    }
}
