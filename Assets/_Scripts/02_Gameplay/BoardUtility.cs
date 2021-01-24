using System.Collections.Generic;
using UnityEngine;

public class BoardUtility
{
    #region Variables
    private Square[,] board;
    private Vector2Int mapSize;

    public Square[,] Board => board;
    public Vector2Int MapSize => mapSize;
    #endregion


    public BoardUtility(Square[,] _board, Vector2Int _mapSize)
    {
        board = _board;
        mapSize = _mapSize;
    }


    public List<Square> GetRange(List<Square> _starting, Ability _ability)
    {
        List<Square> toReturn = new List<Square>(_starting);
        List<Square> tempo = new List<Square>(toReturn);

        if (_ability.RangeType == e_rangeType.Default)
        {
            for (int i = 0; i < _ability.Range; i++)
            {
                tempo = new List<Square>(toReturn);

                foreach (Square s in toReturn)
                {
                    int x = s.Position.x;
                    int y = s.Position.y;

                    if (x > 0)
                        tempo.Add(board[x - 1, y]);
                    if (x < mapSize.x - 1)
                        tempo.Add(board[x + 1, y]);
                    if (y > 0)
                        tempo.Add(board[x, y - 1]);
                    if (y < mapSize.y - 1)
                        tempo.Add(board[x, y + 1]);
                }
                foreach (Square s in tempo)
                    if (!toReturn.Contains(s))
                        if (!_ability.Exceptions.Contains(s.SquareType))
                            if (_ability.GetOccupied || !s.Occupied)
                                toReturn.Add(s);
            }
        }

        else if (_ability.RangeType == e_rangeType.Straight)
        {
            tempo = new List<Square>(toReturn);
            Vector2Int[] directions = new Vector2Int[]
            { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };

            for (int i = 0; i < directions.Length; i++)
                tempo.AddRange(GetRangeInDirection(_starting[0], directions[i], _ability.Range, _ability.Exceptions, _ability.GetOccupied));

            toReturn = tempo;
        }

        else if (_ability.RangeType == e_rangeType.Diagonal)
        {
            tempo = new List<Square>(toReturn);
            Vector2Int[] directions = new Vector2Int[]
            { new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1) };

            for (int i = 0; i < directions.Length; i++)
                tempo.AddRange(GetRangeInDirection(_starting[0], directions[i], _ability.Range, _ability.Exceptions, _ability.GetOccupied));

            toReturn = tempo;
        }

        else if (_ability.RangeType == e_rangeType.Allies)
        {
            tempo.Clear();
            foreach (Square s in board)
                if (s.Occupied && s.Piece.Team == GameplayManager.Instance.Team)
                    tempo.Add(s);

            toReturn = tempo;
        }

        else if (_ability.RangeType == e_rangeType.Ennemies)
        {
            tempo.Clear();
            foreach (Square s in board)
                if (s.Occupied && s.Piece.Team != GameplayManager.Instance.Team)
                    tempo.Add(s);

            toReturn = tempo;
        }

        return toReturn;
    }

    public List<Square> GetAOE(List<Square> _starting, EffectStat _effect)
    {
        List<Square> toReturn = new List<Square>(_starting);
        List<Square> tempo = new List<Square>(toReturn);

        if (_effect.EffectRangeType == e_rangeType.Default)
        {
            for (int i = 0; i < _effect.EffectRange; i++)
            {
                tempo = new List<Square>(toReturn);

                foreach (Square s in toReturn)
                {
                    int x = s.Position.x;
                    int y = s.Position.y;

                    if (x > 0)
                        tempo.Add(board[x - 1, y]);
                    if (x < mapSize.x - 1)
                        tempo.Add(board[x + 1, y]);
                    if (y > 0)
                        tempo.Add(board[x, y - 1]);
                    if (y < mapSize.y - 1)
                        tempo.Add(board[x, y + 1]);
                }
                foreach (Square s in tempo)
                    if (!toReturn.Contains(s))
                        toReturn.Add(s);
            }
        }

        else if (_effect.EffectRangeType == e_rangeType.Straight)
        {
            tempo = new List<Square>(toReturn);
            Vector2Int[] directions = new Vector2Int[]
            { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };

            for (int i = 0; i < directions.Length; i++)
                tempo.AddRange(GetRangeInDirection(_starting[0], directions[i], _effect.EffectRange, null, true));

            toReturn = tempo;
        }

        else if (_effect.EffectRangeType == e_rangeType.Diagonal)
        {
            tempo = new List<Square>(toReturn);
            Vector2Int[] directions = new Vector2Int[]
            { new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1) };

            for (int i = 0; i < directions.Length; i++)
                tempo.AddRange(GetRangeInDirection(_starting[0], directions[i], _effect.EffectRange, null, true));

            toReturn = tempo;
        }

        return toReturn;
    }

    public List<Square> GetRangeInDirection(Square _start, Vector2Int _dir, int _range, List<e_squareType> _excep, bool _getOccu)
    {
        List<Square> toReturn = new List<Square>();

        for (int i = 1; i < _range; i++)
        {
            int x = _start.Position.x + i * _dir.x;
            int y = _start.Position.y + i * _dir.y;

            if (x < 0 || x >= mapSize.x)
                break;
            if (y < 0 || y >= mapSize.y)
                break;

            Square s = board[x, y];
            if (_excep != null && _excep.Contains(s.SquareType))
                break;
            if (!_getOccu && s.Occupied)
                break;

            toReturn.Add(s);
        }

        return toReturn;
    }

    public Square GetFinalSquareInDirection(Square _starting, Vector2Int _direction, int _distance)
    {
        Square square = _starting;
        Vector2Int start = _starting.Position;

        for (int i = 0; i < _distance; i++)
        {
            Vector2Int tempo = start + _direction * i;
            if (tempo.y >= 0 && tempo.y < mapSize.y)
                if (tempo.x >= 0 && tempo.x < mapSize.x)
                    square = board[tempo.x, tempo.y];
        }

        return square;
    }
}
