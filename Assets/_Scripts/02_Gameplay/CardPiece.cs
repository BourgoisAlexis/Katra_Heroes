using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPiece : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image artWork;
    
    private Card card;
    private Transform _transform;
    private Vector2 handPosition;

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

        foreach (Effect ef in card.Effects)
            ef.Set();

        cost.text = card.Cost.ToString();
        _name.text = card.Name;
        description.text = GenerateDescription();
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


    private string GenerateDescription()
    {
        string desc = string.Empty;
        string colorID = string.Empty;

        foreach (Effect ef in card.Effects)
        {
            desc += "- ";

            EffectStat st = ef as EffectStat;
            EffectDraw dr = ef as EffectDraw;

            if (st != null)
            {

                colorID = ColorUtility.ToHtmlStringRGB(GameplayManager.Instance.Data.GetColor(st.Stat));

                desc += "<color=#" + colorID + ">";
                desc += st.Value > 0 ? "+" : "-";
                desc += Mathf.Abs(st.Value);
                desc += "</color>";
                desc += " ";
                desc += st.Stat.ToString();
                desc += " ";

                e_rangeType rangeType = card.RangeType;
                e_targetting targetType = card.Targetting;

                if (rangeType == e_rangeType.Allies)
                {
                    if (targetType == e_targetting.AutomaticTarget)
                        desc += "all allies";
                    else if (targetType == e_targetting.Default)
                        desc += "targeted ally";
                }
                else if (rangeType == e_rangeType.Ennemies)
                {
                    if (targetType == e_targetting.AutomaticTarget)
                        desc += "all ennemies";
                    else if (targetType == e_targetting.Default)
                        desc += "targeted ennemy";
                }

                desc += " ";

                if (st.Duration > 1)
                {
                    desc += "for " + st.Duration + " turns";
                }

                if (st.Tick > 1)
                {
                    desc += "per turn for " + st.Tick + " turns";
                }
            }

            else if (dr != null)
            {
                desc += "Draw " + dr.Value + " card";

                if (dr.Value > 1)
                    desc += "s";
            }

            desc += "\n";
        }

        return desc;
    }
}
