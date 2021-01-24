using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CUSTOM/Card")]
public class Card : Ability
{
    #region Variables
    [Header("Properties")]
    public int Index;
    public string Name;
    public Sprite Graph;
    #endregion
}
