using System.Collections.Generic;
using UnityEngine;

public class Map : ScriptableObject
{
    #region Variables
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private List<int> squaresList;

    //Accessors
    public List<int> SquaresList => squaresList;
    public Vector2Int MapSize => mapSize;
    #endregion


    public void Save(List<Square> _squareList, Vector2Int _mapSize)
    {
        squaresList = new List<int>();

        foreach (Square s in _squareList)
        {
            int data = (int)s.SquareType;
            squaresList.Add(data);
            mapSize = _mapSize;
        }
    }
}
