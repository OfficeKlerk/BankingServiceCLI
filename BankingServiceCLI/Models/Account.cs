namespace BankingServiceCLI.Models;

//перечисление для типа аккаунта
public enum AccountType
{
    Current,   //расчетный
    Cumulative //накопительный
}

//перечисление для валюты
public enum Currency
{
    RUB,
    USD,
    EUR
}


//сущность "Аккаунт"
public class Account
{

    public Guid Id {get; set;} = Guid.NewGuid();
    public decimal Balance {get; set;}
    public Currency Currency {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public AccountType Type {get; set;}
    
    public List<Transaction> TransactionHistory {get; set;} = new List<Transaction>();

    //дефолтный конструктор для JSON-сериализации
    public Account() {}
    public Account(decimal balance, Currency currency, AccountType type)
    {
        Balance = balance;
        Currency = currency;
        Type = type;
    }

}