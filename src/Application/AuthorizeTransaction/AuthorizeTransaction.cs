using System;

namespace Authorizer.Application
{
    public class AuthorizeTransaction : Operation
    {
        public string Merchant { get; }
        public int Amount { get; }
        public DateTime Time { get; }

        public AuthorizeTransaction(string merchant, int amount, DateTime time)
        {
            Merchant = merchant;
            Amount = amount;
            Time = time;
        }
    }
}

