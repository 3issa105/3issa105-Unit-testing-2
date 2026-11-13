# Unit Testing & Architecture Testing — Mini Checkout Engine
**Simulation Academy • .NET Diploma • Cycle 1**

- **Student Name:** Ahmed Mohamed Eissa
- **GitHub / ID:** [@3issa105](https://github.com/3issa105)
- **Track:** Backend .NET
- **Assignment:** Unit testing 2 (Branch: `assignment/1-11`)

---

## Overview

This repository contains automated tests for the DriveNow / Mini Checkout Engine:
- **Task 1: Business Rules** (`CheckoutServiceTests.cs`) — Validation, discount boundaries, money calculations.
- **Task 2: Mocks** (`CheckoutServiceTests.cs`) — Verifying dependency calls (`IOrderRepository`, `IInventoryService`, `INotificationService`).
- **Task 3: Test Data** (`TestData/`) — AutoFixture customization and Bogus with fixed seed.
- **Task 4: Snapshot Testing** (`ReceiptSnapshotTests.cs`) — Stable snapshot verification of `CheckoutReceipt`.
- **Task 5: Architecture Tests** (`ArchitectureTests.cs`) — Layer boundaries and naming conventions via NetArchTest.
- **Task 6: Code Coverage** (`coverage-report/`) — Coverlet collection and ReportGenerator HTML report.

## Running Tests

```bash
dotnet test
```

## Collecting Coverage & Report Generation

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;TextSummary"
```
