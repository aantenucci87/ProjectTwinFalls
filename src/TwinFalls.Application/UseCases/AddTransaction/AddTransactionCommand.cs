using System;
using TwinFalls.Domain.Enums;

namespace TwinFalls.Application.UseCases.AddTransaction
{
    public record AddTransactionCommand(Guid AccountId, decimal Amount, string Currency, DateTime Date, TransactionType Type, Guid? CategoryId, string? Note);
}
