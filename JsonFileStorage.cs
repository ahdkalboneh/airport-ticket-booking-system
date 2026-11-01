using System.Text.Json;

public class JsonFileStorage<T>
{
    string Filepath {get;}

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


}