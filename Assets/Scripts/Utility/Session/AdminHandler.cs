using ConnectBoy.Core;
using ConnectBoy.Core.DTOs;
using ConnectBoy.Core.Entities;
using System.Threading.Tasks;
using System;

public class AdminHandler
{
#if UNITY_EDITOR
    private static string _baseUrl = "http://localhost:5087/api/";
#else
    private static string _baseUrl = "http://connectboy1.runasp.net/api/"; //TODO: Change this to the actual production URL when deploying
#endif

    /// <summary> Sends a login request to the server and returns the response containing the authentication token and refresh token. </summary>
    public static async Task<LoginResponse> Login(LoginRequest loginData)
    {
        return await new WebPoster().PostRequestAsync<LoginRequest, LoginResponse>(_baseUrl + "login", loginData);
    }

    /// <summary>Get's the User data using the auth token</summary>
    public static async Task<UserData> GetUserData(string token)
    {
        return await new WebPoster(true).GetRequestAsync<UserData>(_baseUrl + "account/user");
    }

    #region Account Management
    public static async Task<UserProfileDto> GetThisUser()
    {
        return await new WebPoster(true).GetRequestAsync<UserProfileDto>(_baseUrl + "account/me");
    }

    public static async Task<UserProfileDto> ChangeUsername(UpdateUsernameRequest req)
    {
        return await new WebPoster(true).PostRequestAsync<UpdateUsernameRequest, UserProfileDto>(_baseUrl + "account/username", req);
    }

    public static async Task RegisterDeviceToken(RegisterDeviceTokenRequest req)
    {
        await new WebPoster(true).PostRequestAsync<RegisterDeviceTokenRequest, object>(_baseUrl + "account/device-token", req);
    }

    public static async Task DeleteAccount()
    {
        await new WebPoster(true).DeleteRequestAsync(_baseUrl + "account/delete");
    }

    public static async Task<Wallet> GetWallet()
    {
        return await new WebPoster(true).GetRequestAsync<Wallet>(_baseUrl + "account/wallet");
    }

    public static async Task<object> InitiateTransaction(InitiateTransactionRequest req)
    {
        /* Sample response:
            new
            {
                transactionToken = transaction.ReferenceToken,
                message = "Transaction initiated. Pass this token to the external service."
            }
        */
        return await new WebPoster(true).PostRequestAsync<InitiateTransactionRequest, object>(_baseUrl + "account/transaction", req);
    }
    #endregion

    #region Notifications
    public static async Task<NotificationDto> SendInvite(SendInviteRequest req)
    {
        return await new WebPoster(true).PostRequestAsync<SendInviteRequest, NotificationDto>(_baseUrl + "notifications/invite", req);
    }
    
    public static async Task<NotificationDto[]> GetNotifications(int page = 1, int pageSize = 20)
    {
        // Build URL with query parameters: GET /api/notifications?page=1&pageSize=20

        var url = $"{_baseUrl}notifications?page={page}&pageSize={pageSize}";
        return await new WebPoster(true).GetRequestAsync<NotificationDto[]>(url);
    }

    public static async Task MarkNotificationAsRead(Guid notificationId)
    {
        await new WebPoster(true).PutRequestAsync<string, object>(_baseUrl + $"notifications/{notificationId}/read", notificationId);
    }

    #endregion

    #region Authentication
    public static async Task<AuthResponse> Register(RegisterRequest req)
    {
        return await new WebPoster().PostRequestAsync<RegisterRequest, AuthResponse>(_baseUrl + "auth/register", req);
    }

    public static async Task<object> ForgotPassword(ForgotPasswordRequest req)
    {
        return await new WebPoster().PostRequestAsync<ForgotPasswordRequest, object>(_baseUrl + "auth/forgot-password", req);
    }

    public static async Task<object> ResetPassword(ResetPasswordRequest req)
    {
        return await new WebPoster().PostRequestAsync<ResetPasswordRequest, object>(_baseUrl + "auth/reset-password", req);
    }

    public static async Task<AuthResponse> RefreshToken(RefreshTokenRequest req)
    {
        return await new WebPoster().PostRequestAsync<RefreshTokenRequest, AuthResponse>(_baseUrl + "auth/refresh", req);
    }

    public static async Task<object> Logout(RefreshTokenRequest req)
    {
        return await new WebPoster(true).PostRequestAsync<RefreshTokenRequest, object>(_baseUrl + "auth/logout", req);
    }

    public static async Task<AuthResponse> AltLogin(LoginRequest req)
    {
        return await new WebPoster().PostRequestAsync<LoginRequest, AuthResponse>(_baseUrl + "auth/login", req);
    }

    public static async Task<AuthResponse> GoogleLogin(GoogleLoginRequest req)
    {
        return await new WebPoster().PostRequestAsync<GoogleLoginRequest, AuthResponse>(_baseUrl + "auth/google", req);
    }

    public static async Task<AuthResponse> AppleLogin(AppleLoginRequest req)
    {
        return await new WebPoster().PostRequestAsync<AppleLoginRequest, AuthResponse>(_baseUrl + "auth/apple", req);
    }
    #endregion

}
