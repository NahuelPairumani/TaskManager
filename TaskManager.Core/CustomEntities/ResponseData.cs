using System.Net;
using System.Text.Json.Serialization;

namespace TaskManager.Core.CustomEntities
{
    public class ResponseData
    {
        /// <summary>
        /// Contiene los datos cuando se devuelve una sola entidad o resultado no paginado, tipo object para tomar cualquier tipo de dato.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Contiene los datos cuando la respuesta es una lista paginada.
        /// </summary>
        public PagedList<object> Pagination { get; set; }

        /// <summary>
        /// Mensajes informativos, de advertencia o error asociados a la respuesta.
        /// </summary>
        public Message[] Messages { get; set; }

        /// <summary>
        /// Código de estado HTTP asociado a la respuesta.
        /// </summary>
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
