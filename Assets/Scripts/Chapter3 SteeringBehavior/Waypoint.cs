using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Waypoint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Path path;
    public LineRenderer line1;
    public LineRenderer line2;
    private void OnEnable()
    {
        path = FindObjectOfType<Path>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} Dragging");
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position= new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, 0f);

        if (line1)
            line1.SetPosition(1, transform.position);
        if (line2)
            line2.SetPosition(0, transform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
