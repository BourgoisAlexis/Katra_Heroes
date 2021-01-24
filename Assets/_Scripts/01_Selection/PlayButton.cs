using UnityEngine;

public class PlayButton : OneUseButton
{
    [SerializeField] private GameObject loading;

    protected override void Awake()
    {
        base.Awake();

        loading.SetActive(false);
    }

    protected override void Do()
    {
        loading.SetActive(true);

        base.Do();

        text.text = "In queue";
    }
}
