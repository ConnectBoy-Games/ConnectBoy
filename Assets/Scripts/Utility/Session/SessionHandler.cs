using System.Collections.Generic;
using System.Threading.Tasks;

public class SessionHandler
{
    //TODO: Expand services to handle multiple servers with different base URLs

#if UNITY_EDITOR 
    private static string _baseUrl = "http://localhost:5001/api/sessions";
#else
    private static string _baseUrl = "http://connectboy1.runasp.net/api/Sessions";
#endif

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
    public static async Task<JoinSessionResponse> JoinSession(string sessionId, JoinSessionRequest joinRequest)
    {
        //The URL matches our .NET route: {sessionId}/join
        return await new WebPoster().PostRequestAsync<JoinSessionRequest, JoinSessionResponse>($"{_baseUrl}/{sessionId}/join", joinRequest);
    }

    /// <summary>Get the session details</summary>
    /// <param name="sessionId">The id of the game session</param>
    /// <returns>The details of the game session</returns>
    public static async Task<SessionDetails> CheckSessionStatus(string sessionId)
    {
        return await new WebPoster().GetRequestAsync<SessionDetails>($"{_baseUrl}/{sessionId}/");
    }

    /// <summary>Sends a message to the session and the get all the other unchecked messages</summary>
    /// <param name="sessionId">The session id</param>
    /// <param name="chat">The Chat Message Send object</param>
    /// <returns>A lis of messages since the last one sent</returns>
    public static async Task<List<ChatMessage>> SendSessionChat(string sessionId, ChatMessage chat)
    {
        return await new WebPoster().PostRequestAsync<ChatMessage, List<ChatMessage>>($"{_baseUrl}/{sessionId}/chat", chat);
    }

    /// <summary>Returns a list of messages from the last message specified by id</summary>
    /// <param name="sessionId">The session id</param>
    /// <param name="id">The id of the last message loaded by the client</param>
    /// <returns>A list of Chat message return</returns>
    public static async Task<List<ChatMessage>> GetSessionChat(string sessionId, int id)
    {
        return await new WebPoster().GetRequestAsync<List<ChatMessage>>($"{_baseUrl}/{sessionId}/chat/{id}");
    }

    /// <summary>Sends a move based on the game type</summary>
    /// <param name="sessionId">The session id</param>
    /// <param name="move">The move data of the specific game type</param>
    /// <returns>A game result object that shows the result of the action</returns>
    public static async Task<GameResult> MakeMove(string sessionId, object move)
    {
        return await new WebPoster().PostRequestAsync<MakeMoveRequest, GameResult>($"{_baseUrl}/{sessionId}/game/move", move);
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
