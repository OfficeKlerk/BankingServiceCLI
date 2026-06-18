using BankingServiceCLI.Services.Interfaces;
using BankingServiceCLI.Models;
using BankingServiceCLI.Exceptions;
using BankingServiceCLI.Extensions;
namespace BankingServiceCLI.Services;

//основная имплементация сервиса клиентов
public class ClientService : IClientService
{
    private readonly BankData _data; //хранилище с клиентами

    public ClientService(BankData data)
    {
        _data = data;
    }

    public List<Client> GetAll()
    {
        return _data.Clients;
    }

    public List<Transaction> GetTransactionsById(Guid id, DateTime? fromPeriod, DateTime? toPeriod)
    {
        Client client = GetById(id);

        //если клиент нашелся
        return client.Accounts
                    .SelectMany(x => x.TransactionHistory)
                    .Where(x => (fromPeriod == null || x.CreatedAt >= fromPeriod) && 
                                (toPeriod == null || x.CreatedAt <= toPeriod))
                    .ToList();

    }

    public Client GetById(Guid id)
    {
        Client? client = _data.Clients.FirstOrDefault(x => x.Id == id);
        if(client == null)
        {
            throw new ClientNotFoundException(id);
        }

        return client;
    }

    public Client Add(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Surname))
        {
            throw new ArgumentException("Name and surname are required");
        }

        _data.Clients.Add(client);
        return client;
    }

    public Client Remove(Guid id)
    {
        Client? client = _data.Clients.FirstOrDefault(x => x.Id == id);
        if(client == null)
        {
            throw new ClientNotFoundException(id);
        }

        _data.Clients.Remove(client);
        return client;
    }

    public Client Update(Client newClient, Guid id)
    {
        Client? client = _data.Clients.FirstOrDefault(x => x.Id == id);
        if(client == null)
        {
            throw new ClientNotFoundException(id);
        }

        if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Surname))
        {
            throw new ArgumentException("Name and surname are required");
        }

        client.Name = newClient.Name;
        client.Surname = newClient.Surname;
        
        return client;
    }

    public decimal GetCommonRubBalanceById(Guid id)
    {
        Client? client = _data.Clients.FirstOrDefault(x => x.Id == id);
        if(client == null)
        {
            throw new ClientNotFoundException(id);
        }

        //возвращаем сумму в рублях
        return client.Accounts.Sum(x => x.Balance.ToRub(x.Currency));
    }
}