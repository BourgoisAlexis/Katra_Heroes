﻿using System.Collections.Generic;
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
        Color color = _team == e_teams.Blue ? GameplayManager.Instance.Data.Colors[4] : GameplayManager.Instance.Data.Colors[5];
        ui.Setup(this, color);
        visual.sprite = hero.Graph;

        stats.Add(e_stats.Health, new Stat(hero.Health, false));
        stats.Add(e_stats.Speed, new Stat(hero.Speed, true));
        stats.Add(e_stats.Damage, new Stat(hero.Damage, true));
        stats.Add(e_stats.Range, new Stat(hero.Range, true));
        stats.Add(e_stats.Critic, new Stat(hero.Critic, true));

        hero.Attack.Set();
        hero.Active.Set();
        hero.Passive.Set();

        UpdateUI();
    }


    public void MovePiece(Square _square, bool _useMove)
    {
        GameplayManager.Instance.BoardManager.Board[position.x, position.y].ChangeOccupied(null);
        _transform.position = new Vector3 (_square.transform.position.x, _square.transform.position.y, -1);
        position = _square.Position;
        _square.ChangeOccupied(this);

        if (_useMove)
            canMove --;

        UpdateUI();

        GameplayManager.Instance.BoardManager.CheckForNext();
    }


    public void Attack(Square[] _targets)
    {
        hero.Attack.Use(_targets, this);
        canAct --;

        GameplayManager.Instance.BoardManager.CheckForNext();
    }

    public void ActivePower(Square[] _targets)
    {
        hero.Active.Use(_targets, this);
        ui.ActiveUsed(true);
        canActive--;

        GameplayManager.Instance.BoardManager.CheckForNext();
    }

    public void PassivePower(Square[] _targets)
    {
        hero.Passive.Use(_targets, this);
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
        if (_key == e_stats.Health)
            GameplayManager.Instance.PoolManager.Instantiate("BaseSlash", transform.position, Vector3.zero, null);
        else
            GameplayManager.Instance.PoolManager.Instantiate("BaseParticle", transform.position, Vector3.zero, null);


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