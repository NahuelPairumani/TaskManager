using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySqlConnector; // Pomelo
using SocialMedia.Core.Enum;
using TaskManager.Core.Interfaces;
using System.Data;

namespace TaskManager.Infrastructure.Repositories
{
    // Implementación de IDbConnectionFactory que crea conexiones según el proveedor configurado
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _sqlConn;    // Cadena de conexión SQL Server
        private readonly string _mySqlConn;  // Cadena de conexión MySQL
        private readonly IConfiguration _config; // Para acceder a la configuración

        // Constructor: lee las cadenas de conexión y el proveedor de la configuración
        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;

            // Obtener la cadena de conexión para SQL Server
            _sqlConn = _config.GetConnectionString("ConnectionSqlServer") ?? string.Empty;

            // Obtener la cadena de conexión para MySQL
            _mySqlConn = _config.GetConnectionString("ConnectionMySql") ?? string.Empty;

            // Leer el proveedor configurado ("SqlServer" o "MySql")
            var providerStr = _config.GetSection("DatabaseProvider").Value
                ?? "SqlServer";

            // Asignar la propiedad Provider según la configuración
            Provider = providerStr.Equals("MySql",
                StringComparison.OrdinalIgnoreCase)
                ? DatabaseProvider.MySql
                : DatabaseProvider.SqlServer;
        }

        // Propiedad que indica qué proveedor de base de datos usar
        public DatabaseProvider Provider { get; }

        // Crea y devuelve una conexión abierta según el proveedor
        public IDbConnection CreateConnection()
        {
            return Provider switch
            {
                DatabaseProvider.MySql => new MySqlConnection(_mySqlConn),
                _ => new SqlConnection(_sqlConn) // Por defecto, SQL Server
            };
        }
    }
}

