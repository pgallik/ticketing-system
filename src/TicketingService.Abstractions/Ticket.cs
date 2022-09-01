namespace TicketingService.Abstractions;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public record Ticket(
    Guid TicketId,
    TicketStatus Status,
    IDictionary<string, string> Metadata,
    TicketResult? Result = null)
{
    public TicketStatus Status { get; set; } = Status;
    public TicketResult? Result { get; set; } = Result;
    public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    public void ChangeStatus(TicketStatus newStatus, TicketResult? result = null)
    {
        Status = newStatus;
        LastModified = DateTimeOffset.UtcNow;

        if (Status == TicketStatus.Complete && result is not null)
        {
            Result = result;
        }
    }
}

public enum TicketStatus
{
    Created,
    Pending,
    Complete
}

[JsonConverter(typeof(TicketResultJsonConverter))]
public record TicketResult(object? Result);
