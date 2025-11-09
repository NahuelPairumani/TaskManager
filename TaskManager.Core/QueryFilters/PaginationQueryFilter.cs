using Swashbuckle.AspNetCore.Annotations;

namespace TaskManager.Core.QueryFilters
{
    /// <summary>
    /// Filtro base para paginación de resultados.
    /// </summary>
    public abstract class PaginationQueryFilter
    {
        /// <summary>
        /// Cantidad de registros por página.
        /// </summary>
        [SwaggerSchema("Cantidad de registros por pagina")]
        /// <example>10</example>
        public int PageSize { get; set; }

        /// <summary>
        /// Número de página a mostrar.
        /// </summary>
        [SwaggerSchema("Numero de pagina a mostrar")]
        /// <example>1</example>
        public int PageNumber { get; set; }
    }
}
