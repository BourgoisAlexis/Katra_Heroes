using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Map map;
    [SerializeField]
    private GameObject squarePrefab;
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private float sideSpace;
    [SerializeField]
    private List<Square> squaresList;

    private Square[,] board;
    private Camera _mainCamera;
    private Vector2 startingPoint;
    #endregion

    public void  GenerateMap()
    {
        foreach (Square s in squaresList)
            DestroyImmediate(s.gameObject);

        squaresList.Clear();

        if (map != null)
            mapSize = map.MapSize;
        int current = 0;

        float spriteSize = squarePrefab.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x / 2;

        Vector2 mapSpace = new Vector2(sideSpace - spriteSize, sideSpace - spriteSize);

        float step = mapSpace.x * 2 / (mapSize.x - 1);
        startingPoint = new Vector2(-mapSpace.x, step / 2 * (mapSize.y - 1));

        
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                GameObject inst = Instantiate(squarePrefab, transform);
                inst.transform.localPosition = new Vector2(startingPoint.x + step * i, startingPoint.y - step * j);

                Square square = inst.GetComponent<Square>();

                e_squareType type = map != null ? (e_squareType)map.SquaresList[current] : (e_squareType)Random.Range(1, (int)e_squareType.Obstacle + 1);

                square.Setup(new Vector2Int(i, j), (int)type);
                current++;

                squaresList.Add(square);
            }
        }
    }

    public void UpdateColor()
    {
        foreach (Square s in squaresList)
            s.Colorisation();
    }

#if UNITY_EDITOR
    public void SaveMap()
    {
        Map map = ScriptableObject.CreateInstance<Map>();
        map.Save(squaresList, mapSize);
        AssetDatabase.CreateAsset(map, "Assets/Prefabs/Maps/NewMap_" + mapSize.x + "x" + mapSize.y + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif


    public void Setup()
    {
        if (map != null)
            GenerateMap();

        board = new Square[mapSize.x, mapSize.y];
        int z = 0;

        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                Square s = board[i, j] = squaresList[z];
                s.Setup(new Vector2Int(i, j), (int)squaresList[z].SquareType);
                z++;
            }

        GameplayManager.Instance.Setup(board, mapSize);
    }
}
