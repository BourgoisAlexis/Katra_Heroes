using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneTransi : MonoBehaviour
{
    public static SceneTransi Instance;

    #region Variables
    [SerializeField] private GameObject frameParent;
    [SerializeField] private GameObject logo;

    private List<Transform> frames = new List<Transform>();
    public delegate void ToExecute();
    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(transform.parent.gameObject);

        foreach(Image i in frameParent.GetComponentsInChildren<Image>())
        {
            frames.Add(i.transform);
        }
    }


    public void Transi(bool _in, ToExecute _exe)
    {
        if (_in)
            StartCoroutine(TransiIn(_exe));
        else
            StartCoroutine(TransiOut(_exe));
    }

    private IEnumerator TransiIn(ToExecute _exe)
    {
        List<Transform> list = new List<Transform>(frames);

        while (list.Count > 0)
        {
            int rn = Random.Range(0, list.Count);

            list[rn].DOLocalMoveX(0, 0.08f);
            list.RemoveAt(rn);

            yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));
        }

        yield return new WaitForSeconds(0.2f);
        logo.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        if (_exe != null)
            _exe.Invoke();
    }

    private IEnumerator TransiOut(ToExecute _exe)
    {
        List<Transform> list = new List<Transform>(frames);

        yield return new WaitForSeconds(0.5f);
        logo.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        while (list.Count > 0)
        {
            int rn = Random.Range(0, list.Count);

            list[rn].DOLocalMoveX(-2050, 0.08f);
            list.RemoveAt(rn);

            yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));
        }

        if (_exe != null)
            _exe.Invoke();
    }
}
