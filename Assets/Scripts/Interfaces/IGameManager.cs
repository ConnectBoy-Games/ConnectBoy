public interface IGameManager
{
    public void ClearBoard();
    public void CheckBoardState();
    public int CheckWinState(string piece);
    public void GetGameState();
}
