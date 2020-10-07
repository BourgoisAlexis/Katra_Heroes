﻿using System;
using System.Collections;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    public event Action OnYourTurn;
    public event Action OnEnnemyTurn;

    [SerializeField] private Data data;
    [SerializeField] private Camera mainCamera;

    private e_teams team = e_teams.Blue;
    private DeckManager deckManager;
    private BoardManager boardManager;
    private UIManager uiManager;

    [SerializeField]
    private e_step step;
    [SerializeField]
    private int mana;
    [SerializeField]
    private bool turn;

    //Accessors
    public e_teams Team => team;
    public Data Data => data;
    public DeckManager DeckManager => deckManager;
    public BoardManager BoardManager => boardManager;
    public UIManager UIManager => uiManager;
    public int Mana => mana;
    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(this);

        deckManager = GetComponent<DeckManager>();
        boardManager = GetComponent<BoardManager>();
        uiManager = GetComponent<UIManager>();
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
        GameObject _ui = uiManager.UIClick.DetectUI(Input.mousePosition);
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

        if (step == e_step.Card)
        {
            if(_card != null)
            {
                deckManager.ClickDrag(_card.transform);
                deckManager.ClickOnCard(_card);
            }

            else
                UnselectAll();
        }

        else if (step == e_step.UseCard)
        {
            if (_piece != null)
                deckManager.TargetHero(_piece);
            else if (_square != null)
                deckManager.TargetBoard(_square);
        }

        else if (step == e_step.Board)
        {
            if (_active != null && _active.Piece.Team == team && _active.Piece.CanActive > 0)
                boardManager.ClickOnActive(_active);
            else if (_piece != null)
            {
                if (_piece.Team == team)
                    boardManager.ClickDrag(_piece.transform);

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

    private void Maintain()
    {
        if (deckManager.ToDrag != null)
            deckManager.Drag();
        else if (boardManager.ToDrag != null)
            boardManager.Drag();
    }

    private void Up()
    {
        if (deckManager.Dragging)
            deckManager.DragEnd();
        else if (boardManager.Dragging)
            boardManager.DragEnd();
        else if (boardManager.CanUnselect)
            boardManager.Unselection();
    }


    private void UnselectAll()
    {
        boardManager.Unselection();
        deckManager.Unselection();
    }


    public void ChangeStep(int _step)
    {
        step = (e_step)_step;
    }

    public void UpdateMana(int _value)
    {
        mana += _value;
        uiManager.UpdateMana();

        if (mana <= 0)
            BoardStepDebute();
    }


    //Map(board datas) => Board(board utility) => Deck => DebuteDraw
    public void DebuteDraw()
    {
        StartCoroutine(DebuteDrawCorout());
    }

    private IEnumerator DebuteDrawCorout()
    {
        ChangeStep(1);

        int t = 5;
        yield return new WaitForSeconds(1);

        for (int i = 0; i < t; i++)
        {
            deckManager.DrawCard();
            yield return new WaitForSeconds(0.1f);
        }

        if (turn)
            YourTurn();
        else
            EnnemyTurn();

    }

    public void YourTurn()
    {
        if (OnYourTurn != null)
            OnYourTurn();

        ChangeStep(2);
        //ChangeStep(4);
    }

    public void EnnemyTurn()
    {
        if (OnEnnemyTurn != null)
            OnEnnemyTurn();

        ChangeStep(5);
    }

    public void BoardStepDebute()
    {
        ChangeStep(4);
    }
}
