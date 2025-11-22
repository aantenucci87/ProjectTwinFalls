using System;
using TwinFalls.Domain.Enums;

namespace TwinFalls.Application.DTOs
{
    public record TransactionDto(Guid Id, Guid AccountId, decimal Amount, string Currency, DateTime Date, TransactionType Type, Guid? CategoryId, string? Note);
}
