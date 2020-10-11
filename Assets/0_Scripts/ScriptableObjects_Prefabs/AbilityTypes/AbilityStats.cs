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


    public override void Use(Square[] _targets, HeroPiece _user)
    {
        foreach (Square s in _targets)
        {
            if (s.Piece != null)
                s.Piece.ModifyStat(Stat, Value, Duration);
        }
    }
}
