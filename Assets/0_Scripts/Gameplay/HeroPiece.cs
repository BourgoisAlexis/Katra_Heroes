using System.Collections.Generic;
using UnityEngine;

public class HeroPiece : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private SpriteRenderer visual;

    private Transform _transform;
    private Hero hero;
    private HeroUI ui;
    Dictionary<e_stats, Stat> stats = new Dictionary<e_stats, Stat>();
    private e_teams team;
    private Vector2Int position;
    private int canMove;
    private int canAct;
    private int canActive;

    //Accessors
    public Hero Hero => hero;
    public Dictionary<e_stats, Stat> Stats => stats;
    public Vector2Int Position => position;
    public e_teams Team => team;
    public int CanMove => canMove;
    public int CanAct => canAct;
    public int CanActive => canActive;
    #endregion


    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(Hero _hero, e_teams _team, Vector2Int _position, HeroUI _heroUI)
    {
        _transform.position = new Vector3 (_transform.position.x, _transform.position.y, -1);
        hero = _hero;
        team = _team;
        position = _position;
        ui = _heroUI;
        ui.Setup(this);
        visual.sprite = hero.Graph;

        stats.Add(e_stats.Health, new Stat(hero.Health, false));
        stats.Add(e_stats.Movement, new Stat(hero.Movement, true));
        stats.Add(e_stats.Damage, new Stat(hero.Damage, true));
        stats.Add(e_stats.Range, new Stat(hero.Range, true));
        stats.Add(e_stats.Critic, new Stat(hero.Critic, true));

        hero.Attack.Set();
        hero.Active.Set();
        hero.Passive.Set();

        if (team == e_teams.Blue)
            ui.ChangeColor(GameplayManager.Instance.Data.Colors[4]);
        else if (team == e_teams.Red)
            ui.ChangeColor(GameplayManager.Instance.Data.Colors[5]);

        UpdateUI();
    }


    public void MovePiece(Square _oldSquare, Square _square)
    {
        _oldSquare.ChangeOccupied(null);
        _transform.position = new Vector3 (_square.transform.position.x, _square.transform.position.y, -1);
        position = _square.Position;
        _square.ChangeOccupied(this);

        canMove --;
        UpdateUI();
    }


    public void Attack(Square _target)
    {
        hero.Attack.Use(_target);
        canAct --;
    }

    public void ActivePower(Square _target)
    {
        hero.Active.Use(_target);
        ui.ActiveUsed(true);
        canActive--;
    }

    public void PassivePower(Square _target)
    {
        hero.Passive.Use(_target);
    }


    public void ModifyStat(e_stats _key, int _value, int _duration)
    {
        if (stats.ContainsKey(_key))
        {
            if (stats[_key].CanBeBuffed)
            {
                stats[_key].Buff(_value, _duration);
            }
            else
                stats[_key].UpdateValue(_value);
        }

        UpdateUI();
    }

    public void TurnDebute()
    {
        canMove = 1;
        canAct = 1;
        canActive = 1;
        ui.ActiveUsed(false);

        foreach (KeyValuePair<e_stats, Stat> s in stats)
            s.Value.Decrease();
    }


    public void UpdateUI()
    {
        ui.UpdateVisual(_transform.position, stats[e_stats.Health].CurrentValue, hero.Health);
    }
}