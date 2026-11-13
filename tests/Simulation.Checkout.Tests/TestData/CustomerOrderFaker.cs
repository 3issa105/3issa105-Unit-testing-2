using Bogus;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

public static class CustomerOrderFaker
{
    public const int DefaultSeed = 42;

    public static Order CreateOrderWithSeed(int seed = DefaultSeed)
    {
        var faker = new Faker<Order>()
            .UseSeed(seed)
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.CustomerName, f => f.Person.FullName)
            .RuleFor(o => o.CustomerEmail, f => f.Person.Email)
            .RuleFor(o => o.Items, f => new List<OrderItem>
            {
                new()
                {
                    ProductId = f.Random.Guid(),
                    ProductName = f.Commerce.ProductName(),
                    UnitPrice = 100m,
                    Quantity = 2
                }
            });

        return faker.Generate();
    }
}
