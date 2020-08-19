using UnityEngine;

public class Ability : ScriptableObject
{
    public Sprite Graph;
    protected e_abilityType abilityType;

    [Header("Ranges")]
    public e_rangeType RangeType;
    public int Range;

    //Accessors
    public e_abilityType AbilityType => abilityType;


    public virtual void Set()
    {

    }

    public virtual void Use(Square _target)
    {
        
    }
}
