namespace TicketingService.Storage.PgSqlMarten.Tests;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abstractions;
using ContainerHelper;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class MartenTicketingTests
{
    [Fact]
    public async Task CreateGetUpdatePendingErrorCompleteDelete()
    {
        var composeFileName = Path.Combine(Directory.GetCurrentDirectory(), "postgres_test.yml");
        using var _ = Container.Compose(composeFileName, "postgres_test", "5433", "tcp");

        // add Marten
        const string connectionString = "Host=localhost;Port=5433;Database=tickets;Username=postgres;Password=postgres";
        var services = new ServiceCollection();
        services.AddMartenTicketing(connectionString);
        var serviceProvider = services.BuildServiceProvider();

        var ticketing = new MartenTicketing(serviceProvider.GetRequiredService<IDocumentStore>()) as ITicketing;

        // create
        const string originator = "originator";
        var ticketId = await ticketing.CreateTicket(new Dictionary<string, string> { { originator, originator } });

        try
        {
            // get
            var ticket = await ticketing.Get(ticketId);
            Assert.NotNull(ticket);
            Assert.Equal(TicketStatus.Created, ticket!.Status);
            Assert.Equal(originator, ticket.Metadata[originator]);

            // pending
            await ticketing.Pending(ticketId);
            ticket = await ticketing.Get(ticketId);
            Assert.NotNull(ticket);
            Assert.Equal(TicketStatus.Pending, ticket!.Status);

            // error
            var ticketError = new TicketError("mockErrorMessage", "mockErrorCode");
            await ticketing.Error(ticketId, ticketError);

            // get
            ticket = await ticketing.Get(ticketId);
            Assert.NotNull(ticket);
            Assert.Equal(TicketStatus.Error, ticket!.Status);
            Assert.Equal(new TicketResult(ticketError), ticket.Result);

            // complete
            const string complete = "Complete";
            await ticketing.Complete(ticketId, new TicketResult(complete));

            // get
            ticket = await ticketing.Get(ticketId);
            Assert.NotNull(ticket);
            Assert.Equal(TicketStatus.Complete, ticket!.Status);
            Assert.Equal(new TicketResult(complete), ticket.Result);
        }
        finally
        {
            // delete
            await ticketing.Delete(ticketId);
            var ticket = await ticketing.Get(ticketId);
            Assert.Null(ticket);
        }
    }
}
