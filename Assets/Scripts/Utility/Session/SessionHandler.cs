using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Unity.Services.Authentication;
using Wagr;

public class SessionHandler
{
    //TODO: Expand services to handle multiple servers with different base URLs
    private string _baseUrl = "http://localhost:5000/api/sessions";

    public async Task<CreateSessionResponse> CreateSession()
    {
        CreateSessionRequest createRequest = new CreateSessionRequest
        {
            Name = "New Session",
            HostName = "HostPlayer"
        };

        WebPoster poster = new WebPoster();

        await poster.PostRequestAsync<CreateSessionRequest, CreateSessionResponse>(_baseUrl, createRequest);
        return null;

        using var request = UnityWebRequest.Post(_baseUrl, JsonConvert.SerializeObject(createRequest));
        //request.SetRequestHeader("Content-Type", "application/json");
        request.SendWebRequest();
        Debug.Log("Sending Create Session Request...");

        while (!request.isDone)
            await Task.Yield();

        Debug.Log(request.responseCode);
        Debug.Log(request.result);

        if (request.result == UnityWebRequest.Result.Success)
        {
            var session = JsonUtility.FromJson<CreateSessionResponse>(request.downloadHandler.text);
            Debug.Log($"Session Created: {session.SessionId}");
            return session;
        }
        return null;
        //await SendAuthenticatedRequest(request);
    }

    public async Task<bool> JoinSession(string sessionId)
    {
        //The URL matches our .NET route: {sessionId}/join
        using var request = UnityWebRequest.Post($"{_baseUrl}/{sessionId}/join", "");
        await SendAuthenticatedRequest(request);

        return request.result == UnityWebRequest.Result.Success;
    }

    public async Task DestroySession(string sessionId)
    {
        using var request = UnityWebRequest.Delete($"{_baseUrl}/{sessionId}");
        await SendAuthenticatedRequest(request);

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Session destroyed successfully.");
    }

    // HELPER: Adds the Unity JWT to the header (All requests should be authenticated)
    private async Task SendAuthenticatedRequest(UnityWebRequest request)
    {
        // Grab the token from Unity Authentication Service
        string accessToken = AuthenticationService.Instance.AccessToken;
        request.SetRequestHeader("Authorization", $"Bearer {accessToken}");

        var operation = request.SendWebRequest();
        await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error: {request.downloadHandler.text}");
            NotificationDisplay.instance.DisplayMessage("Error communicating with server. " + request.downloadHandler.text, NotificationType.error);
    }
}
