using FluentAssertions;
using Xunit;
using Authorizer.Infrastructure;
using Authorizer.Domain.Enumerations;
using Authorizer.Application;
using Authorizer.Domain.Services;
using FluentAssertions.Extensions;
using Authorizer.Domain.Entities;
using System.Linq;

namespace AuthorizerTests.Application
{
    public class AuthorizeTransactionHandlerTests
    {
        private InMemoryDataSource dataSource;
        private AuthorizeTransactionHandler handler;

        public AuthorizeTransactionHandlerTests()
        {
            dataSource = new InMemoryDataSource();
            var accountRepository = new AccountRepository(dataSource);
            var transactionRepository = new TransactionRepository(dataSource);
            var transactionService = new TransactionService(transactionRepository);
            handler = new AuthorizeTransactionHandler(
                accountRepository,
                transactionRepository,
                transactionService
            );
        }

        [Fact]
        public async void Handle_ShouldReturnViolation_WhenAccountIsNotInitialized()
        {
            dataSource.Account = null;

            var operation = new AuthorizeTransaction(
                merchant: "Merchant ABC", amount: 123, time: 31.March(2012)
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().NotBeNull()
                .And
                .Contain(Violation.AccountNotInitialized);
        }

        [Fact]
        public async void Handle_ShouldStoreOneTransactionWithPayloadData()
        {
            dataSource.Account = new Account(
                activeCard: true, availableLimit: 300
            );

            var operation = new AuthorizeTransaction(
                merchant: "Merchant ABC", amount: 100, time: 31.March(2012)
            );

            await handler.Handle(operation);

            var storedTransactions = dataSource.Transactions;

            storedTransactions
                .Should().HaveCount(1);

            var transaction = storedTransactions.First();

            transaction.Merchant
                .Should().Be(operation.Merchant);

            transaction.Amount
                .Should().Be(operation.Amount);

            transaction.Time
                .Should().Be(operation.Time);
        }

        [Fact]
        public async void Handle_ShouldUpdateAccount()
        {
            dataSource.Account = new Account(
                activeCard: true, availableLimit: 300
            );

            var operation = new AuthorizeTransaction(
                merchant: "Merchant ABC", amount: 100, time: 12.December(2012)
            );

            await handler.Handle(operation);

            var updatedAccount = dataSource.Account;

            updatedAccount.AvailableLimit
                .Should().Be(200);

            updatedAccount.ActiveCard
                .Should().Be(true);
        }

        [Fact]
        public async void Handle_ShouldReturnUpdatedAcount()
        {
            dataSource.Account = new Account(
                activeCard: true, availableLimit: 10
            );

            var operation = new AuthorizeTransaction(
                merchant: "Merchant ABC", amount: 5, time: 12.December(2012)
            );

            var result = await handler.Handle(operation);

            var updatedAccount = dataSource.Account;

            result.Account.ActiveCard
                .Should().Be(updatedAccount.ActiveCard);

            result.Account.AvailableLimit
                .Should().Be(updatedAccount.AvailableLimit);
        }

        [Fact]
        public async void Handle_ShouldNotReturnViolation()
        {
            dataSource.Account = new Account(
                activeCard: true, availableLimit: 10
            );

            var operation = new AuthorizeTransaction(
                merchant: "Merchant ABC", amount: 5, time: 12.December(2012)
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().NotBeNull()
                .And
                .BeEmpty();
        }

        [Fact]
        public async void Handle_ShouldReturnViolation_WhenTransactionIsUnauthorized()
        {
            dataSource.Account = new Account(
                activeCard: true, availableLimit: 10
            );

            var operation = new AuthorizeTransaction(
                merchant: "Merchant DEF", amount: 100, time: 12.December(2012)
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().NotBeNull()
                .And
                .HaveCount(1);
        }

        [Fact]
        public async void Handle_ShouldNotUpdateAccount_WhenTransactionIsUnauthorized()
        {
            var originalAccount = new Account(
                availableLimit: 999, activeCard: false
            );
            dataSource.Account = originalAccount;

            var operation = new AuthorizeTransaction(
                merchant: "Merchant DEF", amount: 1000, time: 12.December(2012)
            );

            await handler.Handle(operation);

            var updatedAccount = dataSource.Account;

            updatedAccount
                .Should().NotBeNull();

            updatedAccount.AvailableLimit
                .Should().Be(originalAccount.AvailableLimit);

            updatedAccount.ActiveCard
                .Should().Be(originalAccount.ActiveCard);
        }

        [Fact]
        public async void Handle_ShouldReturnMultipleViolations_WhenTransactionWouldIntroduceMultipleViolations()
        {
            dataSource.Account = new Account(
                activeCard: false, availableLimit: 0
            );

            var operation = new AuthorizeTransaction(
                merchant: "Memerchant", amount: 100, time: 01.January(2080)
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().NotBeNull()
                .And
                .BeEquivalentTo(new[] {
                    Violation.CardNotActive,
                    Violation.InsufficientLimit
                });
        }
    }
}
