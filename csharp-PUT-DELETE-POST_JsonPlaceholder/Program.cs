

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Program
{
    private static readonly JsonSerializerOptions JsonOut = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    public static async Task Main()
    {
        using var http = new HttpClient { BaseAddress = new Uri("https://jsonplaceholder.typicode.com/") };

        Console.WriteLine("=== JSONPlaceholder /albums CRUD con una sola classe Album ===\n");

        // READ: GET /Photos/1
        Console.WriteLine("GET /Photos/2");
        Photos? one = await http.GetFromJsonAsync<Photos>("Photos/2");
        Console.WriteLine(JsonSerializer.Serialize(one, JsonOut));
        Console.WriteLine();

        // READ: GET /Photos
        Console.WriteLine("GET /Photos");
        List<Photos>? all = await http.GetFromJsonAsync<List<Photos>>("Photos");
        Console.WriteLine($"Totale album: {all?.Count}");
        if (all is { Count: > 0 })
            Console.WriteLine(JsonSerializer.Serialize(all[0], JsonOut));
        Console.WriteLine();

        // CREATE: POST /Photos
        Console.WriteLine("POST /Photos");
        var toCreate = new Photos { AlbumId = 1, Title = "Nuova foto (demo)" };
        var resPost = await http.PostAsJsonAsync("albums", toCreate, JsonOut);
        resPost.EnsureSuccessStatusCode();
        Photos? created = await resPost.Content.ReadFromJsonAsync<Photos>();
        Console.WriteLine(JsonSerializer.Serialize(created, JsonOut));
        Console.WriteLine("Nota: JSONPlaceholder non persiste (id tipico: 101).\n");

        // UPDATE: PUT /Photos/1
        Console.WriteLine("PUT /albums/1");
        var toUpdate = new Photos { AlbumId = 1, Id = 1, Title = "Foto aggiornata via PUT" };
        var resPut = await http.PutAsJsonAsync("albums/1", toUpdate, JsonOut);
        resPut.EnsureSuccessStatusCode();
        Photos? updated = await resPut.Content.ReadFromJsonAsync<Photos>();
        Console.WriteLine(JsonSerializer.Serialize(updated, JsonOut));
        Console.WriteLine();

        // PATCH: /Photos/1 (uso la stessa classe Album con solo Title impostato)
        Console.WriteLine("PATCH /Photos/1");
        var toPatch = new Photos { Title = "Titolo patch" };
        var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"), "Photos/1")
        {
            Content = JsonContent.Create(toPatch, options: JsonOut)
        };
        var resPatch = await http.SendAsync(patchReq);
        resPatch.EnsureSuccessStatusCode();
        Photos? patched = await resPatch.Content.ReadFromJsonAsync<Photos>();
        Console.WriteLine(JsonSerializer.Serialize(patched, JsonOut));
        Console.WriteLine();

        // DELETE: /Photos/1
        Console.WriteLine("DELETE /Photos/1");
        var resDel = await http.DeleteAsync("Photos/1");
        Console.WriteLine($"Esito: {(int)resDel.StatusCode} {resDel.ReasonPhrase} (200 o 204)");
        Console.WriteLine("\n=== Fine Demo ===");
    }
}

/*public class Album
{
    [JsonPropertyName("userId")] public int UserId { get; set; }
    [JsonPropertyName("id")]     public int Id { get; set; }
    [JsonPropertyName("title")]  public string? Title { get; set; }
}*/


public class Photos
{
    [JsonPropertyName("albumId")] public int AlbumId { get; set; }
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("thumbnailUrl")] public string? ThumbnailUrl { get; set; }
}