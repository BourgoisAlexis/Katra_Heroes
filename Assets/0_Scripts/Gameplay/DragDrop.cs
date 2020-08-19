using UnityEngine;

public class DragDrop : MonoBehaviour
{
    #region Variables
    [SerializeField]
    protected bool uiDragDrop;

    protected Vector3 initialPos;
    protected Transform toDrag;
    protected bool dragging;
    protected Camera mainCamera;

    //Accessors
    public bool Dragging => dragging;
    public Transform ToDrag => toDrag;
    #endregion


    protected virtual void Awake()
    {
        mainCamera = Camera.main;
    }


    public virtual void ClickDrag(Transform _toDrag)
    {
        toDrag = _toDrag;
        initialPos = toDrag.position;
    }

    public virtual void Drag()
    {
        Vector2 pos = uiDragDrop ? Input.mousePosition : mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;

        if (Vector2.Distance(initialPos, pos) <= (uiDragDrop ? 100 : 0.3f))
        {
            dragging = false;
            toDrag.position = initialPos;
        }
        else
            toDrag.position = new Vector3(pos.x, pos.y, initialPos.z - 2);
    }

    public virtual void DragEnd()
    {
        toDrag = null;
        dragging = false;
    } 
}
