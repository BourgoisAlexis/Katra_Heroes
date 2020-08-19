using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    [SerializeField]
    private Data data;
    [SerializeField]
    private Camera mainCamera;

    private e_teams team = e_teams.Blue;
    private DeckManager deck;
    private BoardManager board;
    private UIManager ui;
    private int phase;

    //Accessors
    public e_teams Team => team;

    public Data Data => data;
    public DeckManager Deck => deck;
    public BoardManager Board => board;
    public UIManager UI => ui;
    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(this);

        deck = GetComponent<DeckManager>();
        board = GetComponent<BoardManager>();
        ui = GetComponent<UIManager>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Down();

        if (Input.GetMouseButton(0))
            Maintain();

        if (Input.GetMouseButtonUp(0))
            Up();
    }


    private void Down()
    {
        //UIs
        GameObject _ui = ui.UIClick.DetectUI(Input.mousePosition);
        CardPiece _card = null;
        ActiveButton _active = null;
        if (_ui != null)
        {
            _card = _ui.GetComponent<CardPiece>();
            _active = _ui.GetComponent<ActiveButton>();
        }

        //Colliders
        Vector2 clickPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
        HeroPiece _piece = null;
        Square _square = null;
        if (hit.collider != null)
        {
            _piece = hit.transform.GetComponent<HeroPiece>();
            _square = hit.transform.GetComponent<Square>();
        }


        if (_card != null)
        {
            deck.ClickDrag(_card.transform);
            deck.ClickOnCard(_card);
        }
        else if (_active != null && _active.Piece.Team == team && _active.Piece.CanActive > 0)
            board.ClickOnActive(_active);


        else if (_piece != null)
        {
            if (deck.UsingCard)
                deck.TargetHero(_piece);
            else
            {
                if (_piece.Team == team)
                    board.ClickDrag(_piece.transform);

                board.ClickOnHero(_piece);
                deck.Unselection();
            }
        }
        else if (_square != null)
        {
            if (deck.UsingCard)
                deck.TargetBoard(_square);
            else
            {
                board.ClickOnBoard(_square);
                deck.Unselection();
            }
        }
        else
            UnselectAll();
    }

    private void Maintain()
    {
        if (deck.ToDrag != null)
            deck.Drag();
        else if (board.ToDrag != null)
            board.Drag();
    }

    private void Up()
    {
        if (deck.Dragging)
            deck.DragEnd();
        else if (board.Dragging)
            board.DragEnd();
        else if (board.CanUnselect)
            board.Unselection();
    }


    private void UnselectAll()
    {
        board.Unselection();
        deck.Unselection();
    }


    public void ChangePhase()
    {
        phase++;
    }
}
