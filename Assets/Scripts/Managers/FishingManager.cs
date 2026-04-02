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
    
    public FishInstance lastCaughtFish; 

    [Header("Upgrades (Loaded from GameManager)")]
    public float magnetBonusPercent = 0f;
    public float ragetailBonusPercent = 0f;
    public float rageBaitBonusPercent = 0f;

    [Header("Amount of Fish")]
    public int amountFish = 5; // Changed to int for the loop
    
    public bool baitEquipped = false;
    public bool magnetEquipped = false;
    public bool rageEquipped = false;
    public bool trapperEquipped = false;
    public bool spinnerEquipped = false;
    public bool barbedHookEquipped = false;

    private List<FishMovement> activeFish = new List<FishMovement>();
    private List<FishMovement> currentCaughtFishes = new List<FishMovement>();
    
    private PullMinigame pullMinigame;
    private Rect swimRect;
    private bool is3D = false; // Internal flag to handle 3D positioning

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
        if (GameManager.Instance != null)
        {
            baitEquipped = GameManager.Instance.GetItemCount(ITEM_BAIT) > 0;
            magnetEquipped = GameManager.Instance.GetItemCount(ITEM_MAGNET) > 0;
            rageEquipped = GameManager.Instance.GetItemCount(ITEM_RAGE) > 0;
            trapperEquipped = GameManager.Instance.GetItemCount(ITEM_TRAPPER) > 0;
            spinnerEquipped = GameManager.Instance.GetItemCount(ITEM_SPINNER) > 0;
            barbedHookEquipped = GameManager.Instance.GetItemCount(ITEM_HOOK) > 0;

            if (magnetEquipped) magnetBonusPercent = 15f;
            if (rageEquipped) { rageBaitBonusPercent = 25f; ragetailBonusPercent = 10f; }
        }

        if (hook != null)
        {
            if (barbedHookEquipped) hook.capacity = 3; 
            else hook.capacity = 1;
        }

        // --- HYBRID 2D/3D SPAWN AREA SETUP ---
        if (spawnArea != null)
        {
            // Try 3D first
            var col3D = spawnArea.GetComponent<BoxCollider>();
            var col2D = spawnArea.GetComponent<BoxCollider2D>();

            if (col3D != null)
            {
                is3D = true;
                Vector3 size = col3D.size;
                Vector3 pos = spawnArea.position;
                // In 3D, we usually use X and Z for the area (top down view)
                swimRect = new Rect(pos.x - size.x / 2f, pos.z - size.z / 2f, size.x, size.z);
            }
            else if (col2D != null)
            {
                is3D = false;
                Vector3 size = col2D.size;
                Vector3 pos = spawnArea.position;
                swimRect = new Rect(pos.x - size.x / 2f, pos.y - size.y / 2f, size.x, size.y);
            }
            else
            {
                Debug.LogError("FishingManager: SpawnArea needs a BoxCollider (3D) or BoxCollider2D!");
            }
        }

        // SPAWN FISH (Safely)
        for (int i = 0; i < amountFish; i++) SpawnRandomFish();

        if (ui != null) ui.ShowPrompt("Hold F or Left Click to Cast");

        if (hook != null)
        {
            hook.OnFishCaught += OnFishCaught;
            hook.OnHookReturned += OnHookReturned;
            hook.OnFishLanded += OnFishLanded;
        }
    }

    private void Update()
    {
        if (hook == null || ui == null) return;

        if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && hook.state == HookController.HookState.Idle)
        {
            ui.HidePrompt();
            hook.StartDrop();
        }
    }

    private void OnFishCaught(List<FishMovement> fishes)
    {
        currentCaughtFishes = new List<FishMovement>(fishes); 

        foreach(var f in currentCaughtFishes)
        {
            f.LockToHook(hook.transform);
        }

        pullMinigame = gameObject.GetComponent<PullMinigame>();
        if (pullMinigame == null) pullMinigame = gameObject.AddComponent<PullMinigame>();

        float maxDuration = 0f;
        foreach(var f in currentCaughtFishes)
        {
            float d = GetStruggleDurationByRarity(f.data.rarity);
            if(d > maxDuration) maxDuration = d;
        }

        pullMinigame.StartMinigame(hook, false, 1f, OnPullSuccess, OnPullFail, maxDuration);
    }

    private float GetStruggleDurationByRarity(FishData.Rarity rarity)
    {
        switch (rarity)
        {
            case FishData.Rarity.Common: return 5f;
            case FishData.Rarity.Uncommon: return 4f;
            case FishData.Rarity.Rare: return 3f;
            case FishData.Rarity.UltraRare: return 2f;
            default: return 5f;
        }
    }

    private void OnPullSuccess()
    {
        if (currentCaughtFishes.Count == 0) return;

        float firstWeight = Random.Range(currentCaughtFishes[0].data.minKg, currentCaughtFishes[0].data.maxKg);
        FishInstance displayFish = new FishInstance(currentCaughtFishes[0].data, firstWeight);
        LastCaughtFish.fish = displayFish; 

        if (GameManager.Instance != null)
        {
            foreach(var fishMove in currentCaughtFishes)
            {
                float weight = Random.Range(fishMove.data.minKg, fishMove.data.maxKg);
                FishInstance fi = new FishInstance(fishMove.data, weight);
                GameManager.Instance.playerInventory.AddFish(fi);
                
                activeFish.Remove(fishMove);
                Destroy(fishMove.gameObject, 0.1f);
            }

            if (baitEquipped) GameManager.Instance.ConsumeItem(ITEM_BAIT);
            if (magnetEquipped) GameManager.Instance.ConsumeItem(ITEM_MAGNET);
            if (rageEquipped) GameManager.Instance.ConsumeItem(ITEM_RAGE);
            if (trapperEquipped) GameManager.Instance.ConsumeItem(ITEM_TRAPPER);
            if (spinnerEquipped) GameManager.Instance.ConsumeItem(ITEM_SPINNER);
            if (barbedHookEquipped) GameManager.Instance.ConsumeItem(ITEM_HOOK);
        }

        currentCaughtFishes.Clear();
        CleanupMinigame();
        SceneManager.LoadScene("3DMain");
    }

    private void OnPullFail()
    {
        if (currentCaughtFishes.Count > 0)
        {
            if (ui != null) ui.ShowEscape();

            if (!trapperEquipped)
            {
                foreach(var f in currentCaughtFishes)
                {
                    f.StartReturnToSwim();
                    activeFish.Remove(f);
                }
            }
            currentCaughtFishes.Clear();
        }
        if (hook != null) hook.StartReturn();
    }

    private void CleanupMinigame()
    {
        if (pullMinigame != null) pullMinigame = null;
    }

    private void OnHookReturned()
    {
        if (ui != null) ui.ShowPrompt("Hold F or Left Click to Cast");
    }

 public void SpawnRandomFish()
{
    // 1. SAFETY CHECKS
    if (allFishData == null || allFishData.Count == 0) return;
    if (fishPrefab == null || fishContainer == null || spawnArea == null) {
        Debug.LogError("FishingManager: Check your Inspector! Prefab, Container, or SpawnArea is missing.");
        return;
    }

    // 2. PICK THE FISH DATA
    FishData pick = allFishData[Random.Range(0, allFishData.Count)];
    if (pick == null) return;

    // 3. CALCULATE THE POSITION IN LOCAL SPACE
    // We instantiate as a child FIRST, then set the local position.
    // This forces Z to be 0 relative to the parent.
    GameObject go = Instantiate(fishPrefab, fishContainer);
    
    float weight = Random.Range(pick.minKg, pick.maxKg) * (1f + ragetailBonusPercent / 100f);
    
    Vector3 localSpawnPos = Vector3.zero;

    if (is3D) {
        var col3D = spawnArea.GetComponent<BoxCollider>();
        if (col3D != null) {
            // Get a random world point inside the spawn area
            Bounds b = col3D.bounds;
            Vector3 randomWorldPoint = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                fishContainer.position.z // Start with container's world Z
            );

            // CONVERT World Point to Local Point relative to the Container
            localSpawnPos = fishContainer.InverseTransformPoint(randomWorldPoint);
            
            // FORCE Local Z to 0 (This is what you requested!)
            localSpawnPos.z = 0f; 
        }
    } else {
        // 2D Logic
        localSpawnPos = new Vector3(
            Random.Range(swimRect.xMin, swimRect.xMax) - fishContainer.position.x, 
            Random.Range(swimRect.yMin, swimRect.yMax) - fishContainer.position.y, 
            0f
        );
    }

    // 4. APPLY POSITION AND INITIALIZE
    go.transform.localPosition = localSpawnPos;

    var fm = go.GetComponent<FishMovement>();
    if (fm != null) {
        fm.Initialize(pick, weight, swimRect);
        activeFish.Add(fm);
    }
}

    private void OnFishLanded()
    {
        if (currentCaughtFishes.Count > 0) OnPullSuccess();
    }
}