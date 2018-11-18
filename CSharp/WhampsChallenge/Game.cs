using System;
using System.Collections.Generic;
using System.Linq;
using ContestantContracts.Game;
using ContestantContracts.Perceptions;

namespace WhampsChallenge
{
    enum FieldContent
    {
        Empty,
        Trap,
        Whamps,
        Gold
    }

    public class Game : IGame
    {
        private Dictionary<Randomizers, Random> _randomizers;
        private int _seed;
        private bool _isStarted;
        private Map<FieldContent> _map;
        private (int, int) _playerPosition;

        public bool IsGameOver { get; private set; }

        public int Seed
        {
            get => _seed;
            set
            {
                if (_isStarted) throw new InvalidOperationException("Seed cannot be changed when game is already started.");
                _seed = value;
            }
        }

        public int MovesLeft { get; private set; }

        public bool HasArrow { get; private set; }

        public Game()
        {
            Seed = new Random().Next(int.MinValue, int.MaxValue);
        }

        public void Initialize()
        {
            if (_isStarted) throw new InvalidOperationException("Game can only be started once.");
            _isStarted = true;

            var mainRandom = new Random(Seed);
            _randomizers = Enum.GetValues(typeof(Randomizers))
                .OfType<Randomizers>()
                .ToDictionary(x => x, _ => mainRandom.GetNewChild());

            // Create 5x5 array of empty fields
            _map = new Map<FieldContent>(5, 5)
            {
                [GetFreeSquare()] = {Value = FieldContent.Trap},
                [GetFreeSquare()] = {Value = FieldContent.Trap},
                [GetFreeSquare()] = {Value = FieldContent.Whamps},
                [GetFreeSquare()] = {Value = FieldContent.Gold}
            };

            _playerPosition = GetFreeSquare();
            MovesLeft = 100;
            HasArrow = true;
            IsGameOver = false;
        }

        #region  Actions
        public Perception[] Move(Direction direction)
        {
            ValueTuple<int, int> newPosition;
            List<Perception> result = new List<Perception>();

            switch (direction)
            {
                case Direction.North:
                    newPosition = (_playerPosition.Item1, _playerPosition.Item2 - 1);
                    break;
                case Direction.South:
                    newPosition = (_playerPosition.Item1, _playerPosition.Item2 + 1);
                    break;
                case Direction.West:
                    newPosition = (_playerPosition.Item1 - 1, _playerPosition.Item2);
                    break;
                case Direction.East:
                    newPosition = (_playerPosition.Item1 + 1, _playerPosition.Item2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newField = _map[newPosition];
            if (newField == null)
            {
                result.Add(Perception.Bump);
            }
            else
            {
                switch (newField.Value)
                {
                    case FieldContent.Whamps:
                        IsGameOver = true;
                        result.Add(Perception.Death);
                        break;
                    case FieldContent.Trap:
                        IsGameOver = true;
                        result.Add(Perception.Death);
                        break;
                    case FieldContent.Gold:
                        result.Add(Perception.Glitter);
                        break;
                }

                _playerPosition = newPosition;
            }

            result.AddRange(ProcessAnyAction());
            return result.ToArray();
        }

        public Perception[] Shoot(Direction direction)
        {
            var result = new List<Perception>();
            if (!HasArrow)
            {
                result.Add(Perception.Twang);
            }
            else
            {
                // Move into the direction until the end of the playing field.
                var field = _map[_playerPosition].AdjacentFields[direction];
                while (field != null)
                {
                    if (field.Value == FieldContent.Whamps)
                    {
                        result.Add(Perception.Scream);
                    }

                    field = field.AdjacentFields[direction];
                }
            }

            result.AddRange(ProcessAnyAction());
            return result.ToArray();

        }

        public Perception[] Pickup()
        {
            List<Perception> result = new List<Perception>();

            if (_map[_playerPosition].Value == FieldContent.Whamps)
            {
                IsGameOver = true;
                result.Add(Perception.Win);
            }

            result.AddRange(ProcessAnyAction());
            return result.ToArray();
        }
        #endregion

        private Perception[] ProcessAnyAction()
        {
            var result = new List<Perception>();

            result.AddRange(GetAdjacentPerceptions());

            MovesLeft--;
            if (MovesLeft < 1)
            {
                result.Add(Perception.Death);
                IsGameOver = true;
            }

            return result.ToArray();
        }

        private Perception[] GetAdjacentPerceptions()
        {
            var result = new List<Perception>();

            var fields = _map[_playerPosition].AdjacentFields.Values.Select(f => f.Value).ToArray();

            if (fields.Contains(FieldContent.Trap)) result.Add(Perception.Wind);
            if (fields.Contains(FieldContent.Whamps)) result.Add(Perception.Stench);

            return result.ToArray();
        }

        private ValueTuple<int,int> GetFreeSquare()
        {
            while (true)
            {
                var x = _randomizers[Randomizers.Level].Next(0, 4);
                var y = _randomizers[Randomizers.Level].Next(0, 4);

                // Check if chosen field is empty
                if (_map[x, y].Value != FieldContent.Empty) continue;

                // Check if this field has neither smell nor wind on it
                var adjacentSquares = _map[_playerPosition].AdjacentFields.Values.Select(f => f.Value).ToArray();
                if (adjacentSquares.Contains(FieldContent.Trap) || adjacentSquares.Contains(FieldContent.Whamps)) continue;

                return (x,y);
            }
        }
    }
}
