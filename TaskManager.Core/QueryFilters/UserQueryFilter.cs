using Swashbuckle.AspNetCore.Annotations;

namespace TaskManager.Core.QueryFilters
{
    /// <summary>
    /// Filtro de búsqueda y paginación para usuarios.
    /// </summary>
    /// <remarks>
    /// Permite filtrar usuarios por nombre, apellido o correo electrónico.
    /// También incluye parámetros de paginación heredados de <see cref="PaginationQueryFilter"/>.
    /// </remarks>
    public class UserQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por nombre del usuario (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Filtrar por nombre del usuario")]
        /// <example>Nahuel</example>
        public string? FirstName { get; set; }

        /// <summary>
        /// Filtrar por apellido del usuario (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Filtrar por apellido del usuario")]
        /// <example>Pairumani</example>
        public string? LastName { get; set; }

        /// <summary>
        /// Filtrar por correo electrónico del usuario (coincidencia parcial).
        /// </summary>
        [SwaggerSchema("Filtrar por correo electrónico del usuario")]
        /// <example>nahuel.pairumani@example.com</example>
        public string? Email { get; set; }
    }
}
