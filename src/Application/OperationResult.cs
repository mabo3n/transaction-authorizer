using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;

namespace Authorizer.Application
{
    public class OperationResult
    {
        public Account Account { get; set; }
        public Violation[] Violations { get; set; }

        public OperationResult(Account account, Violation[] violations)
        {
            Account = account;
            Violations = violations;
        }
    }
}
