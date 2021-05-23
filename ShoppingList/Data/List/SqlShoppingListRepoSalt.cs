using Authentication.Salt;
using LaYumba.Functional;
using SharedTypes.Dtos;
using SharedTypes.Entities;
using ShoppingList.Auth;

namespace ShoppingList.Data.List
{
    public partial class SqlShoppingListRepo
    {
        private SaltModule.Hash CreateHash(string password) =>
            SaltModule.createSha(SaltModule.Salt.NewSalt(_salt.Salt()), SaltModule.Password.NewPassword(password));

        protected UserEntity ToUserEntity(UserCreateDto userCreateDto) =>
            new()
            {
                Id = 0, Login = userCreateDto.Login, Password = CreateHash(userCreateDto.Password).Item
            };

        protected UserEntity ToUserEntity(UserLoginData loginData) =>
            new()
            {
                Id = 0, Login = loginData.Username, Password = CreateHash(loginData.Password).Item
            };
    }
}