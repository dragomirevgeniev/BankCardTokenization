namespace BankCardTokenization.Common
{
    public static class Constants
    {
        public static readonly string LOCALHOST = "127.0.0.1";
        public static readonly int PORT = 6543;
        public static readonly int VALID_BANK_CARD_NUMBER_LENGTH = 16;
        public static readonly int GENERATE_TOKEN_MAX_RETRIES = 1337;
        public static readonly string BANK_CARDS_FILE_PATH = "bank_cards.xml";
        public static readonly string USERS_FILE_PATH = "users.xml";
        public static readonly string WAITING_FOR_CONNECTIONS = "Server started! \r\nWaiting for new connections...";
        public static readonly string ACCEPTED_CONNECTION = "Accepted connection! \r\nTotal number of connections: {0}.";
        public static readonly string USER_SUCCESSFULLY_REGISTERED = "User {0} successfully registered!";
        public static readonly string USER_LOGGED_IN = "User {0} successfully logged in!";
        public static readonly string WELLCOME_IN_THE_SYSTEM = "Wellcome, {0}!";
        public static readonly string SUCCESSFULLY_READ_DATA = "Successfully loaded {0} users and {1} bank cards!";
        public static readonly string CARDS_TOKENS_EXPORT_TEMPLATE = "Card: {0} <--> Token: {1}";
        public static readonly string INFORMATION_TITLE = "Information!";
        public static readonly string USER_HAS_CREATED_TOKEN = "User {0} has generated a token!";
        public static readonly string USER_HAS_REQUESTED_BANK_NUMBER = "User {0} has requested a bank number!";
        public static readonly string USER_HAS_LOGGED_OUT = "User {0} logged out!";
        public static readonly string USER_HAS_DISCONNECTED = "User {0} disconnected!";
        public static readonly string CONNECTION_CLOSED = "Connection had been closed!";
        public static readonly string INVALID_OPERATION = "Invalid operation!";
        public static readonly string USERNAME_ALREADY_IN_USE = "Username {0} is already taken!";
        public static readonly string USERNAME_OR_PASSWORD_INCORRECT = "Incorrect username or password!";
        public static readonly string EXPORT_SUCCESSFULL = "Data exported successfully!";
        public static readonly string ACCESS_DENIED = "Access denied for given operation!";
        public static readonly string INVALID_BANK_CARD_NUMBER = "The given bank card number is invalid!";
        public static readonly string FAILED_TOKEN_GENERATION = "Failed to generate token!";
        public static readonly string BANK_CARD_NOT_FOUND = "There is no bank card associated with this token!";
        public static readonly string USER_NOT_FOUND = "There with that nickname doesn't exist!";
        public static readonly string USERNAME_INFO = "Username must be between 6 and 20 characters\nUsername can contain letters and numbers, dots and underscores\nDot and underscore cannot be next to each other\nUsername must not start or end with a dot or underscore";
    }
}
