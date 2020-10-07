using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "CUSTOM/Ability/Stats")]
public class AbilityStats : Ability
{
    #region Variables
    public e_rangeType EffectRangeType;
    public int EffectRange;

    [Header("AbilityStats")]
    public e_stats Stat;
    public int Value;
    public int Duration;
    #endregion


    public override void Set()
    {
        abilityType = e_abilityType.Stats;
    }

    public override void Use(Square _target, HeroPiece _user)
    {
        if (_target.Piece != null)
            _target.Piece.ModifyStat(Stat, Value, Duration);
    }
}
