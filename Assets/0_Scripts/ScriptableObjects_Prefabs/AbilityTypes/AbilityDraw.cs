using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "CUSTOM/Ability/Draw")]
public class AbilityDraw : Ability
{
    #region Variables
    [Header("Draw")]
    public int Value;
    #endregion


    public override void Set()
    {
        targetting = e_targetting.None;
    }

    public override void Use(Square[] _targets, HeroPiece _user)
    {
        for (int i = 0; i < Value; i++)
            GameplayManager.Instance.DeckManager.DrawCard();
    }
}
