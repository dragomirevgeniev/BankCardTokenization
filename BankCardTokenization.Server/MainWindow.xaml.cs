using System;
using System.ComponentModel;
using System.Windows;


namespace BankCardTokenization.Server
{
    public partial class MainWindow : Window
    {
        private Server Server { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Server = new Server(DisplayMessage, DisplayError);
        }

        private void DisplayMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(DisplayMessage), message);
            }
            else
            {
                txtMessages.AppendText(string.Format("{0:MM/dd/yy H:mm:ss}: ", DateTime.Now) + message.ToString() + Environment.NewLine);
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

        private void btnExportByToken_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByToken();
        }

        private void btnExportByCardNumber_Click(object sender, RoutedEventArgs e)
        {
            Server.ExportByBankCard();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            
            Server.Dispose();
            base.OnClosing(e);
            Environment.Exit(0);
        }

        private void txtMessages_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // auto scrolling messages even if not active tab
            txtMessages.Focus();
            txtMessages.CaretIndex = txtMessages.Text.Length;
            txtMessages.ScrollToEnd();
        }
    }
}
