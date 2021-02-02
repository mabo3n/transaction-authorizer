using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Services;
using Authorizer.Infrastructure;
using Authorizer.Domain.Enumerations;
using AuthorizerTests.Utils;
using System.Linq;

namespace AuthorizerTests.Domain.Services
{
    public class TransactionServiceTests
    {
        private InMemoryDataSource dataSource;
        private TransactionRepository transactionRepository;
        private TransactionService service;
        private Random random = new Random(Seed: 12345);

        public TransactionServiceTests()
        {
            dataSource = new InMemoryDataSource();
            transactionRepository =  new TransactionRepository(dataSource);
            service = new TransactionService(transactionRepository);
        }

        private void GenerateRandomTransactions(
            DateTime from, DateTime to, params int[] amounts
        )
            => dataSource.Transactions = dataSource.Transactions
                .Concat(
                    amounts.Select(
                        amount => new Transaction(
                            merchant: $"Merchant {DateTime.Now.Ticks.ToString()}",
                            amount,
                            time: DateTimeUtils.RandomBetween(
                                from, to, randomizer: random
                            )
                        )
                    )
                );

        [Fact]
        public void Authorize_ShouldReturnNoViolation()
        {
            dataSource.Account = new Account(availableLimit: 100, activeCard: true);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 200, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction);

            violations
                .Should().BeEmpty();
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenAccountIsNotInitialized()
        {
            dataSource.Account = null;

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 200, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.AccountNotInitialized);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenAccountCardIsNotActive()
        {
            dataSource.Account = new Account(availableLimit: 100, activeCard: false);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 50, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.CardNotActive);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenAccountHasInsufficientLimit()
        {
            dataSource.Account = new Account(
                availableLimit: 380, activeCard: false
            );

            var transaction = new Transaction(
                merchant: "Some merchant", amount: 420, time: 05.February(2021)
            );

            var violations = service.Authorize(transaction);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.InsufficientLimit);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenTransactionsExceedThreeInATwoMinuteInterval()
        {
            dataSource.Account = new Account(availableLimit: 1000, activeCard: true);

            DateTime now = 5.February(2021);
            DateTime minutesAgo(int minutes) => now.AddMinutes(-minutes);
            DateTime someHoursAgo = minutesAgo(60 * 3);

            GenerateRandomTransactions(
                from: someHoursAgo, to: minutesAgo(2),
                30, 20, 10, 20, 30
            );

            GenerateRandomTransactions(
                from: minutesAgo(2), to: now,
                25, 25, 25
            );

            var transaction = new Transaction(
                merchant: "Some merchant", amount: 10, time: now
            );

            var violations = service.Authorize(transaction);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.HighFrequencySmallInterval);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenTransactionIsDoubledInATwoMinuteInterval()
        {
            dataSource.Account = new Account(availableLimit: 1000, activeCard: true);

            var merchant = "BurgBurgBurg";
            var amount = 123;

            DateTime now = 5.February(2021);
            DateTime minutesAgo(int minutes) => now.AddMinutes(-minutes);
            DateTime someHoursAgo = minutesAgo(60 * 3);

            GenerateRandomTransactions(
                from: someHoursAgo, to: minutesAgo(2),
                10, 20, 30
            );

            dataSource.Transactions = dataSource.Transactions.Append(
                new Transaction(merchant, amount, time: minutesAgo(1))
            );

            GenerateRandomTransactions(
                from: minutesAgo(1), to: now,
                25
            );

            var doubledTransaction = new Transaction(merchant, amount, time: now);

            var violations = service.Authorize(doubledTransaction);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.DoubledTransaction);
        }

        [Fact]
        public void Authorize_ShouldNotReturnViolation_WhenTransactionIsDoubledAfterATwoMinuteInterval()
        {
            dataSource.Account = new Account(availableLimit: 1000, activeCard: true);

            var merchant = "BurgBurgBurg";
            var amount = 123;

            DateTime now = 5.February(2021);
            DateTime minutesAgo(int minutes) => now.AddMinutes(-minutes);
            DateTime someHoursAgo = minutesAgo(60 * 3);

            GenerateRandomTransactions(
                from: someHoursAgo, to: minutesAgo(3),
                10, 20, 30
            );

            dataSource.Transactions = dataSource.Transactions.Append(
                new Transaction(merchant, amount, time: minutesAgo(3))
            );

            GenerateRandomTransactions(
                from: minutesAgo(1), to: now,
                25
            );

            var transaction = new Transaction(merchant, amount, time: now);

            var violations = service.Authorize(transaction);

            violations
                .Should().BeEmpty();
        }

        [Fact]
        public void Authorize_ShouldReturnMultipleViolations_WhenTransactionIsDoubledAndExceedsAccountLimitAndTransactionQuantityLimit()
        {
            dataSource.Account = new Account(availableLimit: 1000, activeCard: true);

            var merchant = "BurgBurgBurg";
            var amount = 300;

            DateTime now = 5.February(2021);
            DateTime minutesAgo(int minutes) => now.AddMinutes(-minutes);
            DateTime someHoursAgo = minutesAgo(60 * 3);

            GenerateRandomTransactions(
                from: someHoursAgo, to: minutesAgo(1),
                200, 100, 200
            );

            dataSource.Transactions = dataSource.Transactions.Append(
                new Transaction(merchant, amount, time: minutesAgo(1))
            );

            GenerateRandomTransactions(
                from: minutesAgo(1), to: now,
                100, 100
            );

            var transaction = new Transaction(merchant, amount, time: now);

            var violations = service.Authorize(transaction);

            violations
                .Should().HaveCount(3)
                .And
                .BeEquivalentTo(new[] {
                    Violation.DoubledTransaction,
                    Violation.HighFrequencySmallInterval,
                    Violation.InsufficientLimit,
                });
        }
    }
}
