using BankingServiceCLI.Models;

namespace BankingServiceCLI.Services.Interfaces;

//интерфейс сервиса управления клиентами
public interface IClientService
{
    //получение всех клиентов
    List<Client> GetAll();  

    //получение транзакций клиента за период времени
    //from_period == null && to_period == null  => возвращаем транзакции клиента за все время
    //исключение - ClientNotFoundException
    List<Transaction> GetTransactionsById(Guid id, DateTime? fromPeriod, DateTime? toPeriod);

    //получение клиента по id   
    //исключение - ClientNotFoundException
    Client GetById(Guid id);    

    //добавление клиента
    //исключение - ArgumentException
    Client Add(Client client); 

    //удаление клиента по id
    //исключение - ClientNotFoundException
    Client Remove(Guid id); 

    //обновление клиента по id
    //исключения:
    //1) ClientNotFoundException
    //2) ArgumentException
    Client Update(Client newClient, Guid id);

    //получение общего баланса по всем счетам клиента
    //исключение - ClientNotFoundException
    decimal GetCommonRubBalanceById(Guid id);
}   
