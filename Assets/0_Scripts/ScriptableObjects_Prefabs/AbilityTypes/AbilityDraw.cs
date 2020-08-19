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
        abilityType = e_abilityType.Draw;
    }

    public override void Use(Square _target)
    {
        for (int i = 0; i < Value; i++)
            GameplayManager.Instance.Deck.DrawCard();
    }
}
