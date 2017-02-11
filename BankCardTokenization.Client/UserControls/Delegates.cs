using BankCardTokenization.Common;

namespace BankCardTokenization.Client.UserControls
{
    public delegate void RegisterDelegate(string username, string password, UserRights rights);

    public delegate void LoginDelegate(string username, string password);

    public delegate void RequestGenerateTokenDelegate(string cardNumber);

    public delegate void LogoutDelegate();
}
