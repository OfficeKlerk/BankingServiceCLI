namespace BankingServiceCLI.Models;

//перечисление для типа транзакции
public enum TransactionType
{
    Deposit,       //пополнение
    Withdrawal,    //снятие
    Transfer        //перевод
}



//сущность "Транзакция"
public record Transaction
{
    public Guid Id {get; init;} = Guid.NewGuid();
    public TransactionType Type {get; init;}
    public decimal Amount {get; init;}
    public DateTime CreatedAt {get; init;} = DateTime.Now;

    public Guid? FromAccountId {get; init;}  //с какого аккаунта провели транзакцию (снятие/перевод)
    public Guid? ToAccountId {get; init;}    //на какой аккаунт провели транзакцию (пополнение/перевод)

    //дефолтный конструктор для JSON-сереализации
    public Transaction() {}

    public Transaction(TransactionType type, decimal amount, Guid? fromAccountId = null, Guid? toAccountId = null)
    {
        Type = type;
        Amount = amount;
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
    }

}