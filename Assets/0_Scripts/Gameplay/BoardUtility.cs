using System.Collections.Generic;
using UnityEngine;

public class BoardUtility
{
    private Square[,] board;
    private Vector2Int mapSize;


    public BoardUtility(Square[,] _board, Vector2Int _mapSize)
    {
        board = _board;
        mapSize = _mapSize;
    }


    public List<Square> GetRange(List<Square> _starting, int _range, List<e_squareType> _exceptions, bool _getOccupieds)
    {
        List<Square> toReturn = new List<Square>(_starting);

        for (int i = 0; i < _range; i++)
        {
            List<Square> tempo = new List<Square>(toReturn);

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
                    if (!_exceptions.Contains(s.SquareType))
                        if (_getOccupieds || !s.Occupied)
                            toReturn.Add(s);
        }
        return toReturn;
    }

    public List<Square> GetRangeSpe(List<Square> _starting, Ability _ability)
    {
        List<Square> toReturn = new List<Square>(_starting);
        List<Square> tempo = new List<Square>(toReturn);

        switch (_ability.RangeType)
        {
            case e_rangeType.Default:
                List<e_squareType> excep = new List<e_squareType>();
                toReturn = GetRange(toReturn, _ability.Range, excep, true);
                break;

            case e_rangeType.Straight:
                foreach (Square s in toReturn)
                {
                    int x = s.Position.x;
                    int y = s.Position.y;

                    for (int i = 0; i < _ability.Range + 1; i++)
                    {
                        if (x - i >= 0)
                            tempo.Add(board[x - i, y]);
                        if (x + i <= mapSize.x - 1)
                            tempo.Add(board[x + i, y]);
                        if (y - i >= 0)
                            tempo.Add(board[x, y - i]);
                        if (y + i <= mapSize.y - 1)
                            tempo.Add(board[x, y + i]);
                    }
                }
                foreach (Square s in tempo)
                    if (!toReturn.Contains(s))
                        toReturn.Add(s);
                break;

            case e_rangeType.Diagonal:
                foreach (Square s in toReturn)
                {
                    int x = s.Position.x;
                    int y = s.Position.y;

                    for (int i = 0; i < _ability.Range + 1; i++)
                    {
                        if (x - i >= 0)
                        {
                            if (y - i >= 0)
                                tempo.Add(board[x - i, y - i]);
                            if (y + i <= mapSize.y - 1)
                                tempo.Add(board[x - i, y + i]);
                        }
                        if (x + i <= mapSize.x - 1)
                        {
                            if (y - i >= 0)
                                tempo.Add(board[x + i, y - i]);
                            if (y + i <= mapSize.y - 1)
                                tempo.Add(board[x + i, y + i]);
                        }
                    }
                }
                foreach (Square s in tempo)
                    if (!toReturn.Contains(s))
                        toReturn.Add(s);
                break;

            case e_rangeType.AllyTeam:
                tempo.Clear();
                foreach (Square s in board)
                    if (s.Occupied && s.Piece.Team == GameplayManager.Instance.Team)
                        tempo.Add(s);
                toReturn = tempo;
                break;

            case e_rangeType.EnnemyTeam:
                tempo.Clear();
                foreach (Square s in board)
                    if (s.Occupied && s.Piece.Team != GameplayManager.Instance.Team)
                        tempo.Add(s);
                toReturn = tempo;
                break;
        }
        return toReturn;
    }
}
