using UnityEngine;

public class Square : MonoBehaviour
{
    #region Variables
    [SerializeField] private e_squareType squareType;

    private Vector2Int position;
    private bool occupied;
    private HeroPiece piece;
    private SpriteRenderer visual;
    private SpriteRenderer overlay;

    //Accessors
    public e_squareType SquareType => squareType;
    public bool Occupied => occupied;
    public Vector2Int Position => position;
    public HeroPiece Piece => piece;
    #endregion


    public void Setup(Vector2Int _position, int _type)
    {
        position = _position;
        squareType = (e_squareType)_type;

        Colorisation();
    }

    public void Colorisation()
    {

        visual = GetComponentInChildren<SpriteRenderer>();
        switch (squareType)
        {
            case e_squareType.Default:
                visual.color = Color.grey;
                break;

            case e_squareType.Ground:
                visual.color = Color.green;
                break;

            case e_squareType.Obstacle:
                visual.color = Color.red;
                break;
        }
    }


    private void Awake()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        visual = renderers[0];
        overlay = renderers[1];
    }

    public void ChangeOccupied(HeroPiece _piece)
    {
        piece = _piece;
        occupied = !occupied;
    }

    public void HighLight(int _color)
    {
        overlay.color = GameplayManager.Instance.Data.Colors[_color];
    }

    public void UnHighLight()
    {
        overlay.color = GameplayManager.Instance.Data.Colors[0];
    }
}
