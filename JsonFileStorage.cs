using System.Text.Json;

public class JsonFileStorage<T>
{
    string Filepath { get; }

    public JsonFileStorage(String filepath)
    {
        this.Filepath = filepath;
    }

    public async Task<List<T>> ReadFile()
    {
        string data = await File.ReadAllTextAsync(this.Filepath);
        List<T>? datas = string.IsNullOrWhiteSpace(data) ? new List<T>()
                                : JsonSerializer.Deserialize<List<T>>(data) ?? new List<T>();
        return datas;
    }

    public async Task<Dictionary<string, T>> ReadDictionary()
    {
        string data = await File.ReadAllTextAsync(this.Filepath);
        Dictionary<string, T> datas = string.IsNullOrWhiteSpace(data) ? new Dictionary<string, T>()
                                             : JsonSerializer.Deserialize<Dictionary<string, T>>(data) ?? new Dictionary<string, T>();
        return datas;
    }

    public async Task WriteDictionary(Dictionary<string, T> data)
    {
        string json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions { WriteIndented = true }
        );

        await File.WriteAllTextAsync(Filepath, json);
    }
    public async Task WriteFile(List<T> data)
    {
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(Filepath, json);
    }

}