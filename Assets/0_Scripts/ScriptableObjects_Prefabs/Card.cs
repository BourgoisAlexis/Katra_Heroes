using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "NewCard", menuName = "CUSTOM/Card")]
public class Card : ScriptableObject
{
    #region Variables
    [Header("Menus")]
    public bool Done;
    public int Index;
    
    [Header("Ingame")]
    public string Name;
    public Sprite Graph;
    
    [Header("Stats")]
    public int Cost;
    public Ability Ability;
    #endregion


    private void Awake()
    {
        if (!Done)
        {
            Index = Directory.GetFiles("Assets/Prefabs/Cards").Length / 2;
            Name = "UNKNOWN";
            Done = true;
        }
    }
}
