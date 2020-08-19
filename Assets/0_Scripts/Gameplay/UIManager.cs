using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject heroeUIPrefab;
    [SerializeField]
    private Transform uiParent;

    private UIClickManager uiClickManager;

    //Accessors
    public UIClickManager UIClick => uiClickManager;
    #endregion


    private void Awake()
    {
        uiClickManager = uiParent.GetComponent<UIClickManager>();
    }

    public HeroUI CreateHeroUI()
    {
        HeroUI ui = Instantiate(heroeUIPrefab, uiParent).GetComponent<HeroUI>();
        
        return ui;
    }
}
