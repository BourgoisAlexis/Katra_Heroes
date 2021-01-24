using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "CUSTOM/Hero")]
public class Hero : ScriptableObject
{
    #region Variables
    [Header("Properties")]
    public int Index;
    public string Name;
    public Sprite Graph;

    [Header("Stats")]
    public int Health;
    public int Speed;
    public int Damage;
    public int Range;
    public int Critic;
    public bool Fly;
    public e_species Species;
    public e_unitType UnitType;

    [Header("Abilities")]
    public Ability Move;
    public Ability Attack;
    public Ability Active;
    public Ability Passive;
    #endregion
}
