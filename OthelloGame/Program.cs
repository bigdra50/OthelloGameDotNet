using System;

namespace OthelloGame
{
    class Program
    {
        private static Board _board;

        static void Main(string[] args)
        {
            _board = new Board();
            var firstPlayer = GeneratePlayer(StoneColor.Black);
            var secondPlayer = GeneratePlayer(StoneColor.White);
            GameLoop(firstPlayer, secondPlayer);
        }

        static IPlayer GeneratePlayer(StoneColor color)
        {
            while (true)
            {
                Console.WriteLine($"Select Player Type {color}: Human[h], Computer[c]");
                var input = Console.ReadLine()?.ToLower();
                switch (input)
                {
                    case "h":
                    case "human":
                        return new PlayableCharacter(color);
                    case "c":
                    case "com":
                    case "cpu":
                    case "computer":
                    case "npc":
                        return new NonPlayableCharacter(color, _board);
                }
            }
        }

        static void GameLoop(IPlayer firstPlayer, IPlayer secondPlayer)
        {
            if (firstPlayer.colorType != StoneColor.Black || secondPlayer.colorType != StoneColor.White)
            {
                throw new ArgumentException("Player Stone Color is Invalid");
            }

            var turn = 0;
            turn++;
            while (true)
            {
                if (_board.IsFull)
                {
                    _board.RenderWinnerInfo();
                    break;
                }

                bool hasTurnEnd;
                if (turn % 2 != 0)
                {
                    var command = firstPlayer.PutStone();
                    if (command == "s")
                    {
                        Console.WriteLine("Skipped!");
                        turn++;
                        continue;
                    }

                    hasTurnEnd = _board.TryPutStone(command, StoneColor.Black);
                    if (firstPlayer.playerType == PlayerType.Playable && !hasTurnEnd)
                        _board.RenderLogMessage();
                }
                else
                {
                    var command = secondPlayer.PutStone();
                    if (command == "s")
                    {
                        Console.WriteLine("Skipped!");
                        turn++;
                        continue;
                    }

                    hasTurnEnd = _board.TryPutStone(command, StoneColor.White);
                    if (secondPlayer.playerType == PlayerType.Playable && !hasTurnEnd)
                        _board.RenderLogMessage();
                }

                if (hasTurnEnd)
                {
                    turn++;
                    _board.Update();
                    Console.WriteLine();
                }
            }
        }
    }
}