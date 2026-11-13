using AutoFixture;
using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

public class TestDataTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IInventoryService> _inventoryServiceMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly CheckoutService _sut;

    public TestDataTests()
    {
        _inventoryServiceMock
            .Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _sut = new CheckoutService(
            _orderRepositoryMock.Object,
            _inventoryServiceMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task CheckoutAsync_WhenOrderCreatedWithAutoFixtureCustomization_ShouldSucceed()
    {
        var fixture = new Fixture();
        fixture.Customize(new ValidOrderCustomization());
        var order = fixture.Create<Order>();

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Should().NotBeNull();
        receipt.OrderId.Should().Be(order.Id);
        receipt.Total.Should().Be(receipt.Subtotal - receipt.Discount);
    }

    [Fact]
    public async Task CheckoutAsync_WhenCustomerGeneratedWithBogusFixedSeed_ReceiptShouldHaveSameNameAndEmail()
    {
        var order = CustomerOrderFaker.CreateOrderWithSeed(seed: 42);

        var receipt = await _sut.CheckoutAsync(order);

        receipt.CustomerName.Should().Be(order.CustomerName);
        receipt.CustomerEmail.Should().Be(order.CustomerEmail);
        receipt.CustomerName.Should().NotBeNullOrWhiteSpace();
        receipt.CustomerEmail.Should().Contain("@");
    }
}
