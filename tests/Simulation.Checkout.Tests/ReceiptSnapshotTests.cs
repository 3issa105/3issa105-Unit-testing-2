using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests;

public class ReceiptSnapshotTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IInventoryService> _inventoryServiceMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly CheckoutService _sut;

    public ReceiptSnapshotTests()
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
    public async Task CheckoutAsync_WhenGivenExampleC_SnapshotShouldMatchVerifiedReceipt()
    {
        var fixedOrderId = new Guid("11111111-1111-1111-1111-111111111111");
        var fixedProductId = new Guid("22222222-2222-2222-2222-222222222222");

        var order = new Order
        {
            Id = fixedOrderId,
            CustomerName = "Charlie Brown",
            CustomerEmail = "charlie@example.com",
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = fixedProductId,
                    ProductName = "Office Chair",
                    UnitPrice = 500m,
                    Quantity = 3
                }
            }
        };

        var receipt = await _sut.CheckoutAsync(order);

        await Verify(receipt);
    }
}
