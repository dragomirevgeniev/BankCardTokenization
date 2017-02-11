using BankCardTokenization.Common;
using BankCardTokenization.Server.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace BankCardTokenization.Server
{
    public class Server
    {
        private Thread ServerThread { get; set; }           // the thread of the server

        private Action<string> ProcessMessage { get; set; } // delegate for managing messages

        private Action<string> ProcessError { get; set; }   // delegate for managing erros

        private List<Socket> Connections { get; set; }      // all client connection

        public List<User> Users;                            // list of all users

        public List<BankCard> BankCards;                    // list of all cards and tokens

        private XMLProcessor xmlProcessor;                  // used for working with xml files

        public Server(Action<string> processMessage, Action<string> processError)
        {
            if (processMessage == null || processError == null)
            {
                Environment.Exit(Environment.ExitCode);
            }

            ProcessMessage = processMessage;
            ProcessError = processError;
            Connections = new List<Socket>();
            xmlProcessor = new XMLProcessor();
            LoadData();
            InitializeServer();
        }

        public void InitializeServer()
        {
            // Start new thread for the server
            ServerThread = new Thread(new ThreadStart(RunServer));
            ServerThread.Start();
        }

        public void RunServer()
        {
            TcpListener listener;
            try
            {
                // Initialising the listener and starting it
                IPAddress address = IPAddress.Parse(Constants.LOCALHOST);
                listener = new TcpListener(address, Constants.PORT);
                listener.Start();

                // Listener was started
                ProcessMessage(Constants.WAITING_FOR_CONNECTIONS);

                while (true)
                {
                    // Get new connection
                    Socket newConnection = listener.AcceptSocket();
                    // Save the connection
                    Connections.Add(newConnection);
                    // Create new client processor for the new connection
                    ClientProcessor clientProcessor = new ClientProcessor(ProcessMessage, ProcessError, Users, BankCards);

                    //Run the client processor on a new thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(clientProcessor.ProcessClient), newConnection);

                    //Notify accepted connection
                    ProcessMessage(string.Format(Constants.ACCEPTED_CONNECTION, this.Connections.Where(c => c.Connected).Count()));
                }
            }
            catch (Exception e)
            {
                if (ProcessError != null)
                {
                    ProcessError(e.Message);
                }
            }
        }

        private void LoadData()
        {
            try
            {
                xmlProcessor.LoadXml(typeof(List<BankCard>), Constants.BANK_CARDS_FILE_PATH, ref BankCards);
                xmlProcessor.LoadXml(typeof(List<User>), Constants.USERS_FILE_PATH, ref Users);

                ProcessMessage(string.Format(Constants.SUCCESSFULLY_READ_DATA, Users.Count, BankCards.Count));
            }
            catch (Exception e)
            {
                ProcessError(e.Message);
                Users = new List<User>();
                BankCards = new List<BankCard>();
            }
        }

        private void SaveData()
        {
            try
            {
                xmlProcessor.SaveXml(typeof(List<BankCard>), Constants.BANK_CARDS_FILE_PATH, BankCards);
                xmlProcessor.SaveXml(typeof(List<User>), Constants.USERS_FILE_PATH, Users);
            }
            catch (Exception e)
            {
                if (ProcessError != null)
                {
                    ProcessError(e.Message);
                }
            }
        }

        public void Dispose()
        {
            SaveData();
        }

        public void ExportByBankCard()
        {
            string filePath = string.Empty;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (object.Equals(dialog.ShowDialog(), DialogResult.OK))
                {
                    filePath = dialog.FileName;
                }
            }

            try
            {
                List<string> result = new List<string>();
                IEnumerable<BankCard> sorted = BankCards.OrderBy(b => b.CardNumber);

                foreach (BankCard card in sorted)
                {
                    foreach (Token token in card.Tokens)
                    {
                        result.Add(string.Format(Constants.CARDS_TOKENS_EXPORT_TEMPLATE, card.CardNumber, token.Id));
                    }
                }

                File.WriteAllLines(filePath, result);
                ProcessMessage(Constants.EXPORT_SUCCESSFULL);
            }
            catch (Exception e)
            {
                if (ProcessError != null)
                {
                    ProcessError(e.Message);
                }
            }
        }

        public void ExportByToken()
        {
            string filePath = string.Empty;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (object.Equals(dialog.ShowDialog(), DialogResult.OK))
                {
                    filePath = dialog.FileName;
                }
            }

            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                List<string> result = new List<string>();
                IEnumerable<BankCard> sorted = BankCards.OrderBy(b => b.CardNumber);

                foreach (BankCard card in sorted)
                {
                    foreach (Token token in card.Tokens)
                    {
                        data.Add(token.Id, card.CardNumber);
                    }
                }

                ICollection<string> tokens = data.Keys.OrderBy(k => k).ToList();

                foreach (var token in tokens)
                {
                    result.Add(string.Format(Constants.CARDS_TOKENS_EXPORT_TEMPLATE, data[token], token));
                }

                File.WriteAllLines(filePath, result);
                ProcessMessage(Constants.EXPORT_SUCCESSFULL);
            }
            catch (Exception e)
            {
                if (ProcessError != null)
                {
                    ProcessError(e.Message);
                }
            }
        }
    }
}
