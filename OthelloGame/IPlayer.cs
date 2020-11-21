namespace OthelloGame
{
    public interface IPlayer
    {
        StoneColor colorType { get; }
        PlayerType playerType { get; }
        string PutStone();
    }
}