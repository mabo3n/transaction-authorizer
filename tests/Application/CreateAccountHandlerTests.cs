using FluentAssertions;
using Xunit;
using Authorizer.Domain.Entities;
using Authorizer.Infrastructure;
using Authorizer.Domain.Enumerations;
using Authorizer.Application;
using Authorizer.Domain.Repositories;

namespace AuthorizerTests.Application
{
    public class CreateAccountHandlerTests
    {
        private InMemoryDataSource dataSource;
        private CreateAccountHandler handler;

        public CreateAccountHandlerTests()
        {
            dataSource = new InMemoryDataSource();
            IAccountRepository accountRepository = new AccountRepository(dataSource);
            handler = new CreateAccountHandler(accountRepository);
        }

        [Fact]
        public async void Handle_ShouldCreateAccountWithPayloadData()
        {
            dataSource.Account = null;

            var operation = new CreateAccount(
                activeCard: true, availableLimit: 100
            );

            await handler.Handle(operation);

            var createdAccount = dataSource.Account;

            createdAccount
                .Should().NotBeNull();

            createdAccount.ActiveCard
                .Should().Be(operation.ActiveCard);

            createdAccount.AvailableLimit
                .Should().Be(operation.AvailableLimit);
        }

        [Fact]
        public async void Handle_ShouldReturnCreatedAccountData()
        {
            dataSource.Account = null;

            var operation = new CreateAccount(
                activeCard: false, availableLimit: 300
            );

            var result = await handler.Handle(operation);

            var createdAccount = dataSource.Account;

            result.Account.ActiveCard
                .Should().Be(createdAccount.ActiveCard);

            result.Account.AvailableLimit
                .Should().Be(createdAccount.AvailableLimit);
        }

        [Fact]
        public async void Handle_ShouldNotReturnViolations()
        {
            dataSource.Account = null;

            var operation = new CreateAccount(
                activeCard: true, availableLimit: 0
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().NotBeNull()
                .And
                .BeEmpty();
        }

        [Fact]
        public async void Handle_ShouldReturnViolation_WhenAccountIsAlreadyInitialized()
        {
            var account = new Account(
                availableLimit: 123, activeCard: true
            );
            dataSource.Account = account;

            var operation = new CreateAccount(
                activeCard: true, availableLimit: 312
            );

            var result = await handler.Handle(operation);

            result.Violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.AccountAlreadyInitialized);
        }

        [Fact]
        public async void Handle_ShouldNotUpdateAccountData_WhenAccountIsAlreadyInitialized()
        {
            var originalAccount = new Account(
                availableLimit: 4444444, activeCard: false
            );
            dataSource.Account = originalAccount;

            var operation = new CreateAccount(
                activeCard: true, availableLimit: 800
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
    }
}
