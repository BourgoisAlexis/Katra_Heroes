using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionInputs : InputManager
{
    #region Variables
    [Header("Selection")]
    [SerializeField] private GameObject tokkenPrefab;
    [SerializeField] private Data datas;

    [SerializeField] private GameObject playPannel;
    [SerializeField] private GameObject selectionPannel;
    [SerializeField] private Transform grid;
    [SerializeField] private TextMeshProUGUI teamText;

    private List<HeroeTokken> selectedHeroes = new List<HeroeTokken>();
    #endregion


    private void Awake()
    {
        selectionPannel.SetActive(false);
    }


    public void StartSelection(e_teams _team)
    {
        playPannel.SetActive(false);
        teamText.text = _team.ToString() + " Team";
        teamText.color = datas.Colors[(int)_team + 1];
        selectionPannel.SetActive(true);

        for (int i = 0; i < datas.Heroes.Length; i++)
        {
            GameObject instance = Instantiate(tokkenPrefab, grid);
            instance.GetComponent<HeroeTokken>().Init(datas, i);
        }
    }

    protected override void Down()
    {
        GameObject ui = DetectUI(Input.mousePosition);
        GameObject obj = DetectCollider(Input.mousePosition);

        if (ui != null)
        {
            HeroeTokken tokken = ui.GetComponent<HeroeTokken>();

            if (tokken != null)
            {
                if (tokken?.OnClick() == true)
                {
                    if (selectedHeroes.Count >= 3)
                    {
                        selectedHeroes[0].OnClick();
                        selectedHeroes.RemoveAt(0);
                    }

                    selectedHeroes.Add(tokken);
                }
                else
                {
                    selectedHeroes.Remove(tokken);
                }
            }
            else
                ui.GetComponent<Clickable>()?.Onclick();
        }
    }

    public void Ready()
    {
        if (selectedHeroes.Count > 0)
        {
            int[] heroes = new int[] { -1, -1, -1 };

            for (int i = 0; i < selectedHeroes.Count; i++)
            {
                heroes[i] = selectedHeroes[i].Index;
            }

            GetComponent<OnlineManager>().ValidTeam(heroes);
        }
    }
}
