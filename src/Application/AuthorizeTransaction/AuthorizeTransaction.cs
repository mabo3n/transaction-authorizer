using System;

namespace Authorizer.Application
{
    public class AuthorizeTransaction : Operation
    {
        public string Merchant { get; set; }
        public int Amount { get; set;}
        public DateTime Time { get; set; }

        public AuthorizeTransaction() { }
    }
}

