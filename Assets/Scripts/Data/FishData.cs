using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite sprite;
    public float minKg;
    public float maxKg;
    public int price; // in g
    [Range(0f,100f)] public float baseChancePercent; // e.g. 28 for 28%
    [TextArea] public string description;
    public string tagline;
    public enum Rarity { Common, Uncommon, Rare, UltraRare }
    public Rarity rarity;
    public float baseSpeed = 1f; 
    public float sellRate = 10f;
}
