using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Domain.Exceptions;

namespace Simulation.Checkout.Tests;

public class CheckoutServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IInventoryService> _inventoryServiceMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly CheckoutService _sut;

    public CheckoutServiceTests()
    {
        _inventoryServiceMock
            .Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _sut = new CheckoutService(
            _orderRepositoryMock.Object,
            _inventoryServiceMock.Object,
            _notificationServiceMock.Object);
    }

    // --- Task 1: Business Rules (xUnit, FluentAssertions) ---

    [Fact]
    public async Task CheckoutAsync_WhenOrderIsEmpty_ShouldThrowEmptyOrderException()
    {
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItem>()
        };

        var act = async () => await _sut.CheckoutAsync(order);

        await act.Should().ThrowAsync<EmptyOrderException>()
            .WithMessage("An order must contain at least one item.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CheckoutAsync_WhenItemQuantityIsZeroOrNegative_ShouldThrowInvalidQuantityException(int quantity)
    {
        const string productName = "Wireless Mouse";
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = productName,
                    UnitPrice = 50m,
                    Quantity = quantity
                }
            }
        };

        var act = async () => await _sut.CheckoutAsync(order);

        var exceptionAssertion = await act.Should().ThrowAsync<InvalidQuantityException>();
        exceptionAssertion.WithMessage($"Quantity for '{productName}' must be greater than zero.");
        exceptionAssertion.Which.Message.Should().Contain(productName);
    }

    [Fact]
    public async Task CheckoutAsync_WhenItemPriceIsNegative_ShouldThrowInvalidPriceException()
    {
        const string productName = "USB-C Cable";
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = productName,
                    UnitPrice = -0.01m,
                    Quantity = 1
                }
            }
        };

        var act = async () => await _sut.CheckoutAsync(order);

        var exceptionAssertion = await act.Should().ThrowAsync<InvalidPriceException>();
        exceptionAssertion.WithMessage($"Unit price for '{productName}' cannot be negative.");
        exceptionAssertion.Which.Message.Should().Contain(productName);
    }

    [Fact]
    public async Task CheckoutAsync_WhenItemPriceIsZero_ShouldSucceed()
    {
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Promotional Sticker",
                    UnitPrice = 0m,
                    Quantity = 2
                }
            }
        };

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Should().NotBeNull();
        receipt.Subtotal.Should().Be(0m);
        receipt.Discount.Should().Be(0m);
        receipt.Total.Should().Be(0m);
    }

    [Theory]
    [InlineData(999.99, 0)]
    [InlineData(1000.00, 100)]
    [InlineData(1500.00, 100)]
    public async Task CheckoutAsync_WhenSubtotalQualifiesForDiscount_ShouldApplyExpectedDiscount(
        decimal subtotal,
        decimal expectedDiscount)
    {
        var order = new Order
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Item",
                    UnitPrice = subtotal,
                    Quantity = 1
                }
            }
        };

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Subtotal.Should().Be(subtotal);
        receipt.Discount.Should().Be(expectedDiscount);
        receipt.Total.Should().Be(subtotal - expectedDiscount);
    }

    [Fact]
    public async Task CheckoutAsync_WhenGivenExampleA_ShouldCalculateCorrectTotals()
    {
        // Example A: 2 × 150 + 1 × 400 -> Subtotal 700, Discount 0, Total 700
        var order = new Order
        {
            CustomerName = "Alice",
            CustomerEmail = "alice@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Item 1",
                    UnitPrice = 150m,
                    Quantity = 2
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Item 2",
                    UnitPrice = 400m,
                    Quantity = 1
                }
            }
        };

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Subtotal.Should().Be(700m);
        receipt.Discount.Should().Be(0m);
        receipt.Total.Should().Be(700m);
    }

    [Fact]
    public async Task CheckoutAsync_WhenGivenExampleC_ShouldCalculateCorrectTotals()
    {
        // Example C: 3 × 500 -> Subtotal 1500, Discount 100 (capped at max 100, not 150), Total 1400
        var order = new Order
        {
            CustomerName = "Charlie",
            CustomerEmail = "charlie@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Item 1",
                    UnitPrice = 500m,
                    Quantity = 3
                }
            }
        };

        var receipt = await _sut.CheckoutAsync(order);

        receipt.Subtotal.Should().Be(1500m);
        receipt.Discount.Should().Be(100m);
        receipt.Total.Should().Be(1400m);
    }
}
