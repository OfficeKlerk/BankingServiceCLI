using BankingServiceCLI.Services.Interfaces;
using BankingServiceCLI.Models;
using BankingServiceCLI.Exceptions;
using BankingServiceCLI.Extensions;
using System.Runtime.CompilerServices;
namespace BankingServiceCLI.Services;

//основная имплементация сервиса аккаунтов
public class AccountService : IAccountService
{
    private readonly BankData _data;

    public AccountService(BankData data)
    {
        _data = data;
    }

    public Client OpenAccount(Guid clientId, Currency currency = Currency.RUB, AccountType type = AccountType.Current)
    {
        Client? client = _data.Clients.FirstOrDefault(x => x.Id == clientId);
        if(client == null)
        {
            throw new ClientNotFoundException(clientId);
        }

        Account account = new Account(0, currency, type);
        client.Accounts.Add(account);

        return client;
    }

    public Client CloseAccount(Guid accountId)
    {
        Account? account = _data.Clients.SelectMany(x => x.Accounts).FirstOrDefault(x => x.Id == accountId);
        if(account == null)
        {
            throw new AccountNotFoundException(accountId);
        }

        if(account.Balance != 0)
        {
            throw new AccountBalanceNotZeroException(account.Balance);
        }

        //ищем клиента, у которого есть такой счет, он точно есть
        Client client = _data.Clients.FirstOrDefault(x => x.Accounts.Any(y => y.Id == account.Id))!;

        client.Accounts.Remove(account);
        
        return client;
    }

    public Transaction DepositRub(Guid accountId, decimal amount)
    {
        Account? account = _data.Clients.SelectMany(x => x.Accounts).FirstOrDefault(x => x.Id == accountId);
        if(account == null)
        {
            throw new AccountNotFoundException(accountId);
        }
        if(amount <= 0)
        {
            throw new ArgumentException("The amount of deposit must be more than zero");
        }
        
        decimal convertedAmount = amount.FromRub(account.Currency);
        account.Balance += convertedAmount;

        Transaction result = new Transaction(TransactionType.Deposit, convertedAmount, 
                                             fromAccountId: null, toAccountId: accountId);
        account.TransactionHistory.Add(result);

        return result;
    }

    public Transaction WithdrawRub(Guid accountId, decimal amount)
    {
        if(amount <= 0)
        {
            throw new ArgumentException("The amount of withdrawal must be more than zero");
        }

        Account? account = _data.Clients.SelectMany(x => x.Accounts).FirstOrDefault(x => x.Id == accountId);
        if(account == null)
        {
            throw new AccountNotFoundException(accountId);
        }
        
        decimal convertedAmount = amount.FromRub(account.Currency);
        if(account.Balance < convertedAmount)
        {
            throw new InsufficientFundsException(account.Balance, convertedAmount);
        }

        account.Balance -= convertedAmount;

        Transaction result = new Transaction(TransactionType.Withdrawal, convertedAmount, 
                                            fromAccountId: accountId, toAccountId: null);
        account.TransactionHistory.Add(result);

        return result;
    }

    public Transaction TransferRub(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        if(fromAccountId == toAccountId)
        {
            throw new SameAccountTransferException(fromAccountId);
        }
        if(amount <= 0)
        {
            throw new ArgumentException("The amount of transfer must be more than zero");
        }

        Account? fromAccount = _data.Clients.SelectMany(x => x.Accounts).FirstOrDefault(x => x.Id == fromAccountId);
        Account? toAccount = _data.Clients.SelectMany(x => x.Accounts).FirstOrDefault(x => x.Id == toAccountId);
        if(fromAccount == null)
        {
            throw new AccountNotFoundException(fromAccountId);
        }
        if(toAccount == null)
        {
            throw new AccountNotFoundException(toAccountId);
        }

        decimal fromConvertedAmount = amount.FromRub(fromAccount.Currency);
        decimal toConvertedAmount = amount.FromRub(toAccount.Currency);
        if(fromAccount.Balance < fromConvertedAmount)
        {
            throw new InsufficientFundsException(fromAccount.Balance, fromConvertedAmount);
        }

        fromAccount.Balance -= fromConvertedAmount;
        toAccount.Balance += toConvertedAmount;

        Transaction result = new Transaction(TransactionType.Transfer, amount, fromAccountId, toAccountId);
        fromAccount.TransactionHistory.Add(result);
        toAccount.TransactionHistory.Add(result);

        return result;
    }
}