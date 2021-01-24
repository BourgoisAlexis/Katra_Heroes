using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class OneUseButton : Clickable
{
    #region Variables
    [SerializeField] private UnityEvent _event;

    private bool done;
    private Image visual;
    protected TextMeshProUGUI text;

    private Color textColor;
    private Color visualColor;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        visual = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        visualColor = visual.color;
        textColor = text.color;
    }


    public override void Onclick()
    {
        base.Onclick();

        if (!done)
        {
            Do();
        }
    }

    protected virtual void Do()
    {
        visual.color = textColor;
        text.color = visualColor;

        _event?.Invoke();

        done = true;
    }
}
