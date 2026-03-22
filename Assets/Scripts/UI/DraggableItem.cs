using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SlotItemData data;
    public RectTransform rectTransform;
    public Canvas canvas;
    public CanvasGroup canvasGroup;

    public UISlot currentSlot;

    Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

public void OnEndDrag(PointerEventData eventData)
{
    canvasGroup.blocksRaycasts = true;

    if (currentSlot != null)
    {
        // уже приняты слотом в OnDrop
        transform.SetParent(currentSlot.transform, false);
        rectTransform.anchoredPosition = Vector2.zero;
    }
    else
    {
        // никто не принял
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}

}
