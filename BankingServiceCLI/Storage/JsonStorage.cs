namespace BankingServiceCLI.Storage;
using System.Text.Json.Serialization;
using System.Text.Json;
using BankingServiceCLI.Models;

//хранилище данных в текстовом файле в формате json
public class JsonStorage
{
    //путь к файлу 
    private readonly string _path;

    //настройки сериализации
    public JsonSerializerOptions Options {get; init;} = new()
    {
        WriteIndented = true,                          //красивые отступы
        Converters = { new JsonStringEnumConverter() } //для красивого отображения enum в json
    };

    public JsonStorage(string path = "storage.json")
    {
        _path = Path.Combine(AppContext.BaseDirectory, path);
        Console.WriteLine($"Storage path: {_path}"); // временно
    }

    //получить данные из файла
    public BankData Load()
    {
        //файла нет — возвращаем пустые данные
        if (!File.Exists(_path))
        {
            return new BankData(); 
        }
            
        string json = File.ReadAllText(_path);
        BankData? result = JsonSerializer.Deserialize<BankData>(json, Options);
        if(result == null)
        {
            throw new InvalidOperationException("Deserialization error: data file is invalid");
        }

        return result;
    }

    public void Save(BankData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(_path, json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Serialization error: {ex.Message}");
        }
    }
}