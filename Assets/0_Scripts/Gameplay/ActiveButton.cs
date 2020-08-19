using UnityEngine;
using System.Collections;

public class ActiveButton : MonoBehaviour
{
    #region Variables
    private HeroPiece piece;

    //Accessors
    public HeroPiece Piece => piece;
    #endregion


    public void Setup(HeroPiece _piece)
    {
        piece = _piece;
    }

    public void Used(bool _used)
    {
        StartCoroutine(Anim(_used));
    }

    private IEnumerator Anim(bool _used)
    {
        float step = 10;
        float scale = _used ? 1 : 0;
        float value = 1 / step * (_used ? -1 : 1);

        for (int i = 0; i < step; i++)
        {
            scale += value;
            transform.localScale = new Vector2(scale, scale);
            transform.Rotate(new Vector3(0, 0, 360 / step));

            yield return new WaitForSeconds(0.01f);
        }
    }
}
