using System.Reflection;
using BankingServiceCLI.Exceptions;
using BankingServiceCLI.Extensions;
using BankingServiceCLI.Models;
using BankingServiceCLI.Services;

namespace BankingServiceCLI.Tests;

//тесты для ClientService
public class ClientServiceTests
{
    private readonly BankData _data;
    private readonly ClientService _clientService;
    private readonly Client _testClient;
    private readonly Account _testAccount;

    //вызывается перед каждым тестом
    public ClientServiceTests()
    {
        _testAccount = new Account(1000m, Currency.RUB, AccountType.Current);
        _testClient = new Client("Tom", "Smith");
        _testClient.Accounts.Add(_testAccount);

        _data = new BankData();
        _data.Clients.Add(_testClient);

        _clientService = new ClientService(_data);
    }

    //получение клиента по id
    [Fact]
    public void GetById_ExistingId_ReturnsClient()
    {
        //Act & Assert
        Assert.Equal(_clientService.GetById(_testClient.Id), _testClient);
    }

    //несуществующий id
    [Fact]
    public void GetById_NonExistingId_ThrowsClientNotFoundException()
    {
        //Arrange
        Guid id = new Guid();

        //Act & Assert
        Assert.Throws<ClientNotFoundException>(
            () => _clientService.GetById(id)
        );
    }

    //добавление валидного клиента
    [Fact]
    public void Add_ValidClient_AddsToList()
    {
        //Arrange
        Client newClient = new Client("Jason", "Smith");

        //Act
        _clientService.Add(newClient);

        //Assert
        Assert.NotNull(_clientService.GetAll().FirstOrDefault(x => x.Id == newClient.Id));
    }

    //добавление невалидного клиента
    [Fact]
    public void Add_InvalidClient_ThrowsArgumentNullException()
    {
        //Arrange
        Client newClient1 = new Client(string.Empty, string.Empty);
        Client newClient2 = new Client("  ", "  ");

        //Assert
        Assert.Throws<ArgumentException>(
            () => _clientService.Add(newClient1)
        );

        Assert.Throws<ArgumentException>(
            () => _clientService.Add(newClient2)
        );
    }

    //подсчет общего баланса
    [Fact]
    public void GetCommonBalanceById_MultipleAccounts_ReturnsSumInRub()
    {
        //Act
        decimal balance = _clientService.GetCommonRubBalanceById(_testClient.Id);

        //Assert
        Assert.Equal(_testClient.Accounts[0].Balance.ToRub(_testClient.Accounts[0].Currency), balance);
    }

    //удаление клиента
    [Fact]
    public void Remove_ExistingClient_RemovesFromList()
    {
        //Act
        _clientService.Remove(_testClient.Id);

        //Assert
        Assert.Null(_clientService.GetAll().FirstOrDefault(x => x.Id == _testClient.Id));
    }

    //удаление несуществующего клиента
    [Fact]
    public void Remove_NonExistingClient_ThrowsClientNotFoundException()
    {
        Guid id = new Guid();


        //Act & Assert
        Assert.Throws<ClientNotFoundException>(
            () => _clientService.Remove(id)
        );
    }

    //валидное обновление клиента
    [Fact]
    public void Update_ValidClient_UpdatesClientData()
    {   
        //Arrange
        Client newClient = new Client("Jason", "Smith");
        
        //Act
        _clientService.Update(newClient, _testClient.Id);

        //Assert
        Assert.Equal(_testClient.Name, newClient.Name);
        Assert.Equal(_testClient.Surname, newClient.Surname);
    }

    //получение пустого списка транзакций
    [Fact]
    public void GetTransactionsById_NoTransactions_ReturnsEmptyList()
    {
        //Assert & Act
        Assert.Empty(_clientService.GetTransactionsById(_testClient.Id, null, null));
    }

    //получение списка транзакций за определенный период времени
    [Fact]
    public void GetTransactionsById_WithPeriod_ReturnsFilteredTransactions()
    {
        //Arrange
        var oldTransaction = new Transaction(TransactionType.Deposit, 100m, null, _testAccount.Id)
        {
            CreatedAt = new DateTime(2024, 1, 1)
        };
        var recentTransaction = new Transaction(TransactionType.Deposit, 200m, null, _testAccount.Id)
        {
            CreatedAt = new DateTime(2024, 6, 1)
        };

        _testAccount.TransactionHistory.Add(oldTransaction);
        _testAccount.TransactionHistory.Add(recentTransaction);

        DateTime fromPeriod = new DateTime(2024, 5, 1);
        DateTime toPeriod = new DateTime(2024, 12, 31);

        //Act
        List<Transaction> result = _clientService.GetTransactionsById(_testClient.Id, fromPeriod, toPeriod);

        //Assert
        Assert.Single(result); // только одна транзакция попала в период
        Assert.Contains(recentTransaction, result);
        Assert.DoesNotContain(oldTransaction, result);
    }
}