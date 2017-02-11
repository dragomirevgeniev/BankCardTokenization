using System.Windows;
using System.Windows.Controls;


namespace BankCardTokenization.Client.UserControls
{
    public partial class LoginUserControl : UserControl
    {
        public LoginDelegate ProcessLogin { get; set; }

        public LoginUserControl()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessLogin != null)
            {
                ProcessLogin(txtUsername.Text, txtPassword.Password);
            }
        }

        private bool ValidateUserInput()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                ShowRequiredWarning(txtUsername, "Username is required");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                ShowRequiredWarning(txtPassword, "Password is required");
                return false;
            }

            return true;
        }

        private void ShowRequiredWarning(Control control, string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            control.Focus();
        }
    }
}
