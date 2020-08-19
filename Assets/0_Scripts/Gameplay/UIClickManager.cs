using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClickManager : MonoBehaviour
{
    #region Variables
    private GraphicRaycaster GRayCaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    #endregion


    private void Awake()
    {
        GRayCaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }


    public GameObject DetectUI(Vector2 _position)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = _position;

        List<RaycastResult> results = new List<RaycastResult>();

        GRayCaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
                return result.gameObject;

        return null;
    }
}
