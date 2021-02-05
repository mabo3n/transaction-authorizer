using System;
using FluentAssertions;
using Xunit;
using Authorizer.Domain.Entities;
using FluentAssertions.Extensions;

namespace AuthorizerTests.Domain.Entities
{
    public class AccountTests
    {
        [Fact]
        public void Constructor_ShouldThrowException_WhenAvailableLimitIsNegative()
        {
            var exception = Record.Exception(
                () => new Account(availableLimit: -10, activeCard: true)
            );

            exception
                .Should()
                .BeOfType<InvalidOperationException>()
                .Which.Message
                .Should()
                .MatchEquivalentOf("*limit*");
        }

        [Fact]
        public void Apply_ShouldReturnNewAccountWithUpdatedData()
        {
            var account = new Account(availableLimit: 100, activeCard: true);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 25, time: 03.October(1911)
            );

            var newAccount = account.Apply(someTransaction);

            newAccount.AvailableLimit
                .Should().Be(75);

            newAccount.ActiveCard
                .Should().Be(true);
        }

        [Fact]
        public void Apply_ShouldReturnNewAccountWithUpdatedData_WhenApplyingMultipleTransactions()
        {
            var account = new Account(availableLimit: 100, activeCard: true);

            var transactions = new [] {
                new Transaction(merchant: "A", amount: 25, time: 03.July(2008)),
                new Transaction(merchant: "B", amount: 26, time: 04.July(2008)),
                new Transaction(merchant: "C", amount: 24, time: 05.July(2008)),
                new Transaction(merchant: "D", amount: 25, time: 06.July(2008)),
            };

            var newAccount = account.Apply(transactions);

            newAccount.AvailableLimit
                .Should().Be(0);

            newAccount.ActiveCard
                .Should().Be(true);
        }

        [Fact]
        public void Apply_ShouldThrowException_WhenTransactionAmountExceedsAvailableLimit()
        {
            var account = new Account(availableLimit: 100, activeCard: true);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 800, time: 03.October(1911)
            );

            var exception = Record.Exception(
                () => account.Apply(someTransaction)
            );

            exception
                .Should()
                .BeOfType<InvalidOperationException>()
                .Which.Message
                .Should()
                .MatchEquivalentOf("*limit*");
        }

        [Fact]
        public void Apply_ShouldThrowException_WhenCardIsInactive()
        {
            var account = new Account(availableLimit: 100, activeCard: false);

            var someTransaction = new Transaction(
                merchant: "Some merchant", amount: 20, time: 15.June(1980)
            );

            var exception = Record.Exception(
                () => account.Apply(someTransaction)
            );

            exception
                .Should()
                .BeOfType<InvalidOperationException>()
                .Which.Message
                .Should()
                .MatchEquivalentOf("*card*");
        }
    }
}
