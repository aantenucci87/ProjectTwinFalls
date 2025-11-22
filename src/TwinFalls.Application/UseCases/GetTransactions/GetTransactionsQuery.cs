using System;

namespace TwinFalls.Application.UseCases.GetTransactions
{
    public record GetTransactionsQuery(DateTime FromUtc, DateTime ToUtc);
}
