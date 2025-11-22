using System;
using FluentAssertions;
using TwinFalls.Domain.ValueObjects;
using Xunit;

namespace TwinFalls.Domain.Tests
{
    public class MoneyTests
    {
        [Fact]
        public void Creating_money_with_nonpositive_amount_throws()
        {
            Action act = () => new Money(0, "USD");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Adding_different_currency_throws()
        {
            var a = new Money(10, "USD");
            var b = new Money(5, "EUR");
            Action act = () => { var c = a + b; };
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
