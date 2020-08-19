using UnityEngine;

public class CardZone : MonoBehaviour
{
    [SerializeField]
    private GameObject visual;

    private void Awake()
    {
        visual.SetActive(false);
    }

    public void ShowZone(bool _show)
    {
        visual.SetActive(_show);
    }
}
