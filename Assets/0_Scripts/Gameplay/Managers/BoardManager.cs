using System.Collections.Generic;
using UnityEngine;

public class BoardManager : DragDrop
{
    #region Variables
    [SerializeField] private GameObject heroePiecePrefab;

    private BoardUtility utility;
    private GameplayManager gameplayManager;

    private Vector2Int mapSize;
    private Square[,] board;

    private List<HeroPiece> pieces = new List<HeroPiece>();

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
    #endregion


    protected override void Awake()
    {
        base.Awake();

        gameplayManager = GetComponent<GameplayManager>();
    }

    private void Start()
    {
        CreateHeroPiece(0, e_teams.Blue, new Vector2Int(5, 4));
        CreateHeroPiece(0, e_teams.Blue, new Vector2Int(4, 5));
        CreateHeroPiece(1, e_teams.Red, new Vector2Int(5, 5));
    }

    public void Setup(Square[,] _board, Vector2Int _mapSize)
    {
        board = _board;
        mapSize = _mapSize;

        utility = gameplayManager.Utility;
    }

    private void CreateHeroPiece(int _heroIndex, e_teams _team, Vector2Int _position)
    {
        Square start = board[_position.x, _position.y];
        GameObject instance = Instantiate(heroePiecePrefab, start.transform.position, Quaternion.identity, null);
        HeroUI ui = gameplayManager.UIManager.CreateHeroUI();
        HeroPiece piece = instance.GetComponent<HeroPiece>();

        piece.Setup(gameplayManager.Data.Heroes[_heroIndex], _team, _position, ui);
        start.ChangeOccupied(instance.GetComponent<HeroPiece>());

        if (gameplayManager.Team == _team)
        {
            pieces.Add(piece);
            gameplayManager.OnYourTurn += piece.TurnDebute;
        }
        else
            gameplayManager.OnEnnemyTurn += piece.TurnDebute;
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
                    Square[] targets = new Square[] { square };
                    selectedActive.Piece.ActivePower(targets);

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
                        selectedPiece.MovePiece(FindPath(square), true);

                    Square[] targets = new Square[] { square };
                    selectedPiece.Attack(targets);
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
                selectedPiece.Hero.Move.Range = selectedPiece.Stats[e_stats.Speed].CurrentValue;
                SelectedPiece.Hero.Move.GetOccupied = selectedPiece.Hero.Fly;
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
                selectedPiece.MovePiece(_square, true);
                Unselection();
            }
            else
                Unselection();
        }
        else if (selectedActive != null)
        {
            if (activeList.Contains(_square))
            {
                Square[] targets = new Square[] { _square };
                selectedActive.Piece.ActivePower(targets);

                Unselection();
            }
        }
    }

    public void ClickOnActive(ActiveButton _active)
    {
        gameplayManager.DeckManager.Unselection();
        Unselection();
        selectedActive = _active;
        HeroPiece piece = _active.Piece;
        Ability activeAbility = piece.Hero.Active;

        if (activeAbility.Targetting == e_targetting.Default)
        {
            activeList.Add(board[piece.Position.x, piece.Position.y]);
            activeList = utility.GetRange(activeList, piece.Hero.Active);

            foreach (Square s in activeList)
                s.HighLight(1);
        }
        else if (activeAbility.Targetting == e_targetting.AutomaticTarget)
        {
            activeList.Add(board[piece.Position.x, piece.Position.y]);
            activeList = utility.GetRange(activeList, piece.Hero.Active);

            piece.ActivePower(activeList.ToArray());
            Unselection();
        }
        else
        {
            piece.ActivePower(null);
            Unselection();
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
            movementList = utility.GetRange(movementList, selectedPiece.Hero.Move);
            attackList = utility.GetRange(movementList, selectedPiece.Hero.Attack);
        }

        //Attack
        else if (selectedPiece.CanAct > 0)
        {
            attackList.Add(board[pos.x, pos.y]);
            attackList = utility.GetRange(attackList, selectedPiece.Hero.Attack);
        }


        foreach (Square s in attackList)
            s.HighLight(2);
        foreach (Square s in movementList)
            s.HighLight(3);
    }

    private Square FindPath(Square _target)
    {
        Square destination = null;
        int currentDist = 100;

        foreach (Square s in movementList)
        {
            int xDiff = s.Position.x - _target.Position.x;
            int yDiff = s.Position.y - _target.Position.y;
            int total = Mathf.Abs(xDiff) + Mathf.Abs(yDiff);

            if (total < currentDist)
            {
                currentDist = total;
                destination = s;
            }
        }

        return destination;
    }


    public void CheckForNext()
    {
        foreach (HeroPiece p in pieces)
        {
            if (p.CanActive > 0 || p.CanMove > 0 || p.CanAct > 0)
                return;
        }

        gameplayManager.NextStep();
    }
}
