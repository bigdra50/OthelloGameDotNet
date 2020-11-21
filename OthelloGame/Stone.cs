using System;
using System.Collections.Generic;

namespace OthelloGame
{
    public class Stone
    {
        private (int raw, int column) _position;
        private StoneColor _currentColor;

        public (int raw, int column) Position => _position;
        public StoneColor CurrentColor => _currentColor;
        public bool IsEmpty => _currentColor == StoneColor.None;
        public char CurrentSymbol => stoneSymbolMap[_currentColor];

        private IReadOnlyDictionary<StoneColor, char> stoneSymbolMap = new Dictionary<StoneColor, char>
        {
            {StoneColor.White, 'o'},
            {StoneColor.Black, 'x'},
            {StoneColor.None, '-'},
        };
        public Stone((int, int) position, StoneColor color = StoneColor.None)
        {
            _position = position;
            _currentColor = color;
        }

        public bool TryReverse()
        {
            if (_currentColor == StoneColor.None) return false;
            _currentColor = _currentColor == StoneColor.White ? StoneColor.Black : StoneColor.White;
            return true;
        }

    }
}