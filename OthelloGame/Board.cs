using System;
using System.Collections.Generic;

namespace OthelloGame
{
    /// <summary>
    ///    a b c d e f g h
    /// 1                   1
    /// 2                   2
    /// 3                   3
    /// 4        w b        4
    /// 5        b w        5
    /// 6                   6
    /// 7                   7
    /// 8                   8
    ///    a b c d e f g h
    /// </summary>
    public class Board
    {
        public Stone[,] Stone => _stones;
        public int RemainStone => width * width - (_whiteCount + _blackCount);
        public bool IsFull => RemainStone == 0;
        public event EventHandler<Stone> OnPutStone;
        public int Width => width;
        private const int width = 8;
        private Stone[,] _stones;
        private int _whiteCount = 0;
        private int _blackCount = 0;
        private List<string> _logMessages = new List<string>();

        public enum AdjacentDirection
        {
            UpperLeft, // 左上
            Up, // 上
            UpperRight, // 右上
            Left, // 左
            Right, // 右
            LowerLeft, // 左下
            Lower, // 下
            LowerRight, // 右下
        }

        private IReadOnlyDictionary<char, int> _columnIndexMap = new Dictionary<char, int>
        {
            {'a', 0},
            {'b', 1},
            {'c', 2},
            {'d', 3},
            {'e', 4},
            {'f', 5},
            {'g', 6},
            {'h', 7},
        };

        private IReadOnlyDictionary<int, int> _rawIndexMap = new Dictionary<int, int>
        {
            {1, 0},
            {2, 1},
            {3, 2},
            {4, 3},
            {5, 4},
            {6, 5},
            {7, 6},
            {8, 7},
        };


        public Board()
        {
            _stones = new Stone[width, width];
            InitStones();
            Render();
        }

        public void Update()
        {
            _logMessages.Clear();
            Render();
        }

        public bool TryPutStone(string cellId, StoneColor color)
        {
            if (!TryGetTargetIndexKeys(cellId, out (char columnIndex, int rawIndex) result)) return false;
            var targetIndices = (raw: _rawIndexMap[result.rawIndex], column: _columnIndexMap[result.columnIndex]);
            if (!_stones[targetIndices.raw, targetIndices.column].IsEmpty)
            {
                _logMessages.Add("そこにはもう石が置いてある!");
                return false;
            }

            var stone = new Stone((targetIndices.raw, targetIndices.column), color);
            //OnPutStone?.Invoke(this, stone);
            if (!TryReverseAllAdjacent(stone))
            {
                _logMessages.Add("そこに置いても何もひっくり返せない!");
                return false;
            }

            _stones[targetIndices.raw, targetIndices.column] = stone;
            if (color == StoneColor.Black) _blackCount++;
            else if (color == StoneColor.White) _whiteCount++;
            return true;
        }

        private bool TryReverseLine(Stone stone, AdjacentDirection direction)
        {
            if (!TryGetAdjacentStone(stone, direction, out var adjacentStone)) return false;
            if (adjacentStone.IsEmpty) return false;

            void UpdateReversedStoneCount()
            {
                switch (stone.CurrentColor)
                {
                    case StoneColor.Black:
                        _blackCount++;
                        _whiteCount--;
                        break;
                    case StoneColor.White:
                        _blackCount--;
                        _whiteCount++;
                        break;
                }
            }

            if (stone.CurrentColor != adjacentStone.CurrentColor)
            {
                if (!stone.TryReverse()) return false;

                UpdateReversedStoneCount();
                return true;
            }

            if (!TryReverseLine(adjacentStone, direction) || !stone.TryReverse()) return false;
            UpdateReversedStoneCount();
            return true;
        }

        private bool TryReverseAllAdjacent(Stone stone)
        {
            var isSucceed = false;
            foreach (AdjacentDirection direction in Enum.GetValues(typeof(AdjacentDirection)))
            {
                if (!TryGetAdjacentStone(stone, direction, out var adjacentStone)) continue; // 隣に盤がなければスキップ
                if (adjacentStone.IsEmpty) continue; // 隣に石が置かれていないならスキップ
                if (stone.CurrentColor == adjacentStone.CurrentColor) continue; // 隣と同じ色ならスキップ
                if (TryReverseLine(adjacentStone, direction)) isSucceed = true;
            }

            return isSucceed;
        }

        private bool TryGetAdjacentStone(Stone currentStone, AdjacentDirection adjacentDirection,
            out Stone adjacentStone)
        {
            adjacentStone = null;
            switch (currentStone.Position.raw)
            {
                // 一番上と一番下のケース
                case 0 when adjacentDirection == AdjacentDirection.UpperLeft ||
                            adjacentDirection == AdjacentDirection.Up ||
                            adjacentDirection == AdjacentDirection.UpperRight:
                case width - 1 when adjacentDirection == AdjacentDirection.LowerLeft ||
                                    adjacentDirection == AdjacentDirection.Lower ||
                                    adjacentDirection == AdjacentDirection.LowerRight:
                    return false;
            }

            switch (currentStone.Position.column)
            {
                // 一番左と一番右のケース
                case 0 when adjacentDirection == AdjacentDirection.Left ||
                            adjacentDirection == AdjacentDirection.LowerLeft ||
                            adjacentDirection == AdjacentDirection.UpperLeft:
                case width - 1 when adjacentDirection == AdjacentDirection.Right ||
                                    adjacentDirection == AdjacentDirection.LowerRight ||
                                    adjacentDirection == AdjacentDirection.UpperRight:
                    return false;
            }

            adjacentStone = adjacentDirection switch
            {
                AdjacentDirection.UpperLeft => _stones[currentStone.Position.raw - 1, currentStone.Position.column - 1],
                AdjacentDirection.Up => _stones[currentStone.Position.raw - 1, currentStone.Position.column],
                AdjacentDirection.UpperRight => _stones[currentStone.Position.raw - 1,
                    currentStone.Position.column + 1],
                AdjacentDirection.Left => _stones[currentStone.Position.raw, currentStone.Position.column - 1],
                AdjacentDirection.Right => _stones[currentStone.Position.raw, currentStone.Position.column + 1],
                AdjacentDirection.LowerLeft => _stones[currentStone.Position.raw + 1, currentStone.Position.column - 1],
                AdjacentDirection.Lower => _stones[currentStone.Position.raw + 1, currentStone.Position.column],
                AdjacentDirection.LowerRight => _stones[currentStone.Position.raw + 1,
                    currentStone.Position.column + 1],
            };
            return true;
        }

        private void InitStones()
        {
            for (var raw = 0; raw < width; raw++)
            {
                for (var column = 0; column < width; column++)
                {
                    _stones[raw, column] = (raw, column) switch
                    {
                        (width / 2 - 1, width / 2 - 1) => new Stone((raw, column), StoneColor.White), // 左上
                        (width / 2, width / 2) => new Stone((raw, column), StoneColor.White), // 右下
                        (width / 2 - 1, width / 2) => new Stone((raw, column), StoneColor.Black), // 右上
                        (width / 2, width / 2 - 1) => new Stone((raw, column), StoneColor.Black), // 左下
                        _ => new Stone((raw, column)) // その他
                    };
                }
            }

            _whiteCount = 2;
            _blackCount = 2;
        }

        private bool TryGetTargetIndexKeys(string cellId, out (char column, int raw) keys)
        {
            keys = (cellId[0], Convert.ToInt32(cellId[1].ToString()));
            if (cellId.Length != 2)
            {
                _logMessages.Add("a8とかd3みたいに指定して");
                return false;
            }

            if (!_columnIndexMap.ContainsKey(keys.column))
            {
                _logMessages.Add("_columnKey Error");
                return false;
            }

            if (!_rawIndexMap.ContainsKey(keys.raw))
            {
                _logMessages.Add($"_rawKey Error: {keys.raw} not found");
                return false;
            }

            return true;
        }

        private void Render()
        {
            RenderBoard();
            Console.WriteLine();
            RenderStonesCountInfo();
            Console.WriteLine();
        }

        private void RenderBoard()
        {
            Console.WriteLine("  a b c d e f g h");
            foreach (var raw in _rawIndexMap.Keys)
            {
                Console.Write($"{raw} ");
                foreach (var column in _columnIndexMap.Values)
                {
                    Console.Write($"{_stones[_rawIndexMap[raw], column].CurrentSymbol} ");
                }

                Console.WriteLine($"{raw}");
            }

            Console.WriteLine("  a b c d e f g h");
        }

        private void RenderStonesCountInfo()
        {
            Console.WriteLine($"White: {_whiteCount}, Black: {_blackCount}");
            Console.WriteLine($"Stones Remaining: {RemainStone}");
        }

        public void RenderWinnerInfo()
        {
            var whiteInfo = $"White: {_whiteCount}";
            var blackInfo = $"Black: {_blackCount}";
            if (_whiteCount > _blackCount)
            {
                Console.WriteLine($"☆{whiteInfo}, {blackInfo}");
                Console.WriteLine("Winner: White!!!!");
            }else if (_whiteCount < _blackCount)
            {
                Console.WriteLine($"☆{blackInfo}, {whiteInfo}");
                Console.WriteLine("Winner: Black!!!!");
            }
            else
            {
                Console.WriteLine($"{whiteInfo}, {blackInfo}");
                Console.WriteLine("Draw!");
            }
        }

        public void RenderLogMessage()
        {
            foreach (var logMessage in _logMessages)
            {
                Console.WriteLine(logMessage);
            }

            _logMessages.Clear();
        }
    }
}