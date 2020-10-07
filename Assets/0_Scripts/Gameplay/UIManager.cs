using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject heroeUIPrefab;
    [SerializeField] private Transform uiParent;
    [SerializeField] private TextMeshProUGUI mana;

    private UIClickManager uiClickManager;
    private GameplayManager gameplayManager;

    //Accessors
    public UIClickManager UIClick => uiClickManager;
    #endregion


    private void Awake()
    {
        uiClickManager = uiParent.GetComponent<UIClickManager>();
        gameplayManager = GetComponent<GameplayManager>();

        UpdateMana();
    }

    public HeroUI CreateHeroUI()
    {
        HeroUI ui = Instantiate(heroeUIPrefab, uiParent).GetComponent<HeroUI>();
        return ui;
    }

    public void UpdateMana()
    {
        mana.text = gameplayManager.Mana.ToString();
    }
}
