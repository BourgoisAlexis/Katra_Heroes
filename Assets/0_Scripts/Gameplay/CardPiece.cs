using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPiece : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image artWork;
    
    private Card card;
    private Transform _transform;
    private Vector2 handPosition;
    private int index;

    //Accessors
    public Card Card => card;
    #endregion


    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(Card _card)
    {
        card = _card;
        card.Ability.Set();

        text.text = card.Cost.ToString();
        artWork.sprite = card.Graph;
    }


    public void ChangePosition(Vector2 _pos)
    {
        handPosition = _pos;
        ReturnInHand(0.3f);
    }

    public void ReturnInHand(float _speed)
    {
        _transform.DOLocalMove(handPosition, _speed);
    }
}
