using UnityEngine;
using System;

[Serializable]
public class EffectDraw : Effect
{
    #region Variables
    [Header("Draw")]
    public int Value;
    #endregion


    public override void Use(Square[] _targets, HeroPiece _user)
    {
        for (int i = 0; i < Value; i++)
            GameplayManager.Instance.DeckManager.DrawCard();
    }
}
