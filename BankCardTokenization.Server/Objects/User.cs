using BankCardTokenization.Common;

namespace BankCardTokenization.Server.Objects
{
    public class User
    {
        private string username;
        private string password;

        public string PasswordSalt { get; set; }
        public UserRights Rights { get; set; }

        #region Properites
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        #endregion

        #region Constructors 
        public User(string username, string password, string passwordSalt, UserRights rights)
        {
            Username = username;
            Password = password;
            PasswordSalt = passwordSalt;
            Rights = rights;
        }

        public User(string username, string password,  UserRights rights)
        {
            Username = username;
            Password = password;
            Rights = rights;
        }

        public User() : this("defaultUser", "defaultPassword", UserRights.None) { }

        public User(User user) : this(user.Username, user.Password, user.Rights) { }

        #endregion
    }
}
