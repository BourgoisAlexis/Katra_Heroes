using UnityEngine;

public class GameplayInputs : InputManager
{
    #region Variables
    private DeckManager deckManager;
    private BoardManager boardManager;

    private e_teams team;
    private e_step step;
    #endregion


    private void Awake()
    {
        deckManager = GetComponent<DeckManager>();
        boardManager = GetComponent<BoardManager>();
    }

    public void GameStart(e_teams _team)
    {
        team = _team;
    }

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Down();

        if (Input.GetMouseButton(0))
            Maintain();

        if (Input.GetMouseButtonUp(0))
            Up();
    }


    protected override void Down()
    {
        GameObject ui = DetectUI(Input.mousePosition);
        GameObject col = DetectCollider(Input.mousePosition);

        //UIs
        CardPiece _card = null;
        ActiveButton _active = null;

        if (ui != null)
        {
            _card = ui.GetComponent<CardPiece>();
            _active = ui.GetComponent<ActiveButton>();

            ui.GetComponent<Clickable>()?.Onclick();
        }

        //Colliders
        HeroPiece _piece = null;
        Square _square = null;

        if (col != null)
        {
            _piece = col.GetComponent<HeroPiece>();
            _square = col.GetComponent<Square>();

            col.GetComponent<Clickable>()?.Onclick();
        }

        //Steps
        if (step == e_step.Placement)
        {
            if (_piece != null)
            {
                boardManager.ClickOnHero(_piece);
            }
            else if (_square != null)
            {
                boardManager.ClickOnBoard(_square);
            }
        }

        else if (step == e_step.Card)
        {
            if (_card != null)
            {
                deckManager.ClickOnCard(_card);
            }
            else
                UnselectAll();
        }

        else if (step == e_step.UseCard)
        {
            if (_piece != null)
            {
                deckManager.TargetHero(_piece);
            }
            else if (_square != null)
            {
                deckManager.TargetBoard(_square);
            }
        }

        else if (step == e_step.Board)
        {
            if (_active != null && _active.Piece.Team == team && _active.CheckMana() && _active.Piece.CanActive > 0)
            {
                boardManager.ClickOnActive(_active);
            }
            else if (_piece != null)
            {
                boardManager.ClickOnHero(_piece);
            }
            else if (_square != null)
            {
                boardManager.ClickOnBoard(_square);
            }
            else
                UnselectAll();
        }

        else if (step == e_step.Ennemy)
        {
            if (_piece != null)
            {
                //Afficher les stats quand ce sera fait
            }
            else if (_card != null)
            {
                //Afficher les stats quand ce sera fait
            }
        }
    }

    protected override void Maintain()
    {
        if (deckManager.ToDrag != null)
            deckManager.Drag();
        else if (boardManager.ToDrag != null)
            boardManager.Drag();
    }

    protected override void Up()
    {
        if (deckManager.Dragging)
            deckManager.DragEnd();
        else if (boardManager.Dragging)
            boardManager.DragEnd();
        else if (boardManager.CanUnselect)
            boardManager.Unselection();
    }


    public void UpdateStep(e_step _step)
    {
        step = _step;

        if (step == e_step.Board)
            deckManager.Unselection();
        else if (step == e_step.Card)
            boardManager.Unselection();
    }


    private void UnselectAll()
    {
        boardManager.Unselection();
        deckManager.Unselection();
    }
}
