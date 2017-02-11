namespace BankCardTokenization.Server.Objects
{
    public class Token
    {
        private string id;
        private string user;

        #region Properties
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }
        #endregion

        #region Constructors
        public Token(string id, string username)
        {
            Id = id;
            user = username;
        }

        public Token() : this(string.Empty, string.Empty) { }

        public Token(Token token) : this(token.Id, token.user) { }

        #endregion
    }
}
