using BankingServiceCLI.Models;

namespace BankingServiceCLI.Exceptions;

public class SameAccountTransferException : Exception
{
    public Guid AccountId { get; }

    public SameAccountTransferException(Guid accountId) : 
           base($"Sender-account and receiver-account are the same: {accountId}")
    {
        AccountId = accountId;
    }
}