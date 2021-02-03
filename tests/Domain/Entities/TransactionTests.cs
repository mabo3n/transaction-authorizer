using System;
using FluentAssertions;
using Xunit;
using Authorizer.Domain.Entities;
using FluentAssertions.Extensions;

namespace AuthorizerTests.Domain.Entities
{
    public class TransactionTests
    {
        [Fact]
        public void IsSimilarTo_ShouldReturnTrue_WhenMerchantAndAmountMatch()
        {
            var transaction = new Transaction(
                merchant: "Merchant X", amount: 100, time: 01.January(2001)
            );

            var someSimilarTransaction = new Transaction(
                merchant: "Merchant X", amount: 100, time: 25.May(1999)
            );

            transaction.IsSimilarTo(someSimilarTransaction)
                .Should().BeTrue();
        }

        [Fact]
        public void IsSimilarTo_ShouldReturnFalse_WhenMerchantDoesNotMatch()
        {
            var transaction = new Transaction(
                merchant: "Merchant X", amount: 100, time: 01.January(2001)
            );

            var someSimilarTransaction = new Transaction(
                merchant: "Merchant Y", amount: 100, time: 01.January(2001)
            );

            transaction.IsSimilarTo(someSimilarTransaction)
                .Should().BeFalse();
        }
    }
}
