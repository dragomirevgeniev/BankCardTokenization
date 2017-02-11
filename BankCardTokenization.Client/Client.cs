using System;
using BankCardTokenization.Common;
using System.Net.Sockets;
using System.IO;
using Xceed.Wpf.Toolkit;

namespace BankCardTokenization.Client
{
    public class Client : IDisposable
    {
        private TcpClient client { get; set; }             // property for connection to the server

        public Action<string> ProcessMessage { get; set; } // delegate for displaying messages

        public Action<string> ProcessError { get; set; }   // delegate for displaying errors

        private NetworkStream NetworkStream { get; set; }  // the network stream of the connection

        private BinaryReader reader { get; set; }          // the binary reader of the connection

        private BinaryWriter writer { get; set; }          // the binary writer of the connection

        public Client(Action<string> processMessage, Action<string> processError)
        {
            ProcessMessage = processMessage;
            ProcessError = processError;
            InitializeClient();
        }

        private void InitializeClient()
        {
            try
            {
                client = new TcpClient();
                client.Connect(Constants.LOCALHOST, Constants.PORT);

                NetworkStream = client.GetStream();
                reader = new BinaryReader(NetworkStream);
                writer = new BinaryWriter(NetworkStream);
            }
            catch(Exception e)
            {
                ProcessError(e.Message);
                Environment.Exit(Environment.ExitCode);
            }
        }

        internal void RequestCardNumber(string token, MaskedTextBox textBoxResponse)
        {
            writer.Write((int)Operation.RequestCardNumber);
            if (object.Equals((Operation)reader.ReadInt32(), Operation.Denied))
            {
                ProcessError(Constants.ACCESS_DENIED);
            }
            else
            {
                writer.Write(token);
                string response = reader.ReadString();
                if (object.Equals(response, Constants.BANK_CARD_NOT_FOUND))
                {
                    ProcessMessage(Constants.BANK_CARD_NOT_FOUND);
                }
                else
                {
                    textBoxResponse.Text = response;
                }
            }
        }

        internal void GenerateToken(string cardNumber, MaskedTextBox textBoxResponse)
        {
            writer.Write((int)Operation.GenerateToken);
            if (object.Equals((Operation)reader.ReadInt32(), Operation.Denied))
            {
                ProcessError(Constants.ACCESS_DENIED);
            }
            else
            {
                writer.Write(cardNumber);
                string response = reader.ReadString();

                if (object.Equals(response, Constants.INVALID_BANK_CARD_NUMBER))
                {
                    ProcessError(Constants.INVALID_BANK_CARD_NUMBER);
                }
                else if (object.Equals(response, Constants.FAILED_TOKEN_GENERATION))
                {
                    ProcessError(Constants.FAILED_TOKEN_GENERATION);
                }
                else
                {
                    textBoxResponse.Text = response;
                }
            }
        }

        public bool Register(string username, string password, UserRights rights)
        {
            writer.Write((int)Operation.Register);
            writer.Write(username);
            writer.Write(password);
            writer.Write((int)rights);

            string response = reader.ReadString();
            ProcessMessage(response);

            return response.Equals(string.Format(Constants.USER_SUCCESSFULLY_REGISTERED, username));
        }

        public bool Login(string username, string password)
        {
            writer.Write((int)Operation.Login);
            writer.Write(username);
            writer.Write(password);

            string response = reader.ReadString();
            ProcessMessage(response);

            return response.Equals(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
        }

        public bool Logout()
        {
            writer.Write((int)Operation.Logout);
            return reader.ReadBoolean();
        }

        public void Dispose()
        {
            writer.Write((int)Operation.Disconnect);
            client.Close();
        }
    }
}
