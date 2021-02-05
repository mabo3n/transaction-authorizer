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

        public async Task<OperationResult> Handle(AuthorizeTransaction payload)
        {
            var account = await accountRepository.Get();

            if (account == null)
                return new OperationResult(
                    account,
                    violations: new Violation[] {
                        Violation.AccountNotInitialized
                    }
                );

            var transaction = new Transaction(
                payload.Merchant, payload.Amount, payload.Time
            );

            var violations = transactionService
                .Authorize(transaction, account)
                .ToList();

            if (violations.Any())
                return new OperationResult(account, violations.ToArray());

            var updatedAccount = account.Apply(transaction);
            await accountRepository.Save(updatedAccount);
            await transactionRepository.Save(transaction);

            return new OperationResult(updatedAccount, violations.ToArray());
        }
    }
}
