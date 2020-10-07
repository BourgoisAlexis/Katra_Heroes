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
    public int Index => index;
    #endregion


    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(Card _card, int _index)
    {
        card = _card;
        index = _index;
        card.Ability.Set();

        text.text = card.Cost.ToString();
        artWork.sprite = card.Graph;
    }


    public void ChangePosition(Vector2 _pos)
    {
        handPosition = _pos;
        ReturnInHand();
    }

    public void ReturnInHand()
    {
        _transform.localPosition = handPosition;
    }
}
