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
    private Vector3 aspectRatio;
    [SerializeField]
    private List<Square> squaresList;

    private Square[,] board;
    private Camera _mainCamera;
    private Vector2 startingPoint;
    private Vector2 screenSize;
    #endregion

    public void  GenerateMap()
    {
        foreach (Square s in squaresList)
            DestroyImmediate(s.gameObject);

        squaresList.Clear();

        float spriteSize = squarePrefab.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x / 2;

        if (map != null)
            mapSize = map.MapSize;
        int current = 0;

        float step = 0;
        float margin = 0.5f;

        screenSize = new Vector2(aspectRatio.z, aspectRatio.z * aspectRatio.x / aspectRatio.y);
        screenSize = new Vector2(screenSize.x - margin - spriteSize, screenSize.y - margin - spriteSize);

        if (screenSize.y <= screenSize.x)
        {
            step = screenSize.y * 2 / (mapSize.y - 1);
            startingPoint = new Vector2(-screenSize.x + Mathf.Abs(screenSize.x - step / 2 * (mapSize.x - 1)), screenSize.y);
        }
        else
        {
            step = screenSize.x * 2 / (mapSize.x - 1);
            startingPoint = new Vector2(-screenSize.x, screenSize.y - Mathf.Abs(screenSize.y - step / 2 * (mapSize.y - 1)));
        }

        
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


    private void Start()
    {
        if (map != null)
            GenerateMap();

        Setup();
    }

    //Fill the board + Give it to Manager
    private void Setup()
    {
        board = new Square[mapSize.x, mapSize.y];
        int z = 0;

        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                Square s = board[i, j] = squaresList[z];
                s.Setup(new Vector2Int(i, j), (int)squaresList[z].SquareType);
                z++;
            }

        GameplayManager.Instance.BoardManager.Setup(board, mapSize);
    }
}
