namespace BankCardTokenization.Common
{
    public enum Operation
    {
        Login = 1,             // user wants to login
        Register = 2,          // user wants to register
        GenerateToken = 3,     // user wants to generate a new token
        RequestCardNumber = 4, // user wants to request a bank card number by token
        Approved = 5,          // system approves actions of the user
        Denied = 6,            // system dennies actions of the user
        Logout = 7,            // user wants to logout
        Disconnect = 8         // client disconnects from the server
    }

    public enum UserRights
    {
        None = 0,              // user has no rights
        GenerateToken = 1,     // user can generate tokens
        RequestCard = 2,       // user can request a bank card number by token
        All = 3                // user has all rights in the system
    }
}
