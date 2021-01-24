using UnityEngine;

public class Square : MonoBehaviour
{
    #region Variables
    [SerializeField] private e_squareType squareType;
    [SerializeField] private e_teams team;

    private Vector2Int position;
    private bool occupied;
    private HeroPiece piece;
    private SpriteRenderer visual;
    private SpriteRenderer overlay;

    //Accessors
    public e_squareType SquareType => squareType;
    public e_teams Team => team;
    public bool Occupied => occupied;
    public Vector2Int Position => position;
    public HeroPiece Piece => piece;
    #endregion


    public void Setup(Vector2Int _position, e_squareType _type, e_teams _team)
    {
        position = _position;
        squareType = _type;
        team = _team;

        Colorisation();
    }

    public void Colorisation()
    {

        visual = GetComponentInChildren<SpriteRenderer>();
        switch (squareType)
        {
            case e_squareType.Default:
                visual.color = Color.red;
                break;

            case e_squareType.Ground:
                visual.color = Color.white;
                break;

            case e_squareType.Obstacle:
                visual.color = Color.black;
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

        if (piece == null)
            occupied = false;
        else
            occupied = true;
    }

    public void HighLight(int _color)
    {
        overlay.color = GameplayManager.Instance.Data.Colors[_color];
        Color c = overlay.color;
        c.a = 0.7f;
        overlay.color = c;
    }

    public void UnHighLight()
    {
        overlay.color = GameplayManager.Instance.Data.Colors[0];
    }
}
