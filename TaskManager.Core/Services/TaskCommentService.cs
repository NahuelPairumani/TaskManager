using System.Net;
using TaskManager.Core.CustomEntities;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.Services
{
    /// <summary>
    /// Servicio encargado de la lógica de negocio para la entidad <see cref="TaskComment"/>.
    /// </summary>
    public class TaskCommentService : ITaskCommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskCommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllCommentsDapperAsync(TaskCommentQueryFilter filters)
        {
            var comments = await _unitOfWork.TaskCommentRepository.GetAllCommentsDapperAsync();

            if (filters.TaskId != null)
                comments = comments.Where(c => c.TaskId == filters.TaskId);

            if (filters.UserId != null)
                comments = comments.Where(c => c.UserId == filters.UserId);

            if (!string.IsNullOrEmpty(filters.Comment))
                comments = comments.Where(c => c.Comment.ToLower().Contains(filters.Comment.ToLower()));

            var pagedComments = PagedList<object>.Create(comments, filters.PageNumber, filters.PageSize);

            if (pagedComments.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Comentarios recuperados correctamente." } },
                    Pagination = pagedComments,
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData()
            {
                Messages = new Message[] { new() { Type = "Warning", Description = "No se encontraron comentarios." } },
                Pagination = pagedComments,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public async Task<TaskComment> GetCommentByIdDapperAsync(int id)
        {
            var comment = await _unitOfWork.TaskCommentRepository.GetCommentByIdDapperAsync(id);
            if (comment == null)
                throw new BussinesException("El comentario especificado no existe.");

            return comment;
        }

        public async Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId)
        {
            var comments = await _unitOfWork.TaskCommentRepository.GetCommentsByTaskIdAsync(taskId);

            if (!comments.Any())
                throw new BussinesException($"No se encontraron comentarios para la tarea con ID {taskId}.");

            return comments;
        }

        public async Task InsertCommentAsync(TaskComment comment)
        {
            var task = await _unitOfWork.TaskEntityRepository.GetById(comment.TaskId);
            if (task == null)
                throw new BussinesException("La tarea asociada al comentario no existe.");

            var user = await _unitOfWork.UserRepository.GetById(comment.UserId);
            if (user == null)
                throw new BussinesException("El usuario asociado al comentario no existe.");

            comment.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.TaskCommentRepository.Add(comment);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task UpdateCommentAsync(TaskComment comment)
        {
            var existingComment = await _unitOfWork.TaskCommentRepository.GetById(comment.Id);
            if (existingComment == null)
                throw new BussinesException("No se puede actualizar un comentario inexistente.");

            existingComment.Comment = comment.Comment;

            await _unitOfWork.TaskCommentRepository.Update(existingComment);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _unitOfWork.TaskCommentRepository.GetById(id);
            if (comment == null)
                throw new BussinesException("El comentario no existe.");

            await _unitOfWork.TaskCommentRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
