public class Stat
{
    #region Variables
    private int currentValue;
    private int baseValue;

    private int buffDuration;
    private int tickValue;
    private int tick;
    private e_stats key;

    //Accessors
    public int CurrentValue => currentValue;
    #endregion


    public Stat(e_stats _key, int _value)
    {
        key = _key;
        baseValue = _value;
        currentValue = _value;
        buffDuration = 0;
    }


    public void UpdateValue(int _value, int _duration, int _tick)
    {
        if (_tick > 0)
        {
            tick = _tick;
            tickValue = _value;
        }

        if (key != e_stats.Health && _duration > 0)
        {
            currentValue = baseValue + _value;
            buffDuration = _duration;
        }
        else
        {
            currentValue += _value;

            if (key == e_stats.Health && currentValue > baseValue)
                currentValue = baseValue;
        }
    }

    public void Decrease(HeroPiece _piece)
    {
        if (tick > 0)
        {
            tick --;
            _piece.ModifyStat(key, tickValue, buffDuration, tick);

            return;
        }

        if (buffDuration > 0)
        {
            buffDuration--;
        }

        if (key != e_stats.Health && tick == 0 && buffDuration == 0)
        {
            currentValue = baseValue;
        }
    }
}
