using System;
using System.Threading.Tasks;
using TwinFalls.Application.Interfaces;
using TwinFalls.Domain.Entities;
using TwinFalls.Domain.ValueObjects;

namespace TwinFalls.Application.UseCases.AddTransaction
{
    public class AddTransactionHandler
    {
        private readonly ITransactionRepository _txRepo;
        private readonly IUnitOfWork _uow;

        public AddTransactionHandler(ITransactionRepository txRepo, IUnitOfWork uow)
        {
            _txRepo = txRepo;
            _uow = uow;
        }

        public async Task<Guid> Handle(AddTransactionCommand command)
        {
            var money = new Money(command.Amount, command.Currency);
            var tx = new Transaction(command.AccountId, money, command.Date, command.Type, command.CategoryId, command.Note);
            await _txRepo.AddAsync(tx);
            await _uow.SaveChangesAsync();
            return tx.Id;
        }
    }
}
