using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    #region Variables
    public Sprite Graph;
    [SerializeField] protected e_targetting targetting;

    [Header("Ranges")]
    public e_rangeType RangeType;
    public int Range;
    public List<e_squareType> Exceptions;
    public bool GetOccupied;

    //Accessors
    public e_targetting Targetting => targetting;
    #endregion


    public virtual void Set()
    {

    }

    public virtual void Use(Square[] _targets, HeroPiece _user)
    {
        
    }
}
