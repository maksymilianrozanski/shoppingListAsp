using System.ComponentModel.DataAnnotations;

namespace SharedTypes.Dtos
{
    public class UserCreateDto
    {
        [Required] public string Login { get; set; }

        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; }

        public UserCreateDto(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}