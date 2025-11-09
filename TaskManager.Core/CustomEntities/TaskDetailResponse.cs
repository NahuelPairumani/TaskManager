using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.CustomEntities
{
    /// <summary>
    /// Representa la respuesta detallada de una tarea incluyendo el nombre del proyecto y del estado.
    /// </summary>
    public class TaskDetailResponse
    {
        /// <summary>
        /// Identificador único de la tarea.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título de la tarea.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descripción de la tarea.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Fecha límite de la tarea.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Nombre del estado actual de la tarea.
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Nombre del proyecto al cual pertenece la tarea.
        /// </summary>
        public string ProjectName { get; set; }
    }
}