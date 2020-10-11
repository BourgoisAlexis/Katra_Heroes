public class Stat
{
    #region Variables
    private bool canBeBuffed;
    private int baseValue;
    private int currentValue;
    private int buffDuration;

    //Accessors
    public bool CanBeBuffed => canBeBuffed;
    public int BaseValue => baseValue;
    public int CurrentValue => currentValue;
    public int BuffDuration => buffDuration;
    #endregion


    public Stat(int _value, bool _buff)
    {
        baseValue = _value;
        currentValue = _value;
        canBeBuffed = _buff;
        buffDuration = 0;
    }


    //For buffable values
    public void Buff(int _value, int _duration)
    {
        currentValue = baseValue + _value;
        buffDuration = _duration;
    }

    public void Decrease()
    {
        if (canBeBuffed && buffDuration > 0)
        {
            buffDuration--;
            if (buffDuration == 0)
                currentValue = baseValue;
        }
    }

    //For non buffable values
    public void UpdateValue(int _value)
    {
        currentValue += _value;
    }
}
