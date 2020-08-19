using System.Collections.Generic;
using UnityEngine;

public class BoardManager : DragDrop
{
    #region Variables
    [SerializeField]
    private GameObject heroePiecePrefab;

    private BoardUtility utility;
    private GameplayManager gameplayManager;

    private Vector2Int mapSize;
    private Square[,] board;

    [SerializeField]
    private List<HeroPiece> heroPieces = new List<HeroPiece>();
    private List<Square> movementList = new List<Square>();
    private List<Square> attackList = new List<Square>();
    private List<Square> activeList = new List<Square>();

    private HeroPiece selectedPiece;
    private ActiveButton selectedActive;
    private bool canUnselect;

    //Accessors
    public Square[,] Board => board;
    public HeroPiece SelectedPiece => selectedPiece;
    public bool CanUnselect => canUnselect;
    public ActiveButton SelectedActive => selectedActive;
    public BoardUtility Utility => utility;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        gameplayManager = GetComponent<GameplayManager>();
    }

    private void Start()
    {
        CreateHeroPiece(0, e_teams.Blue, new Vector2Int(Random.Range(0, mapSize.x - 1), Random.Range(0, mapSize.y - 1)));
        CreateHeroPiece(1, e_teams.Red, new Vector2Int(Random.Range(0, mapSize.x - 1), Random.Range(0, mapSize.y - 1)));

        foreach (HeroPiece piece in heroPieces)
            piece.TurnDebute();
    }

    public void Setup(Square[,] _board, Vector2Int _mapSize)
    {
        board = _board;
        mapSize = _mapSize;

        utility = new BoardUtility(board, mapSize);
        gameplayManager.Deck.Setup(utility);
    }

    private void CreateHeroPiece(int _heroIndex, e_teams _team, Vector2Int _position)
    {
        Square start = board[_position.x, _position.y];
        GameObject instance = Instantiate(heroePiecePrefab, start.transform.position, Quaternion.identity, null);
        HeroUI ui = gameplayManager.UI.CreateHeroUI();
        HeroPiece piece = instance.GetComponent<HeroPiece>();

        piece.Setup(gameplayManager.Data.Heroes[_heroIndex], _team, _position, ui);
        start.ChangeOccupied(instance.GetComponent<HeroPiece>());
        heroPieces.Add(piece);
    }


    public void ClickOnHero(HeroPiece _piece)
    {
        if (selectedActive != null)
        {
            if (_piece == selectedActive.Piece)
                Unselection();
            else
            {
                Square square = board[_piece.Position.x, _piece.Position.y];
                if (activeList.Contains(square))
                {
                    selectedActive.Piece.ActivePower(square);
                    Unselection();
                }
            }
        }
        else if (selectedPiece != null)
        {
            if (_piece == selectedPiece)
                canUnselect = true;

            else if (_piece.Team != gameplayManager.Team)
            {
                Square square = board[_piece.Position.x, _piece.Position.y];
                if (attackList.Contains(square))
                {
                    if (selectedPiece.CanMove > 0)
                    {
                        Square start = board[selectedPiece.Position.x, selectedPiece.Position.y];
                        selectedPiece.MovePiece(start, FindPath(square));
                    }
                    selectedPiece.Attack(square);
                }
                Unselection();
            }
        }
        else if (_piece.Team == gameplayManager.Team)
            if (_piece.CanMove > 0 || _piece.CanAct > 0)
            {
                Unselection();
                selectedPiece = _piece;
                toDrag = selectedPiece.transform;
                selectedPiece.Hero.Attack.Value = - selectedPiece.Stats[e_stats.Damage].CurrentValue;
                selectedPiece.Hero.Attack.Range = selectedPiece.Stats[e_stats.Range].CurrentValue;
                canUnselect = false;
                HighlightHeroRanges();
            }
    }

    public void ClickOnBoard(Square _square)
    {
        if (selectedPiece != null)
        {
            if (movementList.Contains(_square) && !_square.Occupied)
            {
                Square start = board[selectedPiece.Position.x, selectedPiece.Position.y];
                selectedPiece.MovePiece(start, _square);
                Unselection();
            }
            else
                Unselection();
        }
        else if (selectedActive != null)
        {
            if (!activeList.Contains(_square))
                Unselection();
        }
    }

    public void ClickOnActive(ActiveButton _active)
    {
        gameplayManager.Deck.Unselection();
        Unselection();
        selectedActive = _active;
        HeroPiece piece = _active.Piece;
        Ability activeAbility = piece.Hero.Active;

        if (activeAbility.AbilityType != e_abilityType.Creation && activeAbility.AbilityType != e_abilityType.Draw)
        {
            activeList.Add(board[piece.Position.x, piece.Position.y]);
            activeList = utility.GetRangeSpe(activeList, piece.Hero.Active);

            foreach (Square s in activeList)
                s.HighLight(1);
        }
        else
        {
            piece.ActivePower(null);
        }
    }

    public override void Drag()
    {
        base.Drag();
        if(selectedPiece != null)
            selectedPiece.UpdateUI();
    }

    public override void DragEnd()
    {
        base.DragEnd();

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] cols = Physics2D.OverlapCircleAll(mousePos, 0.2f);

        HeroPiece _piece = null;
        Square _square = null;

        if (cols.Length > 1)
        {
            foreach (Collider2D c in cols)
            {
                if (c.GetComponent<HeroPiece>())
                {
                    if (c.GetComponent<HeroPiece>() != selectedPiece)
                        _piece = c.GetComponent<HeroPiece>();
                }
                else if (c.GetComponent<Square>())
                    _square = c.GetComponent<Square>();
            }

            if (_piece != null)
                ClickOnHero(_piece);

            else if (_square != null)
                ClickOnBoard(_square);
        }
        else
            Unselection();
    }


    public void Unselection()
    {
        if (selectedPiece != null)
        {
            Vector2 pos = board[selectedPiece.Position.x, selectedPiece.Position.y].transform.position;
            selectedPiece.transform.position = new Vector3(pos.x, pos.y, -1);
            selectedPiece.UpdateUI();
        }

        selectedPiece = null;
        selectedActive = null;
        canUnselect = false;

        movementList.Clear();
        attackList.Clear();
        activeList.Clear();

        foreach (Square s in board)
            s.UnHighLight();

        toDrag = null;
    }



    private void HighlightHeroRanges()
    {
        if (selectedPiece == null)
            return;
        
        Vector2Int pos = selectedPiece.Position;

        //Movement 
        if (selectedPiece.CanMove > 0)
        {
            movementList.Add(board[pos.x, pos.y]);
            List<e_squareType> excep = new List<e_squareType>();
            if (!selectedPiece.Hero.Fly)
                excep.Add(e_squareType.Obstacle);
            movementList = utility.GetRange(movementList, selectedPiece.Stats[e_stats.Movement].CurrentValue, excep, false);
            attackList = utility.GetRangeSpe(movementList, selectedPiece.Hero.Attack);
        }

        //Attack
        else if (selectedPiece.CanAct > 0)
        {
            attackList.Add(board[pos.x, pos.y]);
            attackList = utility.GetRangeSpe(attackList, selectedPiece.Hero.Attack);
        }


        foreach (Square s in attackList)
            s.HighLight(2);
        foreach (Square s in movementList)
            s.HighLight(3);
    }

    private Square FindPath(Square _interact)
    {
        Square destination = null;
        int currentDist = 0;

        foreach (Square s in movementList)
        {
            int xDiff = s.Position.x - _interact.Position.x;
            int yDiff = s.Position.y - _interact.Position.y;
            int total = Mathf.Abs(xDiff) + Mathf.Abs(yDiff);

            if (currentDist == 0 || total < currentDist)
            {
                currentDist = total;
                destination = s;
            }
        }

        return destination;
    }
}
