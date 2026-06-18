using BankingServiceCLI.Models;
using BankingServiceCLI.Storage;
using BankingServiceCLI.Services;
using System.Text.Json;
namespace BankingServiceCLI;

class Program
{
    static void Main(string[] args)
    {
        var storage = new JsonStorage();
        var data = storage.Load();

        //сервисы
        var clientService = new ClientService(data);
        var accountService = new AccountService(data);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n=== Banking Service ===");
            Console.WriteLine("1. Add client");
            Console.WriteLine("2. List clients");
            Console.WriteLine("3. Open account");
            Console.WriteLine("4. Deposit");
            Console.WriteLine("5. Withdraw");
            Console.WriteLine("6. Transfer");
            Console.WriteLine("7. Client's transaction history");
            Console.WriteLine("8. Total client balance");
            Console.WriteLine("0. Exit");
            Console.Write("Choose action: ");

            string input = Console.ReadLine()!;

            try
            {
                switch (input)
                {
                    case "1":
                        //считать имя и фамилию
                        Console.Write("\nEnter name: ");
                        string name = Console.ReadLine()!;

                        Console.Write("Enter surname: ");
                        string surname = Console.ReadLine()!;

                        Client newClient = clientService.Add(new Client(name, surname));
                        Console.WriteLine($"\nNew client: {JsonSerializer.Serialize(newClient, storage.Options)}\n");
                        break;

                    case "2":
                    {
                        Console.WriteLine("\nClients:\n");
                        List<Client> clients = clientService.GetAll();
                        if(clients.Count == 0)
                        {
                            Console.WriteLine("\n{No clients found}\n");
                        }
                        foreach(var client in clients)
                        {
                            Console.WriteLine($"{JsonSerializer.Serialize(client, storage.Options)}\n");
                        }
                        break;
                    }

                    case "3":
                    {
                        //считать clientId, currency, type
                        Console.Write("\nEnter client id: ");
                        Guid clientId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter currency: 1 - RUB, 2 - USD, 3 - EUR: ");
                        int answ = int.Parse(Console.ReadLine()!);
                        if(answ < 1 || answ > 3)
                        {
                            throw new ArgumentException("Incorrect value of currency");
                        }

                        Currency curr = (Currency)(answ - 1);

                        Console.Write("Enter account type: 1 - Current, 2 - Cumulative: ");
                        answ = int.Parse(Console.ReadLine()!);
                        if(answ < 1 || answ > 2)
                        {
                            throw new ArgumentException("Incorrect value of account type");
                        }

                        AccountType type = (AccountType)(answ - 1);


                        Client client = accountService.OpenAccount(clientId, curr, type);
                        Console.WriteLine($"\nClient: {JsonSerializer.Serialize(client, storage.Options)}\n");
                        break;
                    }

                    case "4":
                    {
                        //считать accountId и amount
                        Console.Write("\nEnter account id: ");
                        Guid accountId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter amount (decimal): ");
                        decimal amount = decimal.Parse(Console.ReadLine()!);

                        Transaction result = accountService.DepositRub(accountId, amount);
                        Console.WriteLine($"\nTransaction: {JsonSerializer.Serialize(result, storage.Options)}\n");
                        break;
                    }
                    case "5":
                    {
                        //считать accountId и amount
                        Console.Write("\nEnter account id: ");
                        Guid accountId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter amount (decimal): ");
                        decimal amount = decimal.Parse(Console.ReadLine()!);

                        Transaction result = accountService.WithdrawRub(accountId, amount);
                        Console.WriteLine($"\nTransaction: {JsonSerializer.Serialize(result, storage.Options)}\n");
                        break;

                    }
                    case "6":
                    {
                        //считать fromAccountId, toAccountId, amount
                        Console.Write("\nEnter sender id: ");
                        Guid fromAccountId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter receiver id: ");
                        Guid toAccountId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter amount (decimal): ");
                        decimal amount = decimal.Parse(Console.ReadLine()!);

                        Transaction result = accountService.TransferRub(fromAccountId, toAccountId, amount);
                        Console.WriteLine($"\nTransaction: {JsonSerializer.Serialize(result, storage.Options)}\n");
                        break;
                    }
                    case "7":
                    {
                        //считать clientId, fromPeriod, toPeriod
                        Console.Write("\nEnter client id: ");
                        Guid clientId = Guid.Parse(Console.ReadLine()!);

                        Console.Write("Enter FROM period (can be empty): ");
                        DateTime? fromPeriod = null;
                        string answ = Console.ReadLine()!;
                        if (!string.IsNullOrWhiteSpace(answ))
                        {
                            fromPeriod = DateTime.Parse(answ);
                        }

                        Console.Write("Enter TO period (can be empty): ");
                        DateTime? toPeriod = null;
                        answ = Console.ReadLine()!;
                        if (!string.IsNullOrWhiteSpace(answ))
                        {
                            toPeriod = DateTime.Parse(answ);
                        }

                        Console.WriteLine("\nTransactions: \n");
                        List<Transaction> transactions = clientService.GetTransactionsById(clientId, fromPeriod, toPeriod);
                        if (transactions.Count == 0)
                        {
                            Console.WriteLine("{No transactions found}\n");
                            break;
                        }
                        foreach(var transaction in transactions)
                        {
                            Console.WriteLine($"{JsonSerializer.Serialize(transaction, storage.Options)}\n");
                        }

                        break;
                    }

                    case "8":
                    {
                        //считать clientId
                        Console.Write("\nEnter client id: ");
                        Guid clientId = Guid.Parse(Console.ReadLine()!);

                        decimal amount = clientService.GetCommonRubBalanceById(clientId);
                        Console.WriteLine($"Total amount: {amount}\n");
                        break;
                    }
                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Incorrect input");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nTry again\n\n");
            }
        }

        storage.Save(data); //сохранить при выходе
        
    }
}

