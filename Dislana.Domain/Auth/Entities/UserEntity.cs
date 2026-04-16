using Dislana.Domain.Auth.ValueObjects;

namespace Dislana.Domain.Auth.Entities
{
    /// <summary>
    /// Entidad de Dominio: Representa un usuario del sistema
    /// Rich Entity con comportamiento y validaciones de negocio
    /// </summary>
    public sealed class UserEntity
    {
        public long Id { get; private set; }
        public string UserName { get; private set; }
        public Email Email { get; private set; }
        public PersonName FirstName { get; private set; }
        public PersonName LastName { get; private set; }
        public bool IsActive { get; private set; }

        // Constructor privado para asegurar creación válida solo mediante Factory Methods
        private UserEntity(
            long id,
            string userName,
            Email email,
            PersonName firstName,
            PersonName lastName,
            bool isActive)
        {
            Id = id;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsActive = isActive;
        }

        /// <summary>
        /// Factory Method: Crea un nuevo usuario durante el registro
        /// </summary>
        public static UserEntity CreateForRegistration(
            string firstName,
            string lastName,
            string email)
        {
            var emailVO = Email.Create(email);
            var firstNameVO = PersonName.Create(firstName, "nombre");
            var lastNameVO = PersonName.Create(lastName, "apellido");

            return new UserEntity(
                id: 0, // Se asignará por la BD
                userName: emailVO.Value, // UserName = Email
                email: emailVO,
                firstName: firstNameVO,
                lastName: lastNameVO,
                isActive: true
            );
        }

        /// <summary>
        /// Factory Method: Reconstruye una entidad desde el repositorio
        /// </summary>
        public static UserEntity Reconstitute(
            long id,
            string userName,
            string email,
            string firstName,
            string lastName,
            bool isActive)
        {
            // Validar que los datos de la BD sean válidos
            var emailVO = Email.Create(email);
            var firstNameVO = PersonName.Create(firstName, "nombre");
            var lastNameVO = PersonName.Create(lastName, "apellido");

            return new UserEntity(id, userName, emailVO, firstNameVO, lastNameVO, isActive);
        }

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Valida si el usuario puede iniciar sesión
        /// </summary>
        public void ValidateCanLogin()
        {
            if (!IsActive)
                throw new Exceptions.DomainException("Usuario inactivo");
        }

        /// <summary>
        /// Desactiva el usuario
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activa el usuario
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }
    }
}
