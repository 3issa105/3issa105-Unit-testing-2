using AutoFixture;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

public sealed class ValidOrderCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<OrderItem>(composer => composer
            .With(i => i.Quantity, 2)
            .With(i => i.UnitPrice, 150m));

        fixture.Customize<Order>(composer => composer
            .With(o => o.Items, () => fixture.CreateMany<OrderItem>(2).ToList()));
    }
}
