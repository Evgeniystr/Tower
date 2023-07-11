using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableTrigger : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler

{
    public Action OnPointerEnterEvent;
    public Action OnPointerExitEvent;
    public Action OnClickEvent;
    public Action OnPointerDownEvent;
    public Action OnPointerUpEvent;
    public Action<PointerEventData> OnDragEvent;
    public Action OnBeginDragEvent;
    public Action OnEndDragEvent;
    public Action OnDropEvent;


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke();
    }
    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragEvent?.Invoke();
    }
    public void OnDrop(PointerEventData eventData)
    {
        OnDropEvent?.Invoke();
    }
}
