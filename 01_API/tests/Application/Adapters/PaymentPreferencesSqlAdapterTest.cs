// <copyright file="PaymentPreferencesSqlAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Adapters;

using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// Unit tests for payment preference methods on <see cref="PaymentPreferencesSqlAdapter"/>.
/// </summary>
public sealed class PaymentPreferencesSqlAdapterTest
{
    /// <summary>
    /// Verifies that account existence is delegated to the payment preference repository.
    /// </summary>
    [Fact]
    public async Task AccountExistsAsync_WhenRepositoryReturnsTrue_ReturnsTrue()
    {
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.AccountExistsAsync(42);

        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that account number retrieval is delegated to the payment preference repository.
    /// </summary>
    [Fact]
    public async Task GetAccountNumberAsync_WhenRepositoryReturnsValue_ReturnsValue()
    {
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository.Setup(candidate => candidate.GetAccountNumberAsync(42)).ReturnsAsync("1001102412");
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.GetAccountNumberAsync(42);

        result.Should().Be("1001102412");
    }

    /// <summary>
    /// Verifies that a missing payment preference maps to null.
    /// </summary>
    [Fact]
    public async Task GetByAccountIdAsync_WhenPreferenceDoesNotExist_ReturnsNull()
    {
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreferenceDb?)null);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.GetByAccountIdAsync(42);

        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a database payment preference maps to the domain model.
    /// </summary>
    [Fact]
    public async Task GetByAccountIdAsync_WhenPreferenceExists_MapsDomainPreference()
    {
        var createdAt = DateTime.UtcNow;
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository
            .Setup(candidate => candidate.GetByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreferenceDb
            {
                Id = 7,
                AccountId = 42,
                PaymentType = 2,
                CreatedAt = createdAt,
                CreatedBy = "user@test.fr"
            });
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.GetByAccountIdAsync(42);

        result.Should().NotBeNull();
        result!.Id.Should().Be(7);
        result.AccountId.Should().Be(42);
        result.PaymentType.Should().Be(2);
        result.CreatedAt.Should().Be(createdAt);
        result.CreatedBy.Should().Be("user@test.fr");
    }

    /// <summary>
    /// Verifies that a domain payment preference maps to the database model when saved.
    /// </summary>
    [Fact]
    public async Task SaveAsync_WhenPreferenceIsProvided_MapsDatabasePreference()
    {
        PaymentPreferenceDb? savedPreference = null;
        var createdAt = DateTime.UtcNow;
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository
            .Setup(candidate => candidate.SaveAsync(It.IsAny<PaymentPreferenceDb>()))
            .Callback<PaymentPreferenceDb>(preference => savedPreference = preference)
            .Returns(Task.CompletedTask);
        var adapter = CreateAdapter(repository.Object);

        await adapter.SaveAsync(new PaymentPreference(7, 42, 2, createdAt, "user@test.fr"));

        savedPreference.Should().NotBeNull();
        savedPreference!.Id.Should().Be(7);
        savedPreference.AccountId.Should().Be(42);
        savedPreference.PaymentType.Should().Be(2);
        savedPreference.CreatedAt.Should().Be(createdAt);
        savedPreference.CreatedBy.Should().Be("user@test.fr");
    }

    /// <summary>
    /// Verifies that saving a null payment preference throws.
    /// </summary>
    [Fact]
    public async Task SaveAsync_WhenPreferenceIsNull_Throws()
    {
        var adapter = CreateAdapter(Mock.Of<IPaymentPreferenceRepository>());

        Func<Task> act = async () => await adapter.SaveAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that cleanup is delegated to the payment preference repository.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenRepositoryReturnsDeletedRows_ReturnsDeletedRows()
    {
        var repository = new Mock<IPaymentPreferenceRepository>();
        repository.Setup(candidate => candidate.CleanupOnboardingDataAsync(42)).ReturnsAsync(2);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.CleanupAsync(42);

        result.Should().Be(2);
        repository.Verify(candidate => candidate.CleanupOnboardingDataAsync(42), Times.Once);
    }

    /// <summary>
    /// Creates the payment preference SQL adapter under test.
    /// </summary>
    /// <param name="repository">The payment preference repository.</param>
    /// <returns>The adapter.</returns>
    private static PaymentPreferencesSqlAdapter CreateAdapter(IPaymentPreferenceRepository repository)
    {
        return new PaymentPreferencesSqlAdapter(repository, Mock.Of<ISepaMandateRepository>());
    }
}
