#nullable enable
using System;
using System.Collections.Generic;

namespace ConnectBoy.Core.DTOs
{
    // Auth
    public record RegisterRequest(string Email, string Password, string Username, string? DisplayName);
    public record LoginRequest(string Email, string Password);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Token, string NewPassword);
    public record GoogleLoginRequest(string IdToken);
    public record AppleLoginRequest(string IdentityToken);
    public record RefreshTokenRequest(string RefreshToken);
    public record AuthResponse(string AccessToken, string RefreshToken, UserProfileDto User);

    // Account
    public record UpdateUsernameRequest(string Username);
    public record RegisterDeviceTokenRequest(string FcmToken, string Platform);
    public record InitiateTransactionRequest(decimal Amount, string Type, string Description);

    // Notifications
    public record SendInviteRequest(string RecipientUsername);

    // Admin
    public record SendEmailRequest(Guid UserId, string Subject, string Body);

    // Shared
    public record UserProfileDto(
        Guid Id,
        string Email,
        string Username,
        string? DisplayName,
        bool IsBlocked,
        DateTime CreatedAt);

    public record NotificationDto(
        Guid Id,
        Guid? SenderUserId,
        string? SenderUsername,
        string Type,
        string Title,
        string Body,
        string? Data,
        bool IsRead,
        DateTime CreatedAt);

    // Admin
    public record AdminUserListDto(
        Guid Id,
        string Email,
        string Username,
        string? DisplayName,
        bool IsBlocked,
        DateTime CreatedAt);

    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
}

// Provide compatibility type for 'init' accessors / records on targets that don't define IsExternalInit.
// This common shim fixes CS0518: Predefined type 'System.Runtime.CompilerServices.IsExternalInit' is not defined.
namespace System.Runtime.CompilerServices
{
    // Keep internal to avoid polluting public API. The compiler only needs the type to exist.
    internal static class IsExternalInit { }
}