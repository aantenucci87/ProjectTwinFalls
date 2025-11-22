using System.Collections.Generic;
using System.Threading.Tasks;
using TwinFalls.Application.DTOs;
using TwinFalls.Application.Interfaces;

namespace TwinFalls.Application.UseCases.GetTransactions
{
    public class GetTransactionsHandler
    {
        private readonly ITransactionRepository _txRepo;

        public GetTransactionsHandler(ITransactionRepository txRepo)
        {
            _txRepo = txRepo;
        }

        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsQuery query)
        {
            var list = await _txRepo.GetByPeriodAsync(query.FromUtc, query.ToUtc);
            var dtos = new List<TransactionDto>();
            foreach (var t in list)
            {
                dtos.Add(new TransactionDto(t.Id, t.AccountId, t.Value.Amount, t.Value.Currency, t.Date, t.Type, t.CategoryId, t.Note));
            }

            return dtos;
        }
    }
}
