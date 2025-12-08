using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    [Header("Prefabs & References")]
    public GameObject fishPrefab;
    public Transform fishContainer;
    public HookController hook;
    public FishingUIManager ui;
    public Transform spawnArea;
    public List<FishData> allFishData;
    
    // Helper for UI transition
    public FishInstance lastCaughtFish; 

    [Header("Upgrades (Loaded from GameManager)")]
    public float magnetBonusPercent = 0f;
    public float ragetailBonusPercent = 0f;
    public float rageBaitBonusPercent = 0f;

    [Header("Amount of Fish")]
    public float amountFish = 0f;
    
    public bool baitEquipped = false;
    public bool magnetEquipped = false;
    public bool rageEquipped = false;
    public bool trapperEquipped = false;
    public bool spinnerEquipped = false;
    public bool barbedHookEquipped = false;

    private List<FishMovement> activeFish = new List<FishMovement>();
    
    // --- UPDATED: LIST FOR BARBED HOOK ---
    private List<FishMovement> currentCaughtFishes = new List<FishMovement>();
    
    private PullMinigame pullMinigame;
    private Rect swimRect;

    // Item Names must match ShopItemData DisplayNames exactly
    const string ITEM_BAIT = "Bait";
    const string ITEM_MAGNET = "Magnet";
    const string ITEM_RAGE = "Rage Bait"; 
    const string ITEM_TRAPPER = "Trapper";
    const string ITEM_SPINNER = "Spinner";
    const string ITEM_HOOK = "Barbed Hook";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1. LOAD UPGRADES & CHECK COUNTS
        if (GameManager.Instance != null)
        {
            // Check if we have stock (> 0) to equip the item
            baitEquipped = GameManager.Instance.GetItemCount(ITEM_BAIT) > 0;
            magnetEquipped = GameManager.Instance.GetItemCount(ITEM_MAGNET) > 0;
            rageEquipped = GameManager.Instance.GetItemCount(ITEM_RAGE) > 0;
            trapperEquipped = GameManager.Instance.GetItemCount(ITEM_TRAPPER) > 0;
            spinnerEquipped = GameManager.Instance.GetItemCount(ITEM_SPINNER) > 0;
            barbedHookEquipped = GameManager.Instance.GetItemCount(ITEM_HOOK) > 0;

            // Apply Stats
            if (magnetEquipped) magnetBonusPercent = 15f;
            if (rageEquipped) rageBaitBonusPercent = 25f; 
            if (rageEquipped) ragetailBonusPercent = 10f; 
        }

        // 2. SET HOOK CAPACITY
        if (barbedHookEquipped) hook.capacity = 3; 
        else hook.capacity = 1;

        // 3. SETUP SPAWN AREA
        var col = spawnArea.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Vector3 size = col.size;
            Vector3 pos = spawnArea.position;
            swimRect = new Rect(pos.x - size.x / 2f, pos.y - size.y / 2f, size.x, size.y);
        }

        // 4. SPAWN FISH
        for (int i = 0; i < amountFish; i++) SpawnRandomFish();

        ui.ShowPrompt("Hold F or Left Click to Cast");

        // 5. REGISTER EVENTS
        hook.OnFishCaught += OnFishCaught;
        hook.OnHookReturned += OnHookReturned;
        hook.OnFishLanded += OnFishLanded;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && hook.state == HookController.HookState.Idle)
        {
            ui.HidePrompt();
            hook.StartDrop();
        }
    }

    // --- HANDLE CATCHING MULTIPLE FISH ---
    private void OnFishCaught(List<FishMovement> fishes)
    {
        // Copy the list from the hook
        currentCaughtFishes = new List<FishMovement>(fishes); 

        // Lock visual position
        foreach(var f in currentCaughtFishes)
        {
            f.LockToHook(hook.transform);
        }

        pullMinigame = gameObject.GetComponent<PullMinigame>();
        if (pullMinigame == null) pullMinigame = gameObject.AddComponent<PullMinigame>();

        // Calculate difficulty based on the TOUGHEST fish in the group
        float maxDuration = 0f;
        foreach(var f in currentCaughtFishes)
        {
            float d = GetStruggleDurationByRarity(f.data.rarity);
            if(d > maxDuration) maxDuration = d;
        }

        pullMinigame.StartMinigame(
            hook,
            allowHoldMode: false,
            power: 1f,
            successCallback: OnPullSuccess,
            failCallback: OnPullFail,
            struggleDuration: maxDuration
        );
    }

    private float GetStruggleDurationByRarity(FishData.Rarity rarity)
    {
        switch (rarity)
        {
            case FishData.Rarity.Common: return 5f;
            case FishData.Rarity.Uncommon: return 4f;
            case FishData.Rarity.Rare: return 3f;
            case FishData.Rarity.UltraRare: return 2f;
        }
        return 5f;
    }

    private void OnPullSuccess()
    {
        if (currentCaughtFishes.Count == 0) return;

        // 1. Pick the first fish to show on the summary screen (UI)
        float firstWeight = Random.Range(currentCaughtFishes[0].data.minKg, currentCaughtFishes[0].data.maxKg);
        FishInstance displayFish = new FishInstance(currentCaughtFishes[0].data, firstWeight);
        LastCaughtFish.fish = displayFish; 

        if (GameManager.Instance != null)
        {
            // 2. Add ALL caught fish to inventory
            foreach(var fishMove in currentCaughtFishes)
            {
                float weight = Random.Range(fishMove.data.minKg, fishMove.data.maxKg);
                FishInstance fi = new FishInstance(fishMove.data, weight);
                GameManager.Instance.playerInventory.AddFish(fi);
                
                // Destroy scene objects
                activeFish.Remove(fishMove);
                Destroy(fishMove.gameObject, 0.1f);
            }

            // 3. Consume Used Items (Subtract 1 from inventory)
            if (baitEquipped) GameManager.Instance.ConsumeItem(ITEM_BAIT);
            if (magnetEquipped) GameManager.Instance.ConsumeItem(ITEM_MAGNET);
            if (rageEquipped) GameManager.Instance.ConsumeItem(ITEM_RAGE);
            if (trapperEquipped) GameManager.Instance.ConsumeItem(ITEM_TRAPPER);
            if (spinnerEquipped) GameManager.Instance.ConsumeItem(ITEM_SPINNER);
            if (barbedHookEquipped) GameManager.Instance.ConsumeItem(ITEM_HOOK);
        }

        currentCaughtFishes.Clear();
        CleanupMinigame();
        SceneManager.LoadScene("Main");
    }

    private void OnPullFail()
    {
        if (currentCaughtFishes.Count > 0)
        {
            ui.ShowEscape();

            if (!trapperEquipped)
            {
                // Release all fish back to the water
                foreach(var f in currentCaughtFishes)
                {
                    f.StartReturnToSwim();
                    activeFish.Remove(f);
                }
            }
            currentCaughtFishes.Clear();
        }

        hook.StartReturn();
    }

    private void CleanupMinigame()
    {
        if (pullMinigame != null) pullMinigame = null;
    }

    private void OnHookReturned()
    {
        ui.ShowPrompt("Hold F or Left Click to Cast");
    }

    public void SpawnRandomFish()
    {
        if (allFishData.Count == 0) return;

        FishData pick = null;

        // BAIT LOGIC (Reroll for better rarity)
        if (baitEquipped)
        {
            FishData option1 = allFishData[Random.Range(0, allFishData.Count)];
            FishData option2 = allFishData[Random.Range(0, allFishData.Count)];
            pick = (option1.rarity > option2.rarity) ? option1 : option2;
        }
        else
        {
            pick = allFishData[Random.Range(0, allFishData.Count)];
        }

        // RAGE LOGIC (Heavier Fish)
        float weight = Random.Range(pick.minKg, pick.maxKg) * (1f + ragetailBonusPercent / 100f);

        GameObject go = Instantiate(fishPrefab, fishContainer);
        Vector3 pos = new Vector3(Random.Range(swimRect.xMin, swimRect.xMax), Random.Range(swimRect.yMin, swimRect.yMax), 0f);
        go.transform.position = pos;

        var fm = go.GetComponent<FishMovement>();
        fm.Initialize(pick, weight, swimRect);
        activeFish.Add(fm);
    }

    private void OnFishLanded()
    {
        if (currentCaughtFishes.Count > 0)
        {
            OnPullSuccess();
        }
    }
}