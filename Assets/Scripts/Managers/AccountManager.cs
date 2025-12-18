public class AccountManager
{
    public LoginState loginState;
    public PlayerStats playerStats;
    public Profile playerProfile;

    public void Setup()
    {
        loginState = LoginState.unsignned;
    }

    public bool CheckUsername()
    {
        return false;
    }

    public void EditUsername()
    {

    }

    public void DeleteAccount()
    {

    }
}
