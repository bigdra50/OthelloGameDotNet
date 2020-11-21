using System;

namespace OthelloGame
{
    public class PlayableCharacter : IPlayer
    {
        public StoneColor colorType { get; }
        public PlayerType playerType => PlayerType.Playable;

        public PlayableCharacter(StoneColor colorType)
        {
            this.colorType = colorType;
        }

        public string PutStone()
        {
            Console.WriteLine($"{colorType} turn:");
            var command = Console.ReadLine();
            return command;
        }
    }
}