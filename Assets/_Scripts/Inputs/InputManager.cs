using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    #region Variables
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected GraphicRaycaster GRayCaster;
    [SerializeField] protected EventSystem eventSystem;

    protected PointerEventData pointerEventData;
    #endregion


    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Down();

        if (Input.GetMouseButton(0))
            Maintain();

        if (Input.GetMouseButtonUp(0))
            Up();
    }


    protected virtual void Down()
    {

    }

    protected virtual void Maintain()
    {

    }

    protected virtual void Up()
    {

    }


    protected GameObject DetectUI(Vector2 _position)
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = _position;

        List<RaycastResult> results = new List<RaycastResult>();

        GRayCaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
            return result.gameObject;

        return null;
    }

    protected GameObject DetectCollider(Vector2 _position)
    {
        Vector2 clickPosition = mainCamera.ScreenToWorldPoint(_position);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hit.collider != null)
            return hit.collider.gameObject;

        return null;
    }
}
