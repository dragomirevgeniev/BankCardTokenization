using BankCardTokenization.Client.UserControls;
using BankCardTokenization.Common;
using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

namespace BankCardTokenization.Client
{
    public partial class MainWindow : Window
    {
        private RequestRegisterTokenUserControl ucRequestRegisterToken;

        private Client Client { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Client = new Client(DisplayMessage, DisplayError);
            ucRequestRegisterToken = new RequestRegisterTokenUserControl();

            // set the LoginUser delegate for managing the login event
            ucLoginUser.ProcessLogin = LoginUser;
            // set the RegisterUser delegate for managing the register event
            ucRegisterUser.ProcessRegister = RegisterUser;
            // set the GenerateToken delegate for managing the login event
            ucRequestRegisterToken.ProcessGenerateToken = GenerateToken;
            // set the RequestCardNumber delegate for managing the login event
            ucRequestRegisterToken.ProcessRequestBankNumber = RequestCardNumber;
            // set the Logout delegate for managing the logout event
            ucRequestRegisterToken.ProcessLogout = LogoutUser;
        }

        private void RequestCardNumber(string token)
        {
            try
            {
                Client.RequestCardNumber(token, ucRequestRegisterToken.txtCardNumber);
            }
            catch (Exception e)
            {
                DisplayError(e.Message);
            }
        }

        private void GenerateToken(string bankNumber)
        {
            try
            {
                Client.GenerateToken(bankNumber, ucRequestRegisterToken.txtToken);
            }
            catch (Exception e)
            {
                DisplayError(e.Message);
            }
        }

        private void LoginUser(string username, string password)
        {
            if (Client.Login(username, password))
            {
                mainGrid.Children.Remove(tabControl);
                mainGrid.Children.Add(ucRequestRegisterToken);
                ucRequestRegisterToken.lblUsername.Text = username;
                switch (Client.CurrentRights)
                {
                    case UserRights.None:
                        ucRequestRegisterToken.btnGenerateToken.Visibility = Visibility.Hidden;
                        ucRequestRegisterToken.btnGetCardNumber.Visibility = Visibility.Hidden;
                        break;
                    case UserRights.GenerateToken:
                        ucRequestRegisterToken.btnGetCardNumber.Visibility = Visibility.Hidden;
                        Grid.SetColumnSpan(ucRequestRegisterToken.btnGenerateToken, 2);
                        break;
                    case UserRights.RequestCard:
                        ucRequestRegisterToken.btnGenerateToken.Visibility = Visibility.Hidden;
                        Grid.SetColumnSpan(ucRequestRegisterToken.btnGetCardNumber, 2);
                        break;
                    case UserRights.All:
                        break;
                    default:
                        break;
                }
            }
        }

        private void LogoutUser()
        {
            if (Client.Logout())
            {
                mainGrid.Children.Remove(ucRequestRegisterToken);
                mainGrid.Children.Add(tabControl);
            }
        }

        private void RegisterUser(string username, string password, UserRights rights)
        {
            if (Client.Register(username, password, rights))
            {
                ucLoginUser.txtUsername.Text = ucRegisterUser.txtUsername.Text;
                ucLoginUser.txtPassword.Password = ucRegisterUser.txtPassword.Password;
                ucRegisterUser.txtUsername.Text = string.Empty;
                ucRegisterUser.txtPassword.Password = string.Empty;
                ucRegisterUser.txtRepeatPassword.Password = string.Empty;
                loginTab.IsSelected = true;
            }
        }

        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayMessage), message);
            }
            else
            {
                MessageBox.Show(message, Constants.INFORMATION_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DisplayError(string errorDetails)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayError), errorDetails);
            }
            else
            {
                MessageBox.Show(errorDetails, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Dispose the client connection when closing the application
            Client.Dispose();
            Environment.Exit(0);
        }
    }
}
