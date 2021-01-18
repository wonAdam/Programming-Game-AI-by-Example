using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof(Collider2D))]
public class DragMoveHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} Dragging");
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, 0f);
        GetComponent<MovingEntity>().SetOffsetFromLeader();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
