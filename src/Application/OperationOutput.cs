using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;

namespace Authorizer.Application
{
    public class OperationOutput
    {
        public Account Account { get; }
        public Violation[] Violations { get; }

        public OperationOutput(Account account, Violation[] violations)
        {
            Account = account;
            Violations = violations;
        }
    }
}
