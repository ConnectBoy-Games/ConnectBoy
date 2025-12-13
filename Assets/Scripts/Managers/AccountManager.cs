public class AccountManager
{
    public Wagr.LoginState loginState;
    public Wagr.PlayerStats playerStats;
    public Wagr.Profile playerProfile;

    public void Setup()
    {
        loginState = Wagr.LoginState.unsignned;
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
