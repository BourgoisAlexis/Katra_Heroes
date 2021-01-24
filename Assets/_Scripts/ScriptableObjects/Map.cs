using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SquareData
{
    public e_squareType Type;
    public e_teams Team;
}

public class Map : ScriptableObject
{
    #region Variables
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private List<SquareData> squaresList;

    //Accessors
    public List<SquareData> SquaresList => squaresList;
    public Vector2Int MapSize => mapSize;
    #endregion


    public void Save(List<Square> _squareList, Vector2Int _mapSize)
    {
        squaresList = new List<SquareData>();
        mapSize = _mapSize;

        foreach (Square s in _squareList)
        {
            SquareData data = new SquareData();
            data.Type = s.SquareType;
            data.Team = s.Team;

            squaresList.Add(data);
        }
    }
}
