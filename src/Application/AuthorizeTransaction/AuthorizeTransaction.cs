using Authorizer.Domain.Entities;

namespace Authorizer.Application
{
    public class AuthorizeTransaction : Operation
    {
        public Transaction Transaction { get; }

        public AuthorizeTransaction(Transaction transaction)
        {
            Transaction = transaction;
        }
    }
}
