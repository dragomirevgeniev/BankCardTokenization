using System.Collections.Generic;

namespace BankCardTokenization.Server.Objects
{
    public class BankCard
    {
        private string cardNumber;
        private List<Token> tokens;

        #region Properties
        public string CardNumber
        {
            get
            {
                return cardNumber;
            }
            set
            {
                cardNumber = value;
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return tokens;
            }
            set
            {
                tokens = new List<Token>();
                foreach (var token in value)
                {
                    tokens.Add(token);
                }
            }
        }

        #endregion

        #region Constructors
        public BankCard(string cardNumber, List<Token> tokens)
        {
            CardNumber = cardNumber;
            Tokens = tokens;
        }

        public BankCard() : this(string.Empty, new List<Token>()) { }
        
        public BankCard(BankCard card) : this(card.CardNumber, card.Tokens) { }

        #endregion
    }
}
