using UnityEngine;

public class BuffFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosion;

    public void ChangeColor(e_stats _key)
    {
        ParticleSystem.MainModule main = explosion.main;
        main.startColor = GameplayManager.Instance.Data.GetColor(_key);
    }
}
