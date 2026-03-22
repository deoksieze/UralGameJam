using UnityEngine;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IDropHandler
{
    public DraggableItem currentItem;

    [Header("Slot config")]
    public bool isHotbarSlot;
    public HotbarController hotbar;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        var draggable = draggedObj.GetComponent<DraggableItem>();
        if (draggable == null) return;

        var itemData = draggable.data;
        if (itemData == null) return;

        // 1. ЛОКАЛЬНЫЙ ПРОВЕРКИ СЛОТА

        // Уже есть логически занятый предмет
        if (currentItem != null)
            return;

        // Уже есть хоть какой-то визуальный ребёнок (иконка)
        if (transform.childCount > 0)
            return;

        // 2. ПРОВЕРКА ЛИМИТОВ ХОТБАРА, НЕ МЕНЯЯ НИЧЕГО

        if (isHotbarSlot && hotbar != null)
        {
            if (!hotbar.CanPlace(itemData))
                return;
        }

        // 3. СНЯТЬ ПРЕДМЕТ СО СТАРОГО СЛОТА

        if (draggable.currentSlot != null)
        {
            UISlot oldSlot = draggable.currentSlot;

            // Если старый слот был хотбаром — вычесть из счётчиков
            if (oldSlot.isHotbarSlot && oldSlot.hotbar != null)
            {
                oldSlot.hotbar.OnItemRemoved(draggable.data);
            }

            oldSlot.currentItem = null;
        }

        // 4. ПРИВЯЗАТЬ К НОВОМУ СЛОТУ

        currentItem = draggable;
        draggable.currentSlot = this;

        draggable.transform.SetParent(transform, false);
        draggable.rectTransform.anchoredPosition = Vector2.zero;

        if (isHotbarSlot && hotbar != null)
        {
            hotbar.OnItemPlaced(itemData);
        }
    }
}
