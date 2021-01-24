using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActiveButton : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI text;

    private HeroPiece piece;

    //Accessors
    public HeroPiece Piece => piece;
    #endregion


    public void Setup(HeroPiece _piece)
    {
        piece = _piece;
        transform.localScale = Vector3.zero;

        text.text = piece.Hero.Active.Cost.ToString();
        text.transform.localScale = Vector3.zero;
    }

    public bool CheckMana()
    {
        if (piece.Hero.Active.Cost <= GameplayManager.Instance.Mana)
            return true;

        return false;
    }

    public void Used(bool _used)
    {
        float scale = _used ? 0 : 1;
        transform.DOScale(scale, 0.2f);
        text.transform.DOScale(scale, 0.2f);

        float rotate = _used ? 0 : 270;
        transform.DORotate(new Vector3(0, 0, rotate), 0.2f);
    }
}
