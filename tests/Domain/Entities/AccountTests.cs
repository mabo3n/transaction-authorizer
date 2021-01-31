using System;
using FluentAssertions;
using Xunit;
using Authorizer.Domain.Entities;

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
                .BeOfType<ArgumentException>()
                .Which.Message
                .Should()
                .MatchEquivalentOf("*limit*");
        }

    }
}
