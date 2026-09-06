namespace consultorio_backend.Models.Identity
{
    /// <summary>
    /// Constantes con los nombres de los roles usados por ASP.NET Core Identity (AspNetRoles).
    /// Usar estas constantes en lugar de "magic strings" al asignar o verificar roles.
    /// </summary>
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Psychologist = "Psychologist";
        public const string Patient = "Patient";

        public static readonly string[] All = { Admin, Psychologist, Patient };
    }
}
