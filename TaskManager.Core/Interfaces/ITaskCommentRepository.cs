using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la entidad <see cref="TaskComment"/>.
    /// </summary>
    public interface ITaskCommentRepository : IBaseRepository<TaskComment>
    {
        Task<IEnumerable<TaskComment>> GetAllCommentsDapperAsync();
        Task<TaskComment?> GetCommentByIdDapperAsync(int commentId);
        Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId);
    }
}
