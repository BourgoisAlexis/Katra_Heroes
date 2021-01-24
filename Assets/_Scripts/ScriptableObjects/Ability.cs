using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "CUSTOM/Ability")]
public class Ability : ScriptableObject
{
    #region Variables
    [SerializeField] protected e_targetting targetting;

    [Header("Ranges")]
    public e_rangeType RangeType;
    public int Range;
    public List<e_squareType> Exceptions;
    public bool GetOccupied;

    [Header("Stats")]
    public int Cost;
    public EffectDraw[] Draws;
    public EffectStat[] Stats;
    public EffectPosition[] Positions;

    private List<Effect> effects = new List<Effect>();

    //Accessors
    public e_targetting Targetting => targetting;
    public List<Effect> Effects => effects;
    #endregion


    public virtual void Set()
    {
        effects.Clear();

        effects.AddRange(Stats);
        effects.AddRange(Draws);
        effects.AddRange(Positions);

        foreach (Effect ef in effects)
            ef.Set();
    }

    public virtual void Use(Square[] _targets, HeroPiece _user)
    {
        foreach (Effect f in Effects)
        {
            f.Use(_targets, _user);
        }
    }
}
