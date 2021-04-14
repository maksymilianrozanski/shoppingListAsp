using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class UserReadDto
    {
        [Key] public int Id { get; set; }

        [Required] public string Login { get; set; }

        public static implicit operator UserReadDto(UserEntity userEntity) =>
            new()
            {
                Id = userEntity.Id, Login = userEntity.Login
            };

        public static implicit operator string(UserReadDto userReadDto) => userReadDto.Login;
    }
}