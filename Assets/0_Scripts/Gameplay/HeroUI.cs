using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroUI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private TextMeshProUGUI healthBarText;
    [SerializeField]
    private ActiveButton active;

    private Transform _transform;
    private Camera mainCamera;
    private HeroPiece heroPiece;
    #endregion


    private void Awake()
    {
        mainCamera = Camera.main;
        _transform = transform;
    }

    public void Setup(HeroPiece _piece)
    {
        heroPiece = _piece;
        active.Setup(heroPiece);
    }


    public void ChangeColor(Color _color)
    {
        healthBar.color = _color;
        healthBarText.color = _color;
    }

    public void UpdateVisual(Vector2 _position, int _health, int _maxHealth)
    {
        _transform.position = mainCamera.WorldToScreenPoint(_position);
        healthBar.fillAmount = (float)_health / (float)_maxHealth;
        healthBarText.text = _health.ToString();
    }

    public void ActiveUsed(bool _used)
    {
        active.Used(_used);
    }
}
