using System;
using UnityEngine;

[Serializable]
public class StatColor
{
    public e_stats Stat;
    public Color Color;
}

[CreateAssetMenu(fileName = "Data", menuName = "CUSTOM/Data")]
public class Data : ScriptableObject
{
    public Color[] Colors;
    public StatColor[] StatColors;
    public Hero[] Heroes;
    public Card[] Cards;

    public Color GetColor(e_stats _key)
    {
        foreach (StatColor c in StatColors)
            if (c.Stat == _key)
                return c.Color;

        return Color.magenta;
    }
}
