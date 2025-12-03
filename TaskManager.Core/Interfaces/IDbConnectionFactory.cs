using TaskManager.Core.Enum;
using System.Data;

namespace TaskManager.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        DatabaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
