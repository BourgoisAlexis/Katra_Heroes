using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    #region Variables
    public Sprite Graph;
    protected e_abilityType abilityType;

    [Header("Ranges")]
    public e_rangeType RangeType;
    public int Range;
    public List<e_squareType> Exceptions;
    public bool GetOccupied;

    //Accessors
    public e_abilityType AbilityType => abilityType;
    #endregion


    public virtual void Set()
    {

    }

    public virtual void Use(Square _target, HeroPiece _user)
    {
        
    }
}
