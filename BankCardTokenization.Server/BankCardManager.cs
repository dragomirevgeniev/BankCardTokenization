using BankCardTokenization.Common;
using System;
using System.Linq;

namespace BankCardTokenization.Server
{
    public static class BankCardManager
    {
        private static Random rand = new Random();

        public static string GenerateToken(string cardNumber)
        {
            if (!IsBankCardNumberValid(cardNumber))
            {
                return null;
            }
            else
            {
                // returning token composed of 12 random numbers + the last 4 digits of the bank card number
                return CreateToken(cardNumber) + cardNumber.Substring(cardNumber.Length - 4);
            }
        }

        private static string CreateToken(string cardNumber)
        {
            int[] token = new int[cardNumber.Length - 4];
            int sum = 0;

            for (int i = 12; i < 16; i++)
            {
                sum += cardNumber[i];
            }

            // generating the first number of the token according to the rule
            do
            {
                token[0] = GenerateRandomDigit(rand);
            }
            while (IsFirstDigitCorrect(token[0]));

            sum += token[0];

            // generating all other numbers of the token
            for (int i = 1; i < token.Length; i++)
            {
                do
                {
                    token[i] = GenerateRandomDigit(rand);
                }
                while (token[i] == cardNumber[i] - '0');

                sum += token[i];
            }

            while (sum % 10 == 0)
            {
                sum -= token[token.Length - 1];
                do
                {
                    token[token.Length - 1] = GenerateRandomDigit(rand);
                }
                while (token[token.Length - 1] == cardNumber[token.Length - 1] - '0');
                
                sum += token[token.Length - 1];
            }

            char[] result = new char[token.Length];
            for (int i = 0; i < token.Length; i++)
            {
                result[i] = token[i].ToString()[0];
            }

            return new string(result);
        }

        private static int GenerateRandomDigit(Random rand)
        {
            return rand.Next(0, 10);
        }

        private static bool IsBankCardNumberValid(string cardNumber)
        {
            return cardNumber.Length == Constants.VALID_BANK_CARD_NUMBER_LENGTH &&
                IsFirstDigitCorrect(Convert.ToInt32(cardNumber[0].ToString())) &&
                TestLuhn(cardNumber);
        }

        private static bool TestLuhn(string cardNumber)
        {
            int[] digits = cardNumber.Select(e => int.Parse(e.ToString())).ToArray();

            int sum = 0;

            for (var i = 0; i < digits.Length; i++)
            {
                if (i % 2 == 0)
                {
                    digits[i] *= 2;
                }

                if (digits[i] >= 10)
                {
                    digits[i] = (digits[i] / 10) + (digits[i] % 10);
                }

                sum += digits[i];
            }

            return (sum % 10 == 0);
        }

        private static bool IsFirstDigitCorrect(int digit)
        {
            return (digit == 3 || digit == 4 || digit == 5 || digit == 6);
        }
    }
}
