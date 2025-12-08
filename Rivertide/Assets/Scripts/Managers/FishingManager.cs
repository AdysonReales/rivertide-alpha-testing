using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    [Header("References")]
    public Transform hookSpawn;
    public GameObject hookPrefab;
    public Transform fishParent;
    public GameObject fishPrefab; // generic fish prefab to be given a FishData

    [Header("Spawn")]
    public float spawnInterval = 1.2f;
    private float spawnTimer = 0f;
    public List<FishData> fishPool; // assign from GameManager.Instance.allFishData

    [Header("State")]
    public bool isFishing = false;
    public HookController currentHook;

    [Header("Upgrades")]
    [Range(0f,100f)] public float magnetBonusPercent = 0f; // from shop
    [Range(0f,100f)] public float rageBaitBonusPercent = 0f;
    public bool spinnerEquipped = false;
    public bool trapperEquipped = false;
    public bool barbedHookEquipped = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // load upgrades from GameManager if needed
        if (GameManager.Instance != null) fishPool = GameManager.Instance.allFishData;
    }

    void Update()
    {
        if (!isFishing) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnFish();
            spawnTimer = spawnInterval;
        }

        // start drop with F or mouse left
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
        {
            if (currentHook == null)
            {
                var go = Instantiate(hookPrefab, hookSpawn.position, Quaternion.identity);
                currentHook = go.GetComponent<HookController>();
                currentHook.BeginDrop();
            }
        }
    }

    void SpawnFish()
    {
        // Weighted random by baseChance and applied upgrades
        float totalWeight = 0f;
        foreach(var f in fishPool)
        {
            float w = f.baseChancePercent;
            // apply magnet/rage effects by rarity
            if (f.rarity == FishData.Rarity.Rare) w *= (1f + magnetBonusPercent/100f);
            if (f.rarity == FishData.Rarity.UltraRare) w *= (1f + rageBaitBonusPercent/100f);
            totalWeight += w;
        }

        float r = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        FishData chosen = fishPool[0];
        foreach(var f in fishPool)
        {
            float w = f.baseChancePercent;
            if (f.rarity == FishData.Rarity.Rare) w *= (1f + magnetBonusPercent/100f);
            if (f.rarity == FishData.Rarity.UltraRare) w *= (1f + rageBaitBonusPercent/100f);
            cumulative += w;
            if (r <= cumulative) { chosen = f; break; }
        }

        // Instantiate a fish visual and give it data
        GameObject go = Instantiate(fishPrefab, fishParent);
        var fm = go.GetComponent<FishMovement>();
        fm.Setup(chosen, spinnerEquipped); // spinner affects swim area/wobble
    }

    public void OnFishCaught(FishInstance caught)
    {
        // Add to inventory
        GameManager.Instance.playerInventory.AddFish(caught);
        UIManager.Instance.ShowCatchPanel(caught);
        // If barbedHook allows multiple catches, stay or handle accordingly
        currentHook = null;
    }

    public void OnFishEscaped()
    {
        UIManager.Instance.ShowEscape();
        currentHook = null;
    }
}
