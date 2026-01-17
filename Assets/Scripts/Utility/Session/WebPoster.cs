using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class WebPoster
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<TResponse> PostRequestAsync<TRequest, TResponse>(string url, object data)
    {
        //Serialize the data to Json
        string jsonRequest = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //Execute the POST request
        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode == false)
        {
            Debug.LogError($"Request failed with status code: {response.StatusCode}");
            return default;
        }
        //response.EnsureSuccessStatusCode(); // Throws if not 200-299

        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(responseBody);
    }
}


