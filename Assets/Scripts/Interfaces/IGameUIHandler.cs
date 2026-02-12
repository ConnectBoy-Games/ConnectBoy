public interface IGameUIHandler
{
    public void GoBack();

    public void GoToHome();

    public void SetTurnText(User turnUser, string text = null);

    public void DisplayWinScreen(string name, int wager = -1);

    public void DisplayDefeatScreen(string name, int wager = -1);
}