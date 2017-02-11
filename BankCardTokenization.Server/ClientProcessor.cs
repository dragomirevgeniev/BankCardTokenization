using BankCardTokenization.Common;
using BankCardTokenization.Server.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace BankCardTokenization.Server
{
    public class ClientProcessor
    {
        private Socket clientSocket;           // client socket (connection to client)

        private NetworkStream networkStream;   // network stream for the client

        private BinaryReader reader;           // binary reader for the client
         
        private BinaryWriter writer;           // binary writer for the client

        private User user;                     // current authenticated user

        private List<User> Users;              // reference list of all users

        private List<BankCard> Cards;          // reference list of all ban cards and tokens

        private Action<string> ProcessMessage; // delegate for managing messages

        private Action<string> ProcessError;   // delegate for managing erros

        public ClientProcessor(Action<string> processMessage, Action<string> processError,
            List<User> users, List<BankCard> cards)
        {
            ProcessMessage = processMessage;
            ProcessError = processError;
            Users = users;
            Cards = cards;
        }

        public void ProcessClient(object socket)
        {
            try
            {
                clientSocket = (Socket)socket;
                networkStream = new NetworkStream(clientSocket);
                reader = new BinaryReader(networkStream);
                writer = new BinaryWriter(networkStream);

                user = AuthenticateUser();

                if (user != null)
                {
                    while(clientSocket.Connected)
                    {
                        // while the connection is active process user actions
                        if (!ManageRequest())
                        {
                            // closing the socket connection when the user disconnects
                            clientSocket.Close();
                            break;
                        }
                    }
                }
                else
                {
                    // closing the socket connection when the user disconnects
                    clientSocket.Close();
                    ProcessMessage(Constants.CONNECTION_CLOSED);
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

        private User AuthenticateUser()
        {
            switch ((Operation)reader.ReadInt32())
            {
                case Operation.Login:
                    return LoginUser();
                case Operation.Register:
                    return RegisterUser();
                case Operation.Disconnect:
                    return null;
                default:
                    throw new InvalidOperationException(Constants.INVALID_OPERATION);
            }
        }

        private User LoginUser()
        {
            string username;
            string password;
            User user;

            try
            {
                username = reader.ReadString();
                password = reader.ReadString();

                user = Users.FirstOrDefault(u => object.Equals(u.Username, username));
                if (user == null)
                {
                    writer.Write(Constants.USER_NOT_FOUND);
                    return AuthenticateUser();
                }

                var saltedPassword = CryptographyManager.GenerateSHA256Hash(password, user.PasswordSalt);

                if (user != null && object.Equals(user.Password, saltedPassword))
                {
                    ProcessMessage(string.Format(Constants.USER_LOGGED_IN, username));
                    writer.Write(string.Format(Constants.WELLCOME_IN_THE_SYSTEM, username));
                    return user;
                }
                else
                {
                    writer.Write(Constants.USERNAME_OR_PASSWORD_INCORRECT);
                }
            }
            catch(Exception e)
            {
                writer.Write(e.Message);
            }

            return AuthenticateUser();
        }

        private User RegisterUser()
        {
            string username;
            string password;
            UserRights rights = UserRights.None;

            username = reader.ReadString();
            password = reader.ReadString();
            rights = (UserRights)reader.ReadInt32();

            if (Users.Any(u => u.Username == username))
            {
                writer.Write(string.Format(Constants.USERNAME_ALREADY_IN_USE, username));
                return AuthenticateUser();
            }
            else
            {
                var salt = CryptographyManager.GenerateSalt();
                var saltedPassword = CryptographyManager.GenerateSHA256Hash(password, salt);

                User newUser = new User(username, saltedPassword, salt, rights);
                lock(Users)
                {
                    // locking the object to prevent multiple addings and misunderstanding
                    Users.Add(newUser);
                }

                string message = string.Format(Constants.USER_SUCCESSFULLY_REGISTERED, username);
                ProcessMessage(message);
                writer.Write(message);

                return AuthenticateUser();
            }
        }

        private bool ManageRequest()
        {
            // the user action
            Operation currentAction = ((Operation)reader.ReadInt32());

            if (object.Equals(currentAction, Operation.GenerateToken) &&
                (object.Equals(user.Rights, UserRights.GenerateToken) || object.Equals(user.Rights, UserRights.All)))
            {
                // generate token if the user has rights
                GenerateToken();
            }
            else if (object.Equals(currentAction, Operation.RequestCardNumber) && 
                (object.Equals(user.Rights, UserRights.RequestCard) || object.Equals(user.Rights, UserRights.All)))
            {
                // request bank card number if the user has rights
                RequestCardNumber();
            }
            else if (object.Equals(currentAction, Operation.Logout))
            {
                // log out the user from the system
                ProcessMessage(string.Format(Constants.USER_HAS_LOGGED_OUT, user.Username));
                user = null;
                writer.Write(true);
                user = AuthenticateUser();
                if (user == null)
                {
                    return false;
                }
            }
            else if (object.Equals(currentAction, Operation.Disconnect))
            {
                // disconnect client
                ProcessMessage(string.Format(Constants.USER_HAS_DISCONNECTED, user.Username));
                user = null;
                return false;
            }
            else
            {
                // user has no rights
                writer.Write((int)Operation.Denied);
            }

            // operation completed succesfully
            return true;
        }

        private void GenerateToken()
        {
            writer.Write((int)Operation.Approved);
            string bankCardNumber = reader.ReadString();
            string token = string.Empty;
            token = BankCardManager.GenerateToken(bankCardNumber);

            if (string.IsNullOrEmpty(token))
            {
                writer.Write(Constants.INVALID_BANK_CARD_NUMBER);
                return;
            }

            int maxRetries = Constants.GENERATE_TOKEN_MAX_RETRIES;

            while (TokenAlreadyInUse(token) && maxRetries > 0)
            {
                // generate new token if this one is used
                token = BankCardManager.GenerateToken(bankCardNumber);
                maxRetries--;
            }

            // Check wether the token generation was succesfull
            if (TokenAlreadyInUse(token))
            {
                // notifying that token generation was unsuccesfull
                writer.Write(Constants.FAILED_TOKEN_GENERATION);
                return;
            }

            // adding token to existing collection
            AddToken(user.Username, bankCardNumber, token);
            writer.Write(token);
            ProcessMessage(string.Format(Constants.USER_HAS_CREATED_TOKEN, user.Username));
        }

        private void AddToken(string username, string bankCardNumber, string token)
        {
            // lock because more than one thread use this collection
            lock (Cards)
            {
                BankCard current = null;
                current = Cards.FirstOrDefault(c => c.CardNumber == bankCardNumber);
                if (current == null)
                {
                    // if the card doesn't exist then create it
                    current = new BankCard(bankCardNumber, new List<Token>());
                    Cards.Add(current);
                }

                current.Tokens.Add(new Token(token, username));
            }
        }

        private bool TokenAlreadyInUse(string token)
        {
            bool result = false;

            // checking if token already exists in one of the bank cards
            Cards.ForEach(c =>
            {
                if (c.Tokens.Any(t => t.Id == token))
                {
                    result = true;
                    return;
                }
            });

            return result;
        }

        private void RequestCardNumber()
        {
            writer.Write((int)Operation.Approved);
            string token = reader.ReadString();
            string cardNumber = Constants.BANK_CARD_NOT_FOUND;

            foreach (var card in Cards)
            {
                if (card.Tokens.Any(t => t.Id == token))
                {
                    cardNumber = card.CardNumber;
                    break;
                }
            }

            writer.Write(cardNumber);
            ProcessMessage(string.Format(Constants.USER_HAS_REQUESTED_BANK_NUMBER, user.Username, cardNumber));
        }
    }
}
