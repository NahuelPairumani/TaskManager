using TaskManager.Core.CustomEntities;

namespace TaskManager.Api.Responses
{
    public class ApiResponse<T>
    {
        public Message[] Messages { get; set; }
        public T Data { get; set; }
        public Pagination Pagination { get; set; }
        public ApiResponse(T data)
        {
            Data = data;
        }
    }
}
