using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private UIClickManager uiClickManager;
    [SerializeField] private GameObject heroeUIPrefab;
    [SerializeField] private Transform heroUIParent;
    [SerializeField] private TextMeshProUGUI mana;

    [Header("Transition")]
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI transitionText;

    private GameplayManager gameplayManager;

    //Accessors
    public UIClickManager UIClick => uiClickManager;
    #endregion


    private void Awake()
    {
        gameplayManager = GetComponent<GameplayManager>();

        UpdateMana();
    }

    public HeroUI CreateHeroUI()
    {
        HeroUI ui = Instantiate(heroeUIPrefab, heroUIParent).GetComponent<HeroUI>();
        return ui;
    }

    public void UpdateMana()
    {
        mana.text = gameplayManager.Mana.ToString();
    }

    public void Transition(e_step _step)
    {
        transitionText.text = _step.ToString();
        animator.SetTrigger("anim");
    }
}
