using System;

[Serializable]
public class LoginResponse
{
    public string Token;
    public string RefreshToken;
}

[Serializable]
public class LoginRequest
{
    public string Email;
    public string Password;
}

[Serializable]
public class UserData
{ }
