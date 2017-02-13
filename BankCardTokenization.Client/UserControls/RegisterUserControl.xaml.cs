using BankCardTokenization.Common;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;


namespace BankCardTokenization.Client.UserControls
{
    public partial class RegisterUserControl : UserControl
    {
        public RegisterDelegate ProcessRegister { get; set; }

        public RegisterUserControl()
        {
            InitializeComponent();

            cbxRights.Items.Add(UserRights.None);
            cbxRights.Items.Add(UserRights.GenerateToken);
            cbxRights.Items.Add(UserRights.RequestCard);
            cbxRights.Items.Add(UserRights.All);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateUserInput() && ProcessRegister != null)
            {
                ProcessRegister(txtUsername.Text, txtPassword.Password, (UserRights)cbxRights.SelectedItem);
            }
        }

        private bool ValidateUserInput()
        {

            // Username validation
            // Username must be between 6 and 30 characters
            // Username can contain letters and numbers, dots and underscores
            // Dot and underscore cannot be next to each other
            // Username must not start or end with a dot or underscore
            if (!Regex.Match(txtUsername.Text, "^(?=.{6,30}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$").Success)
            {
                ShowRequiredWarning(txtUsername, string.Format("{0}\n{1}", "Invalid username!", Constants.USERNAME_INFO));
                return false;
            }
           
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                ShowRequiredWarning(txtPassword, "Password is required");
                return false;
            }
            else if (txtPassword.Password.Length < 6)
            {
                ShowRequiredWarning(txtPassword, "Password must have at least 6 symbols");
                return false;
            }

            if (string.IsNullOrEmpty(txtRepeatPassword.Password))
            {
                ShowRequiredWarning(txtRepeatPassword, "You must enter the password again!");
                return false;
            }

            if (cbxRights.SelectedItem == null)
            {
                ShowRequiredWarning(cbxRights, "User rights are required");
                return false;
            }

            if (txtPassword.Password != txtRepeatPassword.Password)
            {
                MessageBox.Show("Passwords don't match!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
