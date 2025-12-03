using TaskManager.Core.Entities;
using TaskManager.Core.Enum;

namespace TaskManager.Infrastructure.DTOs
{
    public class SecurityDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public RoleType? Role { get; set; }
    }
}
