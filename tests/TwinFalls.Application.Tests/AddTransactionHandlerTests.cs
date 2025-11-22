using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TwinFalls.Application.Interfaces;
using TwinFalls.Application.UseCases.AddTransaction;
using Xunit;

namespace TwinFalls.Application.Tests
{
    public class AddTransactionHandlerTests
    {
        [Fact]
        public async Task Handle_calls_repo_and_uow()
        {
            var txRepo = new Mock<ITransactionRepository>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            var handler = new AddTransactionHandler(txRepo.Object, uow.Object);
            var cmd = new AddTransactionCommand(Guid.NewGuid(), 10m, "EUR", DateTime.UtcNow, TwinFalls.Domain.Enums.TransactionType.Expense, null, "note");

            var id = await handler.Handle(cmd);

            txRepo.Verify(x => x.AddAsync(It.IsAny<TwinFalls.Domain.Entities.Transaction>()), Times.Once);
            uow.Verify(x => x.SaveChangesAsync(), Times.Once);
            id.Should().NotBeEmpty();
        }
    }
}
