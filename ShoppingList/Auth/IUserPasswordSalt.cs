namespace ShoppingList.Auth
{
    public interface IUserPasswordSalt
    {
        string Salt();
    }

    public class UserPasswordSaltImpl : IUserPasswordSalt
    {
        private readonly string _salt;

        public UserPasswordSaltImpl()
        {
            _salt = Startup.PasswordSalt;
        }

        public string Salt() => _salt;
    }
}