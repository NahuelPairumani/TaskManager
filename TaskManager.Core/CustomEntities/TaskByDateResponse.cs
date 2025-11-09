using TaskManager.Core.QueryFilters;

namespace TaskManager.Core.CustomEntities
{
    public class TaskByDateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string ProjectName { get; set; }
    }
}
