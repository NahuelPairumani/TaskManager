namespace TaskManager.Infrastructure.Queries
{
    public static class TaskAssignmentQueries
    {
        // todos los registros
        public static string GetAllTaskAssignmentsSqlServer = @"
            SELECT Id, TaskId, UserId 
            FROM TaskAssignment
            ORDER BY Id ASC;
        ";

        public static string GetTaskAssignmentByIdSqlServer = @"
            SELECT Id, TaskId, UserId 
            FROM TaskAssignment
            WHERE Id = @Id;
        ";

        // por id
        public static string GetAllTaskAssignmentsMySql = @"
            SELECT Id, TaskId, UserId 
            FROM TaskAssignment
            ORDER BY Id ASC;
        ";

        public static string GetTaskAssignmentByIdMySql = @"
            SELECT Id, TaskId, UserId 
            FROM TaskAssignment
            WHERE Id = @Id;
        ";
    }
}
