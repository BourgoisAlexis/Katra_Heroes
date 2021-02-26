using System.Collections.Generic;
using UnityEngine;

public class BoardManager : DragDrop
{
    #region Variables
    [SerializeField] private GameObject heroePiecePrefab;

    private List<HeroPiece> pieces = new List<HeroPiece>();

    private List<Square> movementList = new List<Square>();
    private List<Square> attackList = new List<Square>();
    private List<Square> activeList = new List<Square>();

    private HeroPiece selectedPiece;
    private ActiveButton selectedActive;
    private bool canUnselect;
    
    private e_step step;
    private GameplayManager gameplayManager;
    private BoardUtility utility => gameplayManager.Utility;
    private Square[,] board => utility.Board;

    //Accessors
    public HeroPiece SelectedPiece => selectedPiece;
    public ActiveButton SelectedActive => selectedActive;
    public bool CanUnselect => canUnselect;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        gameplayManager = GetComponent<GameplayManager>();
    }


    public void GameStart(int[] _heroes)
    {
        List<Square> blueStarters = new List<Square>();
        List<Square> redStarters = new List<Square>();

        foreach (Square s in board)
        {
            if (s.Team == e_teams.Blue)
            {
                blueStarters.Add(s);
                s.HighLight(2);
            }
            else if (s.Team == e_teams.Red)
            {
                redStarters.Add(s);
                s.HighLight(3);
            }
        }

        for (int i = 0; i < _heroes.Length; i++)
        {
            if (_heroes[i] >= 0)
            {
                if (i > 2)
                    CreateHeroPiece(_heroes[i], e_teams.Red, redStarters[i - 3].Position);
                else
                    CreateHeroPiece(_heroes[i], e_teams.Blue, blueStarters[i].Position);
            }
        }
    }

    private void CreateHeroPiece(int _heroIndex, e_teams _team, Vector2Int _position)
    {
        Square start = board[_position.x, _position.y];
        GameObject instance = Instantiate(heroePiecePrefab, start.transform.position, Quaternion.identity, null);
        HeroUI ui = gameplayManager.UIManager.CreateHeroUI();
        HeroPiece piece = instance.GetComponent<HeroPiece>();

        piece.Setup(gameplayManager.Data.Heroes[_heroIndex], _team, pieces.Count, _position, ui);
        start.ChangeOccupied(instance.GetComponent<HeroPiece>());

        if (gameplayManager.Team == _team)
        {
            gameplayManager.OnYourTurn += piece.TurnDebute;
        }
        else
            gameplayManager.OnEnnemyTurn += piece.TurnDebute;

        pieces.Add(piece);
    }


    //Placement
    private void PlacementHero(HeroPiece _piece)
    {
        if (selectedPiece != null)
        {
            if (_piece == selectedPiece)
                canUnselect = true;
            else if (_piece != selectedPiece)
                Unselection();
        }
        else if (_piece.Team == gameplayManager.Team)
        {
            Unselection();

            ClickDrag(_piece.transform);
            selectedPiece = _piece;
            selectedPiece.HighLight(true);
            canUnselect = false;
        }

    }

    private void PlacementBoard(Square _square)
    {
        if (selectedPiece != null)
        {
            if (!_square.Occupied && _square.Team == gameplayManager.Team)
            {
                MoveHeroPiece(selectedPiece.Index, _square.Position, false);
                selectedPiece.HighLight(false);
                selectedPiece = null;
            }

            Unselection();
        }
    }

    public void PlacementDone()
    {
        gameplayManager.OnlineManager.ValidPlacement();
        gameplayManager.ChangeStep(e_step.Default);
    }

    public void PlacementValidated()
    {
        Unselection();

        foreach (HeroPiece p in pieces)
            if (p.Team == gameplayManager.Team)
                p.NotifyMovement(board[p.Position.x, p.Position.y], false);

        gameplayManager.OnlineManager.GameStart();
    }


    //InGame
    public void ClickOnHero(HeroPiece _piece)
    {
        if (step == e_step.Placement)
        {
            PlacementHero(_piece);
            return;
        }

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
                        selectedPiece.NotifyMovement(AttackPath(square), true);

                    Square[] targets = new Square[] { square };
                    selectedPiece.Attack(targets);
                    selectedPiece.HighLight(false);
                    selectedPiece = null;
                }
                Unselection();
            }
            else if (_piece != selectedPiece)
            {
                Unselection();
            }
        }
        else if (_piece.Team == gameplayManager.Team)
            if (_piece.CanMove > 0 || _piece.CanAct > 0)
            {
                Unselection();
                selectedPiece = _piece; 
                selectedPiece.HighLight(true);
                canUnselect = false;
                ClickDrag(_piece.transform);

                Ability atck = selectedPiece.Hero.Attack;
                EffectStat ef = (EffectStat)atck.Effects[0];

                ef.Value = - selectedPiece.Stats[e_stats.Damage].CurrentValue;
                atck.Range = selectedPiece.Stats[e_stats.Range].CurrentValue;
                atck.GetOccupied = true;

                Ability mov = selectedPiece.Hero.Move;
                List<e_squareType> exceps = new List<e_squareType>();
                if (selectedPiece.Hero.Fly == false)
                    exceps.Add(e_squareType.Obstacle);

                mov.Range = selectedPiece.Stats[e_stats.Speed].CurrentValue;
                mov.GetOccupied = false;
                mov.Exceptions = exceps;

                HighlightHeroRanges();
            }
    }

    public void ClickOnBoard(Square _square)
    {
        if (step == e_step.Placement)
        {
            PlacementBoard(_square);
            return;
        }

        if (selectedPiece != null)
        {
            if (movementList.Contains(_square) && !_square.Occupied)
            {
                selectedPiece.NotifyMovement(_square, true);
                selectedPiece.HighLight(false);
                selectedPiece = null;
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
        if (selectedPiece != null)
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
        {
            Unselection();
        }
    }


    public void Unselection()
    {
        if (selectedPiece != null)
        {
            Square square = board[selectedPiece.Position.x, selectedPiece.Position.y];
            if (step != e_step.Placement)
                selectedPiece.NotifyMovement(square, false);
            else
                MoveHeroPiece(selectedPiece.Index, square.Position, false);

            selectedPiece.HighLight(false);
        }

        selectedPiece = null;
        selectedActive = null;
        canUnselect = false;
        toDrag = null;

        movementList.Clear();
        attackList.Clear();
        activeList.Clear();

        if (step != e_step.Placement)
            foreach (Square s in board)
                s.UnHighLight();
    }


    private void HighlightHeroRanges()
    {
        if (selectedPiece == null)
            return;
        
        Vector2Int pos = selectedPiece.Position;

        if (selectedPiece.CanMove > 0)
        {
            movementList.Add(board[pos.x, pos.y]);
            List<e_squareType> excep = new List<e_squareType>();
            if (!selectedPiece.Hero.Fly)
                excep.Add(e_squareType.Obstacle);

            movementList = utility.GetRange(movementList, selectedPiece.Hero.Move);

            if (selectedPiece.CanAct > 0)
                attackList = utility.GetRange(movementList, selectedPiece.Hero.Attack);
        }
        else if (selectedPiece.CanAct > 0)
        {
            attackList.Add(board[pos.x, pos.y]);
            attackList = utility.GetRange(attackList, selectedPiece.Hero.Attack);
        }


        foreach (Square s in attackList)
            s.HighLight(3);
        foreach (Square s in movementList)
            s.HighLight(2);
    }

    private Square AttackPath(Square _target)
    {
        Square destination = null;
        int range = selectedPiece.Hero.Range;

        foreach (Square s in movementList)
        {
            int xDiff = s.Position.x - _target.Position.x;
            int yDiff = s.Position.y - _target.Position.y;
            int distance = Mathf.Abs(xDiff) + Mathf.Abs(yDiff);

            if (distance == range)
            {
                destination = s;
                break;
            }
        }

        return destination;
    }


    public void RemainingActions()
    {
        foreach (HeroPiece p in pieces)
            if (p != null && p.Team == gameplayManager.Team)
                if (p.CanActive > 0 || p.CanMove > 0 || p.CanAct > 0)
                    return;

        gameplayManager.NextStep();
    }

    public void RemoveHeroPiece(HeroPiece _piece)
    {
        board[_piece.Position.x, _piece.Position.y].ChangeOccupied(null);
        pieces[_piece.Index] = null;

        if (_piece.Team == gameplayManager.Team)
        {
            gameplayManager.OnYourTurn -= _piece.TurnDebute;

            foreach (HeroPiece p in pieces)
                if (p != null && p.Team == gameplayManager.Team)
                    return;

            gameplayManager.OnlineManager.Defeat();
        }
        else
            gameplayManager.OnEnnemyTurn -= _piece.TurnDebute;
    }

    public void UpdateStep(e_step _step)
    {
        step = _step;
    }


    //Online
    public void MoveHeroPiece(int _index, Vector2Int _desti, bool _useMove)
    {
        pieces[_index].MovePiece(board[_desti.x, _desti.y], _useMove);
    }

    public void ModifyStatHeroPiece(int _index, e_stats _key, int _value, int _duration, int _tick)
    {
        pieces[_index].ModifyStat(_key, _value, _duration, _tick);
    }
}
