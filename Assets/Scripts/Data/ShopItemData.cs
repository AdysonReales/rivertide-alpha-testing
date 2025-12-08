using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/ShopItem")]
public class ShopItemData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    [Tooltip("Cost in g")]
    public int cost;
    [TextArea] public string description;
    public enum Effect { Bait, Magnet, RageBait, Trapper, Spinner, BarbedHook }
    public Effect effect;
}
