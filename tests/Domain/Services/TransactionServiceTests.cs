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
        private TransactionService service;
        private Random random = new Random(Seed: 12345);

        public TransactionServiceTests()
        {
            dataSource = new InMemoryDataSource();
            var transactionRepository =  new TransactionRepository(dataSource);
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
            var account = new Account(availableLimit: 100, activeCard: true);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 200, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction, account);

            violations
                .Should().BeEmpty();
        }

        [Fact]
        public void Authorize_ShouldReturnException_WhenAccountIsNull()
        {
            Account account = null;

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 200, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction, account);

            var exception = Record.Exception(
                () => account.Apply(someTransaction)
            );

            exception
                .Should()
                .BeOfType<ArgumentException>()
                .Which.Message
                .Should()
                .MatchEquivalentOf("*account*");
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenAccountCardIsNotActive()
        {
            var account = new Account(availableLimit: 100, activeCard: false);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 50, time: 05.February(2021)
            );

            var violations = service.Authorize(someTransaction, account);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.CardNotActive);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenAccountHasInsufficientLimit()
        {
            var account = new Account(
                availableLimit: 380, activeCard: false
            );

            var transaction = new Transaction(
                merchant: "Some merchant", amount: 420, time: 05.February(2021)
            );

            var violations = service.Authorize(transaction, account);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.InsufficientLimit);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenTransactionsExceedThreeInATwoMinuteInterval()
        {
            var account = new Account(availableLimit: 1000, activeCard: true);

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

            var violations = service.Authorize(transaction, account);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.HighFrequencySmallInterval);
        }

        [Fact]
        public void Authorize_ShouldReturnViolation_WhenTransactionIsDoubledInATwoMinuteInterval()
        {
            var account = new Account(availableLimit: 1000, activeCard: true);

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

            var violations = service.Authorize(doubledTransaction, account);

            violations
                .Should().HaveCount(1)
                .And
                .Contain(Violation.DoubledTransaction);
        }

        [Fact]
        public void Authorize_ShouldNotReturnViolation_WhenTransactionIsDoubledAfterATwoMinuteInterval()
        {
            var account = new Account(availableLimit: 1000, activeCard: true);

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

            var violations = service.Authorize(transaction, account);

            violations
                .Should().BeEmpty();
        }

        [Fact]
        public void Authorize_ShouldReturnMultipleViolations_WhenTransactionIsDoubledAndExceedsAccountLimitAndTransactionQuantityLimit()
        {
            var account = new Account(availableLimit: 1000, activeCard: true);

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

            var violations = service.Authorize(transaction, account);

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
