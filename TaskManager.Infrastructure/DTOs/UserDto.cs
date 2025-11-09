namespace TaskManager.Infrastructure.DTOs
{
    /// <summary>
    /// Representa la información básica de un usuario dentro del sistema.
    /// </summary>
    /// <remarks>
    /// Este DTO se utiliza para transferir los datos esenciales de los usuarios, 
    /// tales como nombre, apellido y correo electrónico.
    /// </remarks>
    public class UserDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        /// <example>4</example>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        /// <example>Nahuel</example>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        /// <example>Pairumani</example>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Dirección de correo electrónico del usuario.
        /// </summary>
        /// <example>nahuel.pairumani@example.com</example>
        public string Email { get; set; } = null!;
    }
}
