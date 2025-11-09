namespace TaskManager.Infrastructure.Queries
{
    public static class TaskCommentQueries
    {
        // Todos los comentarios
        public static string GetAllCommentsSqlServer = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            ORDER BY Id ASC;
        ";

        public static string GetCommentByIdSqlServer = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            WHERE Id = @CommentId;
        ";

        public static string GetCommentsByTaskIdSqlServer = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            WHERE TaskId = @TaskId
            ORDER BY CreatedAt ASC;
        ";

        // MySQL
        public static string GetAllCommentsMySql = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            ORDER BY Id ASC;
        ";

        public static string GetCommentByIdMySql = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            WHERE Id = @CommentId;
        ";

        public static string GetCommentsByTaskIdMySql = @"
            SELECT Id, TaskId, UserId, Comment, CreatedAt 
            FROM TaskComment
            WHERE TaskId = @TaskId
            ORDER BY CreatedAt ASC;
        ";
    }
}
