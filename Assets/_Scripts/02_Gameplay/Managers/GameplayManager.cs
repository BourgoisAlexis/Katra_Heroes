using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    public event Action OnYourTurn;
    public event Action OnEnnemyTurn;

    [SerializeField] private Data data;
    [SerializeField] private MapGenerator map;

    [SerializeField] private AnimationClip transiAnim;
    [SerializeField] private AnimationClip endAnim;

    private BoardUtility utility;
    private DeckManager deckManager;
    private BoardManager boardManager;
    private UIManager uiManager;
    private PoolManager poolManager;
    private OnlineManager onlineManager;
    private GameplayInputs inputManager;

    [Header("Helpers")]
    private e_teams team;
    private e_step step;
    private int maxMana;
    private int cardNumber = 5;
    private int mana;

    private bool eliminated;

    //Accessors
    public e_teams Team => team;
    public Data Data => data;
    public int Mana => mana;

    public BoardUtility Utility => utility;
    public DeckManager DeckManager => deckManager;
    public BoardManager BoardManager => boardManager;
    public UIManager UIManager => uiManager;
    public PoolManager PoolManager => poolManager;
    public OnlineManager OnlineManager => onlineManager;
    #endregion


    //Start
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(this);

        deckManager = GetComponent<DeckManager>();
        boardManager = GetComponent<BoardManager>();
        uiManager = GetComponent<UIManager>();
        poolManager = GetComponent<PoolManager>();
        inputManager = GetComponent<GameplayInputs>();

        maxMana = 3;

        map.Setup();
    }

    public void Init(OnlineManager _onlineManager, e_teams _team, int[] _heroes)
    {
        team = _team;
        onlineManager = _onlineManager;
        utility = new BoardUtility(map.Board, map.MapSize);

        inputManager.GameStart(team);
        boardManager.GameStart(_heroes);
        deckManager.GameStart();

        ChangeStep(e_step.Placement);
    }

    public void GameStart()
    {
        StartCoroutine(FirstDrawCorout());
    }


    //End
    public void GameEnd()
    {
        if (!eliminated)
            uiManager.GameEnd(true);

        eliminated = true;

        StartCoroutine(GameEndCorout());
    }

    private IEnumerator GameEndCorout()
    {
        yield return new WaitForSeconds(endAnim.length + 1);
        onlineManager.Disconnect();
        Destroy(onlineManager.gameObject);

        SceneTransi.ToExecute exe = LoadLobby;
        SceneTransi.Instance.Transi(true, exe);
    }

    private void LoadLobby()
    {
        SceneManager.LoadScene(0);
    }


    //Steps
    public void ChangeStep(e_step _step)
    {
        step = _step;

        inputManager.UpdateStep(_step);
        boardManager.UpdateStep(_step);
    }

    public void ModifyMana(int _value)
    {
        mana += _value;
        uiManager.UpdateMana();

        if (mana <= 0)
            if (step == e_step.Card || step == e_step.UseCard)
            {
                step = e_step.Card;
                NextStep();
            }
    }

    private IEnumerator FirstDrawCorout()
    {
        ChangeStep(e_step.Draw);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < cardNumber; i++)
        {
            deckManager.DrawCard();
            yield return new WaitForSeconds(0.1f);
        }

        uiManager.GameStart();

        if (team != e_teams.Blue)
            EnnemyTurn();
    }


    public void YourTurn()
    {
        if (OnYourTurn != null)
            OnYourTurn();

        ChangeStep(e_step.Draw);
        deckManager.DrawCard();
        mana = maxMana;
        UIManager.UpdateMana();
        NextStep();
    }

    public void EnnemyTurn()
    {
        if (OnEnnemyTurn != null)
            OnEnnemyTurn();

        StartCoroutine(StepTansition(e_step.Ennemy));
    }

    public void Eliminated(e_teams _team)
    {
        if (_team == team)
        {
            eliminated = true;
            uiManager.GameEnd(false);
        }
        else
        {
            Debug.Log("le joueur " + _team + " a perdu");
        }
    }


    public void NextStep()
    {
        switch (step)
        {
            case e_step.Draw:
                StartCoroutine(StepTansition(e_step.Card));
                break;

            case e_step.Card:
                StartCoroutine(StepTansition(e_step.Board));
                break;

            case e_step.Board:
                EnnemyTurn();
                break;
        }
    }

    public IEnumerator StepTansition(e_step _final)
    {
        ChangeStep(e_step.Default);
        UIManager.Transition(_final);

        yield return new WaitForSeconds(transiAnim.length);

        if (!eliminated)
            ChangeStep(_final);
    }
}
