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
using System.Text;

namespace AuthorizerTests.Integration
{
    public class ConsoleHostTests
    {
        private IOperationHandler<CreateAccount> createAccountHandler;
        private IOperationHandler<AuthorizeTransaction> authorizeTransactionHandler;
        private ConsoleHost host;
        private JsonStringParser jsonParser;

        public ConsoleHostTests()
        {
            var dataSource = new InMemoryDataSource();
            var accountRepository = new AccountRepository(dataSource);
            var transactionRepository = new TransactionRepository(dataSource);

            var transactionService = new TransactionService(transactionRepository);
            var createAccountHandler = new CreateAccountHandler(accountRepository);
            var authorizeTransactionHandler = new AuthorizeTransactionHandler(
                accountRepository, transactionRepository, transactionService
            );

            var jsonParser = new JsonStringParser();
        }

        public ConsoleHost BuildHost()
            => new ConsoleHost(jsonParser, createAccountHandler, authorizeTransactionHandler);

        [Fact]
        public async void Host_ShouldDisplayViolations()
        {
            var input = @"
{'transaction': {'merchant': 'Merchant 001', 'amount': 10, 'time': '2021-02-01T10:32:30.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:32:30.000Z'}}
{'transaction': {'merchant': 'Merchant 003', 'amount': 00, 'time': '2021-02-01T10:33:10.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:34:40.000Z'}}
{'account': {'active-card': true, 'available-limit': 20}}
{'transaction': {'merchant': 'Merchant 001', 'amount': 10, 'time': '2021-02-01T10:35:50.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:36:40.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:37:10.000Z'}}
{'transaction': {'merchant': 'Merchant 002', 'amount': 10, 'time': '2021-02-01T10:37:30.000Z'}}
{'transaction': {'merchant': 'Merchant 004', 'amount': 10, 'time': '2021-02-01T10:40:00.000Z'}}
{'account': {'active-card': false, 'available-limit': 200}}
            ".Replace('\'', '"').Trim(Environment.NewLine.ToCharArray());

            var expectedOutput = @"
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':null,'violations':['account-not-initialized']}
{'account':{'active-card':true,'available-limit':20},'violations':[]}
{'account':{'active-card':true,'available-limit':10},'violations':[]}
{'account':{'active-card':true,'available-limit':00},'violations':[]}
{'account':{'active-card':true,'available-limit':00},'violations':['doubled-transation']}
{'account':{'active-card':true,'available-limit':00},'violations':['double-transaction','high-frequency-small-interval']}
{'account':{'active-card':true,'available-limit':00},'violations':['insufficient-limit']}
{'account':{'active-card':true,'available-limit':00},'violations':['account-already-initialized']}
            ".Replace('\'', '"').Trim(Environment.NewLine.ToCharArray());

            var input2 = "{\"transaction\": {\"merchant\": \"Merchant 001\", \"amount\": 10, \"time\": \"2021-02-01T10:32:30.000Z\"}}";
            using (var stringReader = new StringReader(input2))
            using (var stringWriter = new StringWriter())
            {
                Console.SetIn(stringReader);
                Console.SetOut(stringWriter);

                await BuildHost().StartAsync(CancellationToken.None);

                stringWriter.ToString().Should().Be(input2);
            }
        }
    }
}
