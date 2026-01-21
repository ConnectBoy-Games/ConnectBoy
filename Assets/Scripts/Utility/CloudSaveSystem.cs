using Unity.Services.CloudSave;
using Unity.Services.CloudCode;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

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
            Dictionary<string, object> args = new Dictionary<string, object>
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
            Debug.LogError("User not found!");
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
        }
    }

}
