using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject heroeUIPrefab;
    [SerializeField] private Transform heroUIParent;
    [SerializeField] private TextMeshProUGUI mana;

    [Header("Start")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject readyButton;

    [Header("Transition")]
    [SerializeField] private PlayAnim transition;
    [SerializeField] private TextMeshProUGUI transitionText;

    [Header("End")]
    [SerializeField] private PlayAnim endScreen;
    [SerializeField] private TextMeshProUGUI endScreenText;
    [SerializeField] private Image logo;

    private GameplayManager gameplayManager;
    #endregion


    private void Awake()
    {
        gameplayManager = GetComponent<GameplayManager>();

        endScreen.gameObject.SetActive(false);
        nextButton.SetActive(false);
    }


    public void GameStart()
    {
        nextButton.SetActive(true);
        readyButton.SetActive(false);
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
        transition.Anim();
    }

    public void GameEnd(bool _victory)
    {
        Color c = gameplayManager.Data.Colors[_victory ? 2 : 3];
        c.a = 0.8f;
        logo.color = c;
        endScreen.gameObject.SetActive(true);
        endScreenText.text = _victory ? "victory" : "defeat";
        endScreen.Anim();
    }
}
