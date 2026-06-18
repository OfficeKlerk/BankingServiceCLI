namespace BankingServiceCLI.Models;

//сущность "Клиент"
public class Client
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public string Name {get; set;} = string.Empty;
    public string Surname {get; set;} = string.Empty;
    public List<Account> Accounts {get; set;} = new List<Account>();

    //дефолтный конструктор для JSON-сереализации
    public Client() {}

    public Client(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

}