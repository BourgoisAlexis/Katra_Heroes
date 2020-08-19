using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CUSTOM/Data")]
public class Data : ScriptableObject
{
    public Color[] Colors;
    public Hero[] Heroes;
    public Card[] Cards;
}
