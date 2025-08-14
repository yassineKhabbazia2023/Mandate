using System.Data;
using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Models;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
using Microsoft.DurableTask;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Org.BouncyCastle.Asn1.X509;

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

public class MandateCreationOrchestrationTest
{
    private readonly Mock<IJeDeclareService> _jeDeclareClient;
    private readonly Mock<IMandateRepository> _mandateRepository;
    private readonly Mock<IDatabaseService> _dbService;
    private readonly MandateCreationOrchestration _sut;
    private readonly int _companyId;
    private readonly Guid _collectionDbId;
    private readonly MandateCreationMessage _mandateCreationMessage;
    private readonly Company _company;
    private readonly Bban _bban;
    private readonly MandateCreationMessageAndCollectionId _mandateCreationMessageAndCollectionId;
    private readonly MandateCreationMessageAndCompanyAndCollectionId _mandateCreationMessageAndCompanyAndCollectionId;
    private readonly MandateCreationMessageAndCompanyAndMore _mandateCreationMessageAndCompanyAndMore;

    public MandateCreationOrchestrationTest()
    {
        _jeDeclareClient = new Mock<IJeDeclareService>(MockBehavior.Strict);
        _mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);
        _dbService = new Mock<IDatabaseService>(MockBehavior.Strict);
        _sut =
            new MandateCreationOrchestration(_jeDeclareClient.Object, _mandateRepository.Object, _dbService.Object);
        _companyId = 666;
        _collectionDbId = Guid.NewGuid();
        _bban = new Bban(string.Empty, string.Empty, string.Empty, string.Empty, null, null);
        _company = new Company(_companyId, null, string.Empty, null, null, null, null);
        _mandateCreationMessage = new MandateCreationMessage
        {
            Id = 0,
            SiretNumber = "sfsf",
            Address = new Address(null, null, null, null, null),
            Bank = new Bank(string.Empty, null, null, null, new BankAgreement(JdcPartnership.Partner)),
            Company = new Company(_companyId, null, string.Empty, null, null, null, null),
            Bban = new Bban(string.Empty, string.Empty, string.Empty, string.Empty, null, null),
            Signatory = new Signatory(null, null, null, null),
        };
        _mandateCreationMessageAndCollectionId =
            new MandateCreationMessageAndCollectionId(_mandateCreationMessage, _collectionDbId);
        _mandateCreationMessageAndCompanyAndCollectionId =
            new MandateCreationMessageAndCompanyAndCollectionId(_mandateCreationMessage, _company, _collectionDbId);
        _mandateCreationMessageAndCompanyAndMore = new MandateCreationMessageAndCompanyAndMore(
            _mandateCreationMessage,
            _company,
            new CollectionIdAndRib(
                _collectionDbId,
                _bban));
    }

    [Fact]
    public async Task GetCollection_Should_Return_ExistingCollection()
    {
        _mandateRepository
            .Setup(t => t
                .GetCollectionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _companyId))
            .Returns(Task.FromResult<CollectionDb?>(new CollectionDb
            {
                Id = _collectionDbId,
                CompanyId = _companyId
            }));

        _dbService
      .Setup(d => d.SaveMandateCreationLogMessageAsync(It.IsAny<MandateCreationLogMessage>()))
      .Returns(Task.CompletedTask);

        var result = await _sut.GetCollectionAndSaveMessage(_mandateCreationMessage, new MyFunctionContextStub());
        result.Should().Be(_collectionDbId);
    }

    [Fact]
    public async Task GetCollection_Should_Throw_Exception_If_Collection_Does_Not_Exist()
    {
        _mandateRepository
            .Setup(t => t
                .GetCollectionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _companyId))
            .Returns(Task.FromResult<CollectionDb?>(null));
        var result = () => _sut.GetCollectionAndSaveMessage(_mandateCreationMessage, new MyFunctionContextStub());
        await result.Should().ThrowAsync<CollectionNotFoundException>();
    }

    [Fact]
    public async Task CreateJdcFolderAsync_Should_Return_Company_On_Happy_Path()
    {
        _mandateRepository
            .Setup(t => t
                .GetCollectionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _companyId))
            .Returns(Task.FromResult<CollectionDb?>(null));
        _jeDeclareClient.Setup(i =>
                i.CreateFolderAsync(It.IsAny<Company>(), It.IsAny<Address?>(), It.IsAny<Signatory?>()))
            .ReturnsAsync(_company);
        _mandateRepository.Setup(r => r.CreateOrUpdateFolderAsync(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        var result =
            await _sut.CreateJdcFolderAsync(
                _mandateCreationMessageAndCollectionId,
                new MyFunctionContextStub());

        result.Should().Be(_company);
    }

    [Fact]
    public async Task CreateJdcFolderAsync_Should_Set_Status_To_Creation_Failed_If_An_Exception_Is_Thrown()
    {
        _mandateRepository
            .Setup(t => t
                .GetCollectionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    _companyId))
            .Returns(Task.FromResult<CollectionDb?>(null));
        _jeDeclareClient.Setup(i =>
                i.CreateFolderAsync(It.IsAny<Company>(), It.IsAny<Address?>(), It.IsAny<Signatory?>()))
            .Throws(new Exception("Boum !"));

        var action = () =>
            _sut.CreateJdcFolderAsync(_mandateCreationMessageAndCollectionId, new MyFunctionContextStub());

        await action.Should().ThrowAsync<Exception>();
        _dbService.Verify(
            s =>
                s.CreateStatusAsync(
                    _collectionDbId,
                    (int)JdcCollectionStatus.Creation_Failed),
            Times.Once);
    }

    [Fact]
    public async Task AddRibToJdcFolderAsync_Should_Return_Info_On_Happy_Path()
    {
        _jeDeclareClient.Setup(c =>
                c.AddRibToFolderAsync(It.IsAny<string?>(), It.IsAny<CollectionCreationCommand>(), It.IsAny<Bank>()))
            .ReturnsAsync(_bban);

        var result = await _sut.AddRibToJdcFolderAsync(
            _mandateCreationMessageAndCompanyAndCollectionId,
            new MyFunctionContextStub());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddRibToJdcFolderAsync_Should_Set_Status_To_Creation_Failed_If_An_Exception_Is_Thrown()
    {
        _jeDeclareClient.Setup(c =>
                c.AddRibToFolderAsync(It.IsAny<string?>(), It.IsAny<CollectionCreationCommand>(), It.IsAny<Bank>()))
            .Throws(new Exception("Boum !"));

        var action = () =>
            _sut.AddRibToJdcFolderAsync(
                _mandateCreationMessageAndCompanyAndCollectionId,
                new MyFunctionContextStub());

        await action.Should().ThrowAsync<Exception>();
        _dbService.Verify(
            s =>
                s.CreateStatusAsync(
                    _collectionDbId,
                    (int)JdcCollectionStatus.Creation_Failed),
            Times.Once);
    }

    [Fact]
    public async Task StartCollectAsync_Should_InsertServicesProvier_If_It_Doesnt_Exist()
    {
        _mandateRepository.Setup(r => r.GetServicesProviderIdsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<JeDeclareCollectionDb?>(null));
        var signatory = new Signatory(null, null, null, null);
        var bankServiceProviderId = string.Empty;
        _jeDeclareClient.Setup(c =>
                c.CreateCollecteConfigurationAsync(It.IsAny<Company>(), It.IsAny<Bban>(), It.IsAny<Signatory>(),
                    It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("yolo");
        _mandateRepository.Setup(c =>
                c.InsertServicesProviderIdsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result =
            await _sut.StartCollectAsync(
                _mandateCreationMessageAndCompanyAndMore,
                new MyFunctionContextStub());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StartCollectAsync_Should_Not_InsertServicesProvier_If_It_Exists()
    {
        var message = new MandateCreationMessageAndCompanyAndMore(
            _mandateCreationMessage,
            _company,
            new CollectionIdAndRib(
                _collectionDbId,
                _bban));
        _mandateRepository.Setup(r => r.GetServicesProviderIdsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<JeDeclareCollectionDb?>(new JeDeclareCollectionDb()));
        _jeDeclareClient.Setup(c =>
                c.CreateCollecteConfigurationAsync(
                    It.IsAny<Company>(),
                    It.IsAny<Bban>(),
                    It.IsAny<Signatory>(),
                    It.IsAny<string>(),It.IsAny<string>()))
            .ThrowsAsync(new Exception("That should not happens !"));

        var result = await _sut.StartCollectAsync(message, new MyFunctionContextStub());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StartCollectAsync_Should_Set_Status_To_Creation_Failed_If_An_Exception_Is_Thrown()
    {
        _mandateRepository.Setup(r => r.GetServicesProviderIdsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult<JeDeclareCollectionDb?>(null));
        var signatory = new Signatory(null, null, null, null);
        var bankServiceProviderId = string.Empty;
        _jeDeclareClient.Setup(c =>
                c.CreateCollecteConfigurationAsync(
                    It.IsAny<Company>(),
                    It.IsAny<Bban>(),
                    It.IsAny<Signatory>(),
                    It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Boum !"));

        var action = () =>
            _sut.StartCollectAsync(_mandateCreationMessageAndCompanyAndMore, new MyFunctionContextStub());

        await action.Should().ThrowAsync<Exception>();
        _dbService.Verify(
            s =>
                s.CreateStatusAsync(
                    _collectionDbId,
                    (int)JdcCollectionStatus.Creation_Failed),
            Times.Once);
    }

    [Fact]
    public async Task SaveSignatoryAndAddStatusAsync_Should_Not_Add_Signatory_If_It_Already_Exists()
    {
        _mandateRepository.Setup(r => r.GetCollectionSignatoryAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync(new PersonalDb());
        _dbService.Setup(s => s.CreateStatusAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.FromResult<Status?>(null));

        var result = () =>
            _sut.SaveSignatoryAndAddStatusAsync(
                _mandateCreationMessageAndCompanyAndMore,
                new MyFunctionContextStub());
        await result.Should().NotThrowAsync();
        _dbService.Verify(
            s =>
                s.CreateStatusAsync(
                    _collectionDbId,
                    (int)JdcCollectionStatus.Activation_Requested_Collection_Pending),
            Times.Once);
    }

    [Fact]
    public async Task SaveSignatoryAndAddStatusAsync_Should_Add_Signatory_If_It_Doesnt_Exist()
    {
        _mandateRepository.Setup(r => r.GetCollectionSignatoryAsync(
                It.IsAny<Guid>()))
            .Returns(Task.FromResult<PersonalDb?>(null));
        _mandateRepository.Setup(r => r.SaveSignatoryAsync(
            It.IsAny<PersonalDb>())).Returns(Task.CompletedTask);
        _dbService.Setup(s => s.CreateStatusAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.FromResult<Status?>(null));

        var result = () =>
            _sut.SaveSignatoryAndAddStatusAsync(
                _mandateCreationMessageAndCompanyAndMore,
                new MyFunctionContextStub());
        await result.Should().NotThrowAsync();
        _dbService.Verify(
            s =>
                s.CreateStatusAsync(
                    _collectionDbId,
                    (int)JdcCollectionStatus.Activation_Requested_Collection_Pending),
            Times.Once);
    }
}