using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EffectStat : Effect
{
    #region Variables
    public e_rangeType EffectRangeType;
    public int EffectRange;

    [Header("AbilityStats")]
    public e_stats Stat;
    public int Value;
    public int Tick;
    public int Duration;
    #endregion


    public override void Use(Square[] _targets, HeroPiece _user)
    {
        List<Square> targets = new List<Square>();
        targets.AddRange(_targets);
        targets = GameplayManager.Instance.Utility.GetAOE(targets, this);

        foreach (Square s in targets)
            if (s.Piece != null)
                s.Piece.NotifyStat(Stat, Value, Duration, Tick);
    }
}
