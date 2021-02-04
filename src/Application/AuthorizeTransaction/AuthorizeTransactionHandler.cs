using System.Linq;
using System.Threading.Tasks;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Enumerations;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services;

namespace Authorizer.Application
{
    public class AuthorizeTransactionHandler
        : IOperationHandler<AuthorizeTransaction>
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly ITransactionService transactionService;

        public AuthorizeTransactionHandler(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            ITransactionService transactionService
        )
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.transactionService = transactionService;
        }

        public Task<OperationResult> Handle(AuthorizeTransaction payload)
        {
            var account = accountRepository.Get();

            if (account == null)
                return Task.FromResult(
                    new OperationResult(
                        account,
                        violations: new Violation[] {
                            Violation.AccountNotInitialized
                        }
                    )
                );

            var transaction = new Transaction(
                payload.Merchant, payload.Amount, payload.Time
            );

            var violations = transactionService.Authorize(
                transaction, account
            );

            if (violations.Any())
                return Task.FromResult(
                    new OperationResult(account, violations.ToArray())
                );

            var updatedAccount = account.Apply(transaction);
            accountRepository.Save(updatedAccount);
            transactionRepository.Save(transaction);

            var op = new OperationResult(
                account: new Account(payload.Amount, true),
                violations: new Violation[] { }
            );

            return Task.FromResult(
                new OperationResult(updatedAccount, violations.ToArray())
            );
        }
    }
}
