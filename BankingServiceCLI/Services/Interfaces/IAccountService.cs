using BankingServiceCLI.Models;

namespace BankingServiceCLI.Services.Interfaces;

//интерфейс сервиса управления счетами
public interface IAccountService
{
    //открыть счет для клиента
    //исключение - ClientNotFoundException
    Client OpenAccount(Guid clientId, Currency currency, AccountType type);

    //закрыть счет клиента
    //исключения:
    //1) AccountNotFoundException
    //2) AccountBalanceNotZeroException
    Client CloseAccount(Guid accountId);

    //пополнить счет
    //исключения:
    //1) AccountNotFoundException
    //2) ArgumentException
    Transaction DepositRub(Guid accountId, decimal amount);

    //снять деньги со счета
    //исключения:
    //1) AccountNotFoundException
    //2) ArgumentException
    //3) InsufficientFundsException
    Transaction WithdrawRub(Guid accountId, decimal amount);

    //перевести сумму между счетами
    //исключения:
    //1) AccountNotFoundException
    //2) ArgumentException
    //3) InsufficientFundsException
    //4) SameAccountTransferException
    Transaction TransferRub(Guid fromAccountId, Guid toAccountId, decimal amount);

}