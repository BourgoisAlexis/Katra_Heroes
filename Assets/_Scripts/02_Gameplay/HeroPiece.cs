using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HeroPiece : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer visual;

    private Transform _transform;
    private Hero hero;
    private HeroUI ui;
    Dictionary<e_stats, Stat> stats = new Dictionary<e_stats, Stat>();
    private e_teams team;
    private Vector2Int position;
    private int canMove;
    private int canAct;
    private int canActive;
    private int index;

    //Accessors
    public Hero Hero => hero;
    public Dictionary<e_stats, Stat> Stats => stats;
    public Vector2Int Position => position;
    public e_teams Team => team;
    public int CanMove => canMove;
    public int CanAct => canAct;
    public int CanActive => canActive;
    public int Index => index;
    #endregion


    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(Hero _hero, e_teams _team, int _index, Vector2Int _position, HeroUI _heroUI)
    {
        _transform.position = new Vector3 (_transform.position.x, _transform.position.y, -1);
        hero = _hero;
        team = _team;
        index = _index;
        position = _position;
        ui = _heroUI;
        Color color = _team == e_teams.Blue ? GameplayManager.Instance.Data.Colors[2] : GameplayManager.Instance.Data.Colors[3];
        ui.Setup(this, color);
        visual.sprite = hero.Graph;

        stats.Add(e_stats.Health, new Stat(e_stats.Health, hero.Health));
        stats.Add(e_stats.Speed, new Stat(e_stats.Speed, hero.Speed));
        stats.Add(e_stats.Damage, new Stat(e_stats.Damage, hero.Damage));
        stats.Add(e_stats.Range, new Stat(e_stats.Range, hero.Range));
        stats.Add(e_stats.Critic, new Stat(e_stats.Critic, hero.Critic));

        hero.Attack.Set();
        hero.Active.Set();
        hero.Passive.Set();

        UpdateUI();
    }


    public void MovePiece(Square _square, bool _useMove)
    {
        if (_square.Position == position)
            return;

        GameplayManager.Instance.Utility.Board[position.x, position.y].ChangeOccupied(null);
        StartCoroutine(Move (new Vector3 (_square.transform.position.x, _square.transform.position.y, -1)));
        position = _square.Position;
        _square.ChangeOccupied(this);

        if (_useMove)
            canMove --;

        UpdateUI();

        GameplayManager.Instance.BoardManager.RemainingActions();
    }

    private IEnumerator Move(Vector3 _desti)
    {
        int iterrations = 5;
        Vector3 start = _transform.position;
        Vector3 step = (_desti - start) / iterrations;

        while (iterrations != 0)
        {
            _transform.position += step;
            iterrations--;
            yield return new WaitForFixedUpdate();
            UpdateUI();
        }

        _transform.position = _desti;
        UpdateUI();
    }


    public void Attack(Square[] _targets)
    {
        hero.Attack.Use(_targets, this);
        canAct --;

        GameplayManager.Instance.BoardManager.RemainingActions();
    }

    public void ActivePower(Square[] _targets)
    {
        hero.Active.Use(_targets, this);
        GameplayManager.Instance.ModifyMana(-hero.Active.Cost);
        ui.ActiveUsed(true);
        canActive--;

        GameplayManager.Instance.BoardManager.RemainingActions();
    }

    public void PassivePower(Square[] _targets)
    {
        hero.Passive.Use(_targets, this);
    }


    public void ModifyStat(e_stats _key, int _value, int _duration, int _tick)
    {
        if (stats.ContainsKey(_key))
        {
            stats[_key].UpdateValue(_value, _duration, _tick);
        }
        if (_key == e_stats.Health && _value < 0)
            GameplayManager.Instance.PoolManager.Instantiate("BaseSlash", transform.position, Vector3.zero, null);
        else
        {
            GameObject fx = GameplayManager.Instance.PoolManager.Instantiate("BaseBuff", transform.position, Vector3.zero, null);
            fx.GetComponent<BuffFX>()?.ChangeColor(_key);
        }

        UpdateUI();

        if (stats[e_stats.Health].CurrentValue <= 0)
            Death();
    }

    public void TurnDebute()
    {
        canMove = 1;
        canAct = 1;
        canActive = 1;
        ui.ActiveUsed(false);

        foreach (KeyValuePair<e_stats, Stat> s in stats)
            s.Value.Decrease(this);
    }

    private void Death()
    {
        ui.gameObject.SetActive(false);
        GameplayManager.Instance.BoardManager.RemoveHeroPiece(this);
        gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        ui.UpdateVisual(_transform.position, stats[e_stats.Health].CurrentValue, hero.Health);
    }

    public void HighLight(bool _highlight)
    {
        visual.material.SetColor("_Color", Color.white * (_highlight ? 0.2f : 0));
    }


    //Online
    public void NotifyMovement(Square _square, bool _useMove)
    {
        GameplayManager.Instance.OnlineManager.MoveHeroPiece(index, _square.Position, _useMove);
        //MovePiece(_square, _useMove);
    }

    public void NotifyStat(e_stats _key, int _value, int _duration, int _tick)
    {
        GameplayManager.Instance.OnlineManager.ModifyStatHeroPiece(index, _key, _value, _duration, _tick);
        //ModifyStat(_key, _value, _duration);
    }
}