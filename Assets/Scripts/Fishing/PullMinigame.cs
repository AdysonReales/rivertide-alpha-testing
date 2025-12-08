using UnityEngine;
using System;

public class PullMinigame : MonoBehaviour
{
    private HookController hook;
    private bool allowHold = false;
    private float enemyPower = 1f;
    private float progress = 1f; // START AT 1.0 (FULL BAR/TIMER)
    
    // Base stats
    private float decayRate = 0.45f; // Slower decay
    private float gainPerPress = 0.08f; // Slower gain
    
    // Multipliers from Upgrades
    private float currentGainMultiplier = 1f;
    private float currentDecayMultiplier = 1f;

    private bool isStruggling = false;
    private float struggleTimer = 0f;
    private Action onSuccess;
    private Action onFail;

    // ADDED struggleDuration parameter
   public void StartMinigame(HookController hookCtrl, bool allowHoldMode, float power, Action successCallback, Action failCallback, float struggleDuration)
    {
        hook = hookCtrl;
        allowHold = allowHoldMode;
        enemyPower = power;

        // --- CALCULATE BONUSES ---
        currentGainMultiplier = 1f;   // 1.0 = 100% (Normal Speed)
        currentDecayMultiplier = 1f;  // 1.0 = 100% (Normal Difficulty)

        if (FishingManager.Instance != null)
        {
            // 1. BAIT: +5% Gain, -5% Decay
            if (FishingManager.Instance.baitEquipped)
            {
                currentGainMultiplier += 0.05f;
                currentDecayMultiplier -= 0.05f;
            }

            // 2. MAGNET: +15% Gain
            if (FishingManager.Instance.magnetEquipped) // Or use magnetBonusPercent if you kept it
                currentGainMultiplier += 0.15f;

            // 3. RAGE BAIT: +25% Gain
            if (FishingManager.Instance.rageEquipped) 
                currentGainMultiplier += 0.25f;

            // 4. SPINNER: -20% Decay (Easier to hold)
            if (FishingManager.Instance.spinnerEquipped) 
                currentDecayMultiplier -= 0.30f;

            // 5. TRAPPER: -30% Decay (Much easier to hold)
            if (FishingManager.Instance.trapperEquipped) 
                currentDecayMultiplier -= 0.20f;

            // (Barbed Hook logic is handled in HookController capacity, no physics change needed here)
        }

        // Prevent Decay from ever becoming negative (impossible to lose)
        if (currentDecayMultiplier < 0.1f) currentDecayMultiplier = 0.1f;

        // --- REST OF SETUP ---
        progress = 1f; 
        onSuccess = successCallback;
        onFail = failCallback;
        
        isStruggling = true; 
        struggleTimer = struggleDuration; 
        
        hook.StopAutoPull(); 
        hook.StartShaking(); 
        hook.allowPlayerControl = false;
        
        FishingUIManager.Instance.ShowPull(1f, "FISH ON! PULL!");
    }

    private void Update()
    {
        if (hook == null) return;

        // **NEW LOGIC: CHECK IF HOOK REACHES THE SURFACE**
        // If the hook is at the surface, force success immediately.
        if (hook.transform.localPosition.y >= hook.startLocalPos.y)
        {
            ForceEndSuccess();
            return;
        }

        if (isStruggling)
        {
            UpdateStruggle(); // Bar decays, player spams to fill
        }
        else 
        {
            // The hook is currently pulling up (AutoPulling)
            // Check for a chance to re-initiate struggle
            struggleTimer -= Time.deltaTime;
            
            if (struggleTimer <= 0f)
            {
                // Reset timer for next struggle check
                struggleTimer = UnityEngine.Random.Range(0.5f, 1.5f); 
                
                // Random chance to start struggling again while pulling up
                if (UnityEngine.Random.value < 0.3f) // e.g., 30% chance
                {
                    BeginStruggle();
                }
            }
        }
        
        FishingUIManager.Instance.UpdatePull(progress);
    }

    void BeginStruggle()
    {
        isStruggling = true;
        
        // Reset bar to 1.0 (full timer)
        progress = 1f; 
        
        // Disable hook movement and enable shaking
        hook.StopAutoPull(); 
        hook.allowPlayerControl = false; 
        hook.StartShaking();
        
        struggleTimer = UnityEngine.Random.Range(1.3f, 2f); 
        
        FishingUIManager.Instance.ShowPull(progress, "THE FISH IS STRUGGLING!");
    }

    void EndStruggle()
    {
        isStruggling = false;
        
        // Enable hook movement (Start going UP) and stop shaking
        hook.allowPlayerControl = true;
        hook.StartAutoPull(true);
        hook.StopShaking();
        
        // Reset timer for the next struggle check while pulling up
        struggleTimer = UnityEngine.Random.Range(0.5f, 1.5f); 
        FishingUIManager.Instance.ShowPull(progress, "PULLING UP...");
    }

    void UpdateStruggle()
    {
        // 1. Bar Decays (Timer goes down)
        // Applying the Multiplier here makes it decay slower if you have upgrades
        progress -= Time.deltaTime * decayRate * enemyPower * currentDecayMultiplier;
        
        // 2. Player Spams to Gain Progress
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) 
        {
            // Applying the Multiplier here makes it fill faster if you have upgrades
            progress += gainPerPress * currentGainMultiplier;
        }
        
        progress = Mathf.Clamp01(progress);
        
        // Success Condition: Player filled the bar
        if (progress >= 1f) 
        {
            EndStruggle(); 
        }
        
        // Failure Condition: Timer ran out
        if (progress <= 0f) 
        {
            Fail();
        }
    }

    void Fail() 
    {
        hook.StopAutoPull();
        hook.StopShaking();
        // Restore player control before destruction
        hook.allowPlayerControl = true; 
        FishingUIManager.Instance.HidePull();
        onFail?.Invoke();
        Destroy(this);
    }

    public void ForceEndSuccess() 
    {
        // Final success, regardless of current state
        hook.StartAutoPull(true);
        hook.StopShaking();
        // Restore player control before destruction
        hook.allowPlayerControl = true; 
        FishingUIManager.Instance.HidePull();
        onSuccess?.Invoke();
        Destroy(this);
    }

    public float GetProgress01() 
    { 
        return progress; 
    }
}