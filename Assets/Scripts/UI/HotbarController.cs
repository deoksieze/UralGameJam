using UnityEngine;

public class HotbarController : MonoBehaviour
{
    public int maxTraits = 2;
    public int maxAbilities = 1;

    int currentTraits;
    int currentAbilities;

    public bool CanPlace(SlotItemData item)
    {
        if (item == null) return false;

        switch (item.type)
        {
            case SlotItemType.Trait:
                return currentTraits < maxTraits;
            case SlotItemType.Ability:
                return currentAbilities < maxAbilities;
            default:
                return false;
        }
    }

    public void OnItemPlaced(SlotItemData item)
    {
        if (item == null) return;

        switch (item.type)
        {
            case SlotItemType.Trait:
                currentTraits++;
                break;
            case SlotItemType.Ability:
                currentAbilities++;
                break;
        }
    }

    public void OnItemRemoved(SlotItemData item)
    {
        if (item == null) return;

        switch (item.type)
        {
            case SlotItemType.Trait:
                currentTraits = Mathf.Max(0, currentTraits - 1);
                break;
            case SlotItemType.Ability:
                currentAbilities = Mathf.Max(0, currentAbilities - 1);
                break;
        }
    }
}
