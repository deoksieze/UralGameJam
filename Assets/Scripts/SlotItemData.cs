using UnityEngine;

public enum SlotItemType { Ability, Trait }

[System.Serializable]
public class SlotItemData
{
    public string id;
    public string displayName;
    public Sprite icon;
    public SlotItemType type;
}
