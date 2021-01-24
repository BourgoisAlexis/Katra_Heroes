using DG.Tweening;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    protected Transform _transform;

    protected virtual void Awake()
    {
        _transform = transform;
    }

    public virtual void Onclick()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_transform.DOScale(0.9f, 0.05f));
        seq.Append(_transform.DOScale(1f, 0.1f));
    }
}
