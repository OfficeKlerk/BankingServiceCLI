namespace BankingServiceCLI.Exceptions;

public class AccountBalanceNotZeroException : Exception
{
    public decimal Balance {get; set;}

    public AccountBalanceNotZeroException(decimal balance) : base($"Account balance must be equal to zero: {balance}")
    {
        Balance = balance;
    }
}