using LaYumba.Functional;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class UserCreatedReadDto : UserReadDto
    {
        public UserCreatedReadDto(int id, string login, string password)
        {
            Password = password;
            this.Id = id;
            this.Login = login;
        }

        public string Password { get; set; }

        public static implicit operator UserCreatedReadDto(UserEntity userEntity) =>
            new(userEntity.Id, userEntity.Login, userEntity.Password);
    }
}