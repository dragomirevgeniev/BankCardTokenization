using System.Windows;
using System.Windows.Controls;

namespace BankCardTokenization.Client.UserControls
{
    public partial class RequestRegisterTokenUserControl : UserControl
    {
        public RequestGenerateTokenDelegate ProcessGenerateToken { get; set; }

        public RequestGenerateTokenDelegate ProcessRequestBankNumber { get; set; }

        public LogoutDelegate ProcessLogout { get; set; }

        public RequestRegisterTokenUserControl()
        {
            InitializeComponent();
        }

        private void btnGetCardNumber_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessRequestBankNumber != null)
            {
                // removes unnecessary symbols from mask
                ProcessRequestBankNumber(txtToken.Text.Replace(" ", "").Replace("_", "")); 
            }
        }

        private void btnGenerateToken_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessGenerateToken != null)
            {
                // removes unnecessary symbols from mask
                ProcessGenerateToken(txtCardNumber.Text.Replace(" ", "").Replace("_", ""));
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessLogout != null)
            {
                // logout the user
                ProcessLogout();
            }
        }
    }
}
