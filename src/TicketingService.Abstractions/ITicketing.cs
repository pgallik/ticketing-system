namespace TicketingService.Abstractions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface ITicketing
{
    Task<Guid> CreateTicket(string originator, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ticket>> GetAll(CancellationToken cancellationToken = default);
    Task<Ticket?> Get(Guid ticketId, CancellationToken cancellationToken = default);
    Task Pending(Guid ticketId, CancellationToken cancellationToken = default);
    Task Complete(Guid ticketId, TicketResult result, CancellationToken cancellationToken = default);
    Task Delete(Guid ticketId, CancellationToken cancellationToken = default);
}
