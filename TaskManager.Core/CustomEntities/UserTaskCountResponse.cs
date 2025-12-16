namespace TaskManager.Core.CustomEntities
{
    public class UserTaskCountResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TaskCount { get; set; }
        public string ProjectName { get; set; } = null!;
    }
}
