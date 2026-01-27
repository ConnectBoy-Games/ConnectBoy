using System.Threading.Tasks;

public class SessionHandler
{
    private static string _baseUrl = "http://localhost:5001/api/sessions"; //TODO: Expand services to handle multiple servers with different base URLs

    /// <summary>Creates a game session on the server</summary>
    /// <param name="sessionRequest">Details the parameters for the game session</param>
    /// <returns>A CreateSessionResponse that give the details of the session on the server</returns>
    public static async Task<CreateSessionResponse> CreateSession(CreateSessionRequest sessionRequest)
    {
        return await new WebPoster().PostRequestAsync<CreateSessionRequest, CreateSessionResponse>(_baseUrl, sessionRequest);
    }

    /// <summary>Join an already created game session</summary>
    /// <param name="sessionId">The id of the game session</param>
    /// <returns>True if the connection was succesful</returns>
    public static async Task<bool> JoinSession(string sessionId, JoinSessionRequest joinRequest)
    {
        //The URL matches our .NET route: {sessionId}/join
        return await new WebPoster().PostRequestAsync<string, bool>($"{_baseUrl}/{sessionId}/join", joinRequest);
    }

    public static async Task DestroySession(string sessionId)
    {
        throw new System.NotImplementedException();
    }
}


/*
    private async Task SendAuthenticatedRequest(UnityWebRequest request)
    {
        // Grab the token from Unity Authentication Service
        string accessToken = AuthenticationService.Instance.AccessToken;
        // HELPER: Adds the Unity JWT to the header (All requests should be authenticated)
        request.SetRequestHeader("Authorization", $"Bearer {accessToken}");

        var operation = request.SendWebRequest();
        await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error: {request.downloadHandler.text}");
        NotificationDisplay.instance.DisplayMessage("Error communicating with server. " + request.downloadHandler.text, NotificationType.error);
    }
    */
