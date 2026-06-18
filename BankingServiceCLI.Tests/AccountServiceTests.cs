using BankingServiceCLI.Exceptions;
using BankingServiceCLI.Extensions;
using BankingServiceCLI.Models;
using BankingServiceCLI.Services;

namespace BankingServiceCLI.Tests;

//тесты для AccountService
public class AccountServiceTests
{
    private readonly BankData _data;
    private readonly AccountService _accountService;
    private readonly Client _testClient;
    private readonly Account _testAccount;

    //вызывается перед каждым тестом
    public AccountServiceTests()
    {
        _testAccount = new Account(1000m, Currency.RUB, AccountType.Current);
        _testClient = new Client("Tom", "Smith");
        _testClient.Accounts.Add(_testAccount);

        _data = new BankData();
        _data.Clients.Add(_testClient);

        _accountService = new AccountService(_data);
    }


    //пополнение аккаунта валидным значением
    [Fact]
    public void Deposit_ValidAmount_IncreasesBalance()
    {
        //Arrange
        decimal initialBalance = _testAccount.Balance; //1000
        decimal depositAmount = 500m;

        //Act
        _accountService.DepositRub(_testAccount.Id, depositAmount);

        //Assert
        Assert.Equal(initialBalance + depositAmount, _testAccount.Balance);
    }

    //пополнение аккаунта невалидным значением
    [Fact]
    public void Deposit_NegativeAmount_ThrowsArgumentException()
    {
        //Arrange
        decimal depositAmount = -500m;

        //Act & Assert
        Assert.Throws<ArgumentException>(() => _accountService.DepositRub(_testAccount.Id, depositAmount));
    }


    //снятие слишком большой суммы с аккаунта
    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsInsufficientFundsException()
    {
        //Arrange
        decimal withdrawAmount = _testAccount.Balance + 1;

        //Act & Assert
        Assert.Throws<InsufficientFundsException>(() => _accountService.WithdrawRub(_testAccount.Id, withdrawAmount));
    }

    //снятие с аккаунта валидной суммы
    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalance()
    {
        //Arrange
        decimal initialBalance = _testAccount.Balance;
        decimal withdrawAmount = initialBalance - 200m;

        //Act
        _accountService.WithdrawRub(_testAccount.Id, withdrawAmount);

        //Assert
        Assert.Equal(initialBalance - withdrawAmount, _testAccount.Balance);
    }   

    //валидный перевод между двумя аккаунтами 
    [Fact]
    public void Transfer_ValidAmount_MovesMoneyBetweenAccounts()
    {
        //Arrange
        Account secondTestAccount = new Account(2000m, Currency.USD, AccountType.Current);
        Client secondTestClient = new Client("Jason", "Smith");
        secondTestClient.Accounts.Add(secondTestAccount);
        _data.Clients.Add(secondTestClient);

        decimal fromInitBalance = _testAccount.Balance;
        decimal toInitBalance = secondTestAccount.Balance;
        decimal amount = 500m;

        //Act
        _accountService.TransferRub(_testAccount.Id, secondTestAccount.Id, amount);

        //Assert
        Assert.Equal(fromInitBalance - amount.FromRub(_testAccount.Currency), _testAccount.Balance);
        Assert.Equal(toInitBalance + amount.FromRub(secondTestAccount.Currency), secondTestAccount.Balance);
    }

    //невалидный перевод между двумя аккаунтами - один и тот же получатель и отправитель
    [Fact]
    public void Transfer_SameAccount_ThrowsSameAccountTransferException()
    {
        //Arrange
        decimal amount = 0m;

        //Act & Assert
        Assert.Throws<SameAccountTransferException>(
            () => _accountService.TransferRub(_testAccount.Id, _testAccount.Id, amount)
            );

    }

    //валидное закрытия аккаунта
    [Fact]
    public void CloseAccount_ZeroBalance_RemovesAccountFromClient()
    {
        //Act
        _accountService.WithdrawRub(_testAccount.Id, _testAccount.Balance);
        _accountService.CloseAccount(_testAccount.Id);

        //Assert 
        Assert.Null(_testClient.Accounts.FirstOrDefault(x => x.Id == _testAccount.Id));
    }

    //попытка закрытия аккаунта с балансом, не равным 0
    [Fact]
    public void CloseAccount_NonZeroBalance_ThrowsAccountBalanceNotZeroException()
    {
        //Act & Assert
        Assert.Throws<AccountBalanceNotZeroException>(
            () => _accountService.CloseAccount(_testAccount.Id)
        );
    }


    //ненайденный аккаунт
    [Fact]
    public void Deposit_AccountNotFound_ThrowsAccountNotFoundException()
    {
        //Arrange
        Guid id = Guid.NewGuid();
        decimal amount = 0m;

        //Act & Assert
        Assert.Throws<AccountNotFoundException>(
            () => _accountService.DepositRub(id, amount)
        );
    }

    //ненайденный клиент, для которого открываем аккаунт
    [Fact]
    public void OpenAccount_NonExistingClient_ThrowsClientNotFoundException()
    {
        //Arrange
        Guid id = Guid.NewGuid();

        //Act & Assert
        Assert.Throws<ClientNotFoundException>(
            () => _accountService.OpenAccount(id, Currency.RUB, AccountType.Current)
        );
    }

    

}
