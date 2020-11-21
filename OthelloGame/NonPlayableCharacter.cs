using System;

namespace OthelloGame
{
    public class NonPlayableCharacter : IPlayer
    {
        public StoneColor colorType { get; }
        public PlayerType playerType => PlayerType.NonPlayable;

        private readonly Board _board;

        public NonPlayableCharacter(StoneColor colorType, Board board)
        {
            this.colorType = colorType;
            _board = board;
        }

        public string PutStone()
        {
            var raw = (char) ('a' + new Random().Next(0, _board.Width));
            var column = new Random().Next(1, _board.Width+1);

            var command = $"{raw}{column}";
            return command;
        }
    }
}