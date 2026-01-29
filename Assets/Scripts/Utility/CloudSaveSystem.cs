using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.Networking;

public class CloudSaveSystem
{
    public static async Task<T> RetrieveSpecificData<T>(string key)
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });

            if (results.TryGetValue(key, out var item))
            {
                string json = item.Value.GetAsString();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                Debug.Log($"There is no such key as {key}!");
                return default;
            }
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }

        return default;
    }

    public static async Task DeleteSpecificData(string key, string writeLock)
    {
        try
        {
            // Deletion of the key with write lock validation
            await CloudSaveService.Instance.Data.Player.DeleteAsync(key, new DeleteOptions { WriteLock = writeLock });

            Debug.Log($"Successfully deleted {key}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }

    public static async Task SaveSpecificData<T>(string key, T data)
    {
        try
        {
            var saveData = new Dictionary<string, object>
            {
                { key, data }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);

            Debug.Log($"Successfully saved data for key: {key}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }

    public static async Task<bool> SetUsername(string username)
    {
        try
        {
            Debug.Log($"Attempting to claim name: {username}...");

            //var args = new SetNameRequest { requestedName = username };
            Dictionary<string, object> args = new()
            {
                { "requestedName", username }
            };
            
            // Call the Cloud Code script
            var response = await CloudCodeService.Instance.CallEndpointAsync<NameResponse>("SetDisplayName", args);

            if (response.success)
            {
                Debug.Log($"Success! Your name is now: {response.name}");
                return true;
            }
        }
        catch (CloudCodeException ex)
        {
            // This catches the "throw new Error" from the JS script
            if (ex.Message.Contains("already taken"))
            {
                Debug.LogError("Name is unavailable. Please try another.");
            }
            else
            {
                Debug.LogError($"Cloud Code Error: {ex.Message}");
            }
        }
        return false;
    }

    public static async Task<bool> IsNameTaken(string nameToCheck)
    {
        var result = await CallLookupScript(nameToCheck);
        return result.exists;
    }

    public static async Task<string> GetIdByName(string nameToFind)
    {
        var result = await CallLookupScript(nameToFind);
        
        if (result.exists)
        {
            Debug.Log($"Found ID for {nameToFind}: {result.playerId}");
            return result.playerId;
        }
        else
        {
            Debug.LogWarning($"User {nameToFind} not found.");
            return null;
        }
    }

    private static async Task<LookupResponse> CallLookupScript(string name)
    {
        try
        {
            Dictionary<string, object> args = new Dictionary<string, object>
            {
                { "targetName", name }
            };
            
            var response = await CloudCodeService.Instance.CallEndpointAsync<LookupResponse>("LookUpName", args);
            return response;
        }
        catch (CloudCodeException ex)
        {
            Debug.LogError($"Cloud Code Error: {ex.Message}");
            return new LookupResponse { exists = false }; // Fail safe
        }
    }

    public static async Task SendMatchInvite(string targetUsername, int type, int wager, string mId)
    {
        // 1. Use the lookup script from the previous step to get the Receiver's ID
        string targetId = await GetIdByName(targetUsername);
        
        if (string.IsNullOrEmpty(targetId))
        {
            NotificationDisplay.instance.DisplayMessage("User not found!", NotificationType.error);
            return;
        }

        // 2. Prepare the arguments for Cloud Code
        var args = new Dictionary<string, object> { 
            { "targetPlayerId", targetId },
            { "matchId", mId },
            { "matchType", type },
            { "wager", wager }
        };

        try 
        {
            await CloudCodeService.Instance.CallEndpointAsync("SendInvite", args);
            Debug.Log($"Wager of {wager} sent to {targetUsername}!");
        }
        catch (CloudCodeException ex)
        {
            Debug.LogError($"Failed to send invite: {ex.Message}");
            NotificationDisplay.instance.DisplayMessage("Failed to send invit: " + ex.Message, NotificationType.error);
        }
    }
}

public static class NetworkRetryHelper
{
    private const int MaxRetries = 3;
    private const int InitialDelayMs = 1000;

    public static async Task<UnityWebRequest> ExecuteWithRetry(Func<UnityWebRequest> requestFactory)
    {
        int retryCount = 0;

        while (true)
        {
            UnityWebRequest request = requestFactory();
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            // Success! Return the request so the caller can handle the data
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request;
            }

            // Check if the error is "Retryable" (Network errors or 5xx Server errors)
            bool isRetryable = request.result == UnityWebRequest.Result.ConnectionError ||
                               request.result == UnityWebRequest.Result.ProtocolError && request.responseCode >= 500;

            if (isRetryable && retryCount < MaxRetries)
            {
                retryCount++;
                // Exponential delay: 1s, 2s, 4s...
                int delay = InitialDelayMs * (int)Math.Pow(2, retryCount - 1);

                Debug.LogWarning($"Request failed ({request.error}). Retrying in {delay}ms... (Attempt {retryCount}/{MaxRetries})");

                await Task.Delay(delay);
                request.Dispose(); // Clear the failed request before trying again
            }
            else
            {
                // Not retryable (like 401 Unauthorized or 404) or out of retries
                return request;
            }
        }
    }
}