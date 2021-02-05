using Xunit;
using Authorizer.Api;
using Authorizer.Application;
using Authorizer.Domain.Services;
using Authorizer.Infrastructure;
using Authorizer;
using System;
using System.IO;
using System.Threading;
using FluentAssertions;

namespace AuthorizerTests.Integration
{
    public class ConsoleHostTests
    {
        private JsonStringParser jsonParser;
        private IOperationHandler<CreateAccount> createAccountHandler;
        private IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler;
        private ConsoleHost host;

        public ConsoleHostTests()
        {
            var dataSource = new InMemoryDataSource();
            var accountRepository = new AccountRepository(dataSource);
            var transactionRepository = new TransactionRepository(dataSource);
            var transactionService = new TransactionService(transactionRepository);

            jsonParser = new JsonStringParser();
            createAccountHandler = new CreateAccountHandler(accountRepository);
            authorizeTransactionHandler = new AuthorizeTransactionHandler(
                accountRepository, transactionRepository, transactionService
            );

            host = new ConsoleHost(
                jsonParser, createAccountHandler, authorizeTransactionHandler
            );
        }

        [Fact]
        public async void Host_ShouldDisplayViolations()
        {
            string Trimmed(string output) => output
                .Trim()
                .Trim(Environment.NewLine.ToCharArray());

            string ToValidJson(string jsonLike)
                => Trimmed(jsonLike).Replace('\'', '"');

            var input = ToValidJson(@"
{'transaction': {'merchant': 'Merchant 001', 'amount': 15, 'time': '2021-02-01T10:32:30.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:32:30.000Z'}}
{'transaction': {'merchant': 'Merchant 003', 'amount': 20, 'time': '2021-02-01T10:33:10.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:34:40.000Z'}}
{'account': {'active-card': true, 'available-limit': 30}}
{'transaction': {'merchant': 'Merchant 001', 'amount': 10, 'time': '2021-02-01T10:35:50.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:36:40.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:37:10.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:37:30.000Z'}}
{'transaction': {'merchant': 'Merchant 004', 'amount': 20, 'time': '2021-02-01T10:40:00.000Z'}}
{'account': {'active-card': false, 'available-limit': 200}}
{'transaction': {'merchant': 'Merchant 005', 'amount': 1, 'time': '2021-02-01T10:41:00.000Z'}}
{'transaction': {'merchant': 'Merchant 005', 'amount': 2, 'time': '2021-02-01T10:41:30.000Z'}}
{'transaction': {'merchant': 'Merchant 005', 'amount': 3, 'time': '2021-02-01T10:42:00.000Z'}}
{'transaction': {'merchant': 'Merchant 005', 'amount': 4, 'time': '2021-02-01T10:42:30.000Z'}}
            ");

            var expectedOutput = ToValidJson(@"
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':{'active-card':true,'available-limit':30},'violations':[]}
{'account':{'active-card':true,'available-limit':20},'violations':[]}
{'account':{'active-card':true,'available-limit':10},'violations':[]}
{'account':{'active-card':true,'available-limit':10},'violations':['doubled-transaction']}
{'account':{'active-card':true,'available-limit':10},'violations':['doubled-transaction']}
{'account':{'active-card':true,'available-limit':10},'violations':['insufficient-limit']}
{'account':{'active-card':true,'available-limit':10},'violations':['account-already-initialized']}
{'account':{'active-card':true,'available-limit':9},'violations':[]}
{'account':{'active-card':true,'available-limit':7},'violations':[]}
{'account':{'active-card':true,'available-limit':4},'violations':[]}
{'account':{'active-card':true,'available-limit':4},'violations':['high-frequency-small-interval']}
            ");

            using (var stringReader = new StringReader(input))
            using (var stringWriter = new StringWriter())
            {
                Console.SetIn(stringReader);
                Console.SetOut(stringWriter);

                await host.StartAsync(CancellationToken.None);

                var output = Trimmed(stringWriter.ToString());

                output.Should().Be(expectedOutput);
            }
        }
    }
}
