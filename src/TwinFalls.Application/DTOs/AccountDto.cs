using System;

namespace TwinFalls.Application.DTOs
{
    public record AccountDto(Guid Id, string Name, decimal Balance, string Currency);
}
