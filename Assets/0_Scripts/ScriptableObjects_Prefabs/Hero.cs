using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewHero", menuName = "CUSTOM/Hero")]
public class Hero : ScriptableObject
{
    #region Variables
    [Header("Menus")]
    public bool Done;
    public int Index;
    
    [Header("Ingame")]
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
    public AbilityPosition Move;
    public AbilityStats Attack;
    public Ability Active;
    public Ability Passive;
    #endregion




    private void Awake()
    {
        if (!Done)
        {
            Index = Directory.GetFiles("Assets/Prefabs/Heroes").Length / 2;
            Name = "UNKNOWN";
            Done = true;
        }
    }
}
