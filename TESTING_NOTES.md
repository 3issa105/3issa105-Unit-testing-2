# Testing Notes

## 1. Total tests, passing, failing
- **Total Tests:** 20
- **Passing:** 20
- **Failing:** 0
- **Skipped:** 0

## 2. Line coverage % (from coverage-report/Summary.txt)
- **Overall Line Coverage:** 90.5%
  - `Simulation.Checkout.Application`: 95.8%
  - `Simulation.Checkout.Domain`: 100%
  - `Simulation.Checkout.Infrastructure`: 0% (Repository implementation is mocked in unit tests)
- **Branch Coverage:** 83.3%

## 3. Libraries used
- **Moq:** Used in `CheckoutServiceTests` and `TestDataTests` to isolate `CheckoutService` from external dependencies (`IOrderRepository`, `IInventoryService`, `INotificationService`) and verify invocation counts (`Times.Once`, `Times.Never`).
- **AutoFixture:** Used in `TestDataTests` with `ValidOrderCustomization` to generate populated `Order` and `OrderItem` hierarchies with valid constraints (`Quantity > 0`, `UnitPrice >= 0`).
- **Bogus:** Used in `CustomerOrderFaker` with a fixed seed (`seed: 42`) to produce realistic customer profiles and product details deterministically across runs.

## 4. Discount fairness (Examples B & C)
- No, it is not fair because of the sharp discount threshold at 1,000 EGP, which gives an instant 100 EGP discount making a 1,000 EGP purchase cost only 900 EGP, whereas a customer spending 999.99 EGP receives 0 discount and pays 99.99 EGP more.

## 5. Potential bugs found
- In `CheckoutService.EnsureInventoryAsync`, inventory availability is verified per line item independently rather than summing quantities for identical `ProductId`s, allowing an order with multiple split lines of the same item to pass validation even if total demand exceeds available stock.
