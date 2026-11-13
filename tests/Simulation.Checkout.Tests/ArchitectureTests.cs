using FluentAssertions;
using NetArchTest.Rules;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Infrastructure;

namespace Simulation.Checkout.Tests;

public class ArchitectureTests
{
    private const string ApplicationNamespace = "Simulation.Checkout.Application";
    private const string InfrastructureNamespace = "Simulation.Checkout.Infrastructure";

    [Fact]
    public void DomainLayer_WhenCheckingDependencies_ShouldNotDependOnApplicationOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(Order).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_WhenCheckingDependencies_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(CheckoutService).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Interfaces_WhenCheckingNamingConventions_ShouldStartWithI()
    {
        var assemblies = new[]
        {
            typeof(Order).Assembly,
            typeof(CheckoutService).Assembly,
            typeof(InMemoryOrderRepository).Assembly
        };

        var result = Types.InAssemblies(assemblies)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Repositories_WhenImplementingIOrderRepository_ShouldEndWithRepository()
    {
        var result = Types.InAssembly(typeof(InMemoryOrderRepository).Assembly)
            .That()
            .ImplementInterface(typeof(IOrderRepository))
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
