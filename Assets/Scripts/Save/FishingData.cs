using UnityEngine;

public static class FishingData
{
    // Stores player last position before entering fishing scene
    public static Vector3 playerLastPosition;
    public static bool hasPosition = false;

    // Stores the last caught fish
    public static FishInstance lastCaughtFish;
}
