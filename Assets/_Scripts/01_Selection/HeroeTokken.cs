using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroeTokken : MonoBehaviour
{
    #region Variables
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    private Transform _transform;

    private bool selected;
    private Image backGround;
    private Image visual;
    private int index;

    //Accessors
    public int Index => index;
    #endregion


    public void Init(Data _datas, int _index)
    {
        _transform = transform;
        Image[] images = GetComponentsInChildren<Image>();
        backGround = images[0];
        visual = images[1];

        index = _index;
        visual.sprite = _datas.Heroes[index].Graph;
        backGround.color = unselectedColor;
    }

    public bool OnClick()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_transform.DOScale(0.9f, 0.05f));
        seq.Append(_transform.DOScale(1f, 0.1f));

        selected = !selected;
        backGround.color = selected ? selectedColor : unselectedColor;

        return selected;
    }
}
