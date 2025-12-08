using UnityEngine;
using System;
using System.Collections.Generic;

public class HookController : MonoBehaviour
{
    public enum HookState { Idle, Dropping, AutoPulling, Returning }
    public HookState state = HookState.Idle;
    [HideInInspector] public bool allowPlayerControl = true;

    [Header("Hook Settings")]
    public float dropSpeed = 6f;
    public float pullSpeed = 4f;
    public float returnSpeed = 7f;
    
    public int capacity = 1; 
    public List<FishMovement> caughtFishes = new List<FishMovement>();

    private bool shaking = false;
    private float shakeIntensity = 0.05f;

    public float maxDepth = 4f;

    public Transform hookTip; // Bottom of hook (where fish attach)
    
    // --- NEW: Line Renderer Variables ---
    public Transform rodTipPoint; // Assign the Empty Object at top of screen here
    private LineRenderer lineRenderer;
    // ------------------------------------

    public event Action<List<FishMovement>> OnFishCaught;
    public event Action OnHookReturned;
    public event Action OnFishLanded;

    [HideInInspector] public Vector3 startLocalPos;
    private float bottomLocalY;
    private bool autoPull = false;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
        ComputeBottomLocalY();
        transform.localPosition = startLocalPos;
        
        // Get the Line Renderer attached to this object
        lineRenderer = GetComponent<LineRenderer>();
        
        // Setup simple 2-point line
        if(lineRenderer != null) lineRenderer.positionCount = 2;
    }

    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            startLocalPos = transform.localPosition;
            ComputeBottomLocalY();
        }
    }

    void ComputeBottomLocalY()
    {
        if (maxDepth < 0f) bottomLocalY = maxDepth;
        else bottomLocalY = startLocalPos.y - Mathf.Abs(maxDepth);

        if (bottomLocalY >= startLocalPos.y) bottomLocalY = startLocalPos.y - 0.1f;
    }

    private void Update()
    {
        // --- NEW: UPDATE LINE POSITION ---
        if (lineRenderer != null && rodTipPoint != null)
        {
            // Point 0 = Top (Rod)
            lineRenderer.SetPosition(0, rodTipPoint.position);
            // Point 1 = Bottom (This Hook)
            lineRenderer.SetPosition(1, transform.position);
        }
        // ---------------------------------

        switch (state)
        {
            case HookState.Dropping: HandleManualDrop(); break;
            case HookState.AutoPulling: HandleAutoPull(); break;
            case HookState.Returning: HandleReturn(); break;
        }
    }

    void HandleManualDrop()
    {
        if (!allowPlayerControl) return;

        if (Input.GetKey(KeyCode.F) || Input.GetMouseButton(0))
        {
            transform.localPosition += Vector3.down * dropSpeed * Time.deltaTime;

            if (transform.localPosition.y <= bottomLocalY)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, bottomLocalY, transform.localPosition.z);
                
                if (caughtFishes.Count > 0) TriggerCatch();
                else state = HookState.Returning;
            }
        }
    }

    void HandleAutoPull()
    {
        if (!autoPull) return;

        transform.localPosition += Vector3.up * pullSpeed * Time.deltaTime;

        if (transform.localPosition.y >= 3f && state == HookState.AutoPulling)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 3f, transform.localPosition.z);
            state = HookState.Returning;
            autoPull = false;
            OnFishLanded?.Invoke();
            return;
        }

        if (transform.localPosition.y >= startLocalPos.y)
        {
            transform.localPosition = startLocalPos;
            state = HookState.Idle;
            autoPull = false;
            OnHookReturned?.Invoke();
        }
    }

    void HandleReturn()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, startLocalPos, returnSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, startLocalPos) < 0.01f)
        {
            transform.localPosition = startLocalPos;
            state = HookState.Idle;
            ClearFishes();
            OnHookReturned?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state != HookState.Dropping) return;

        var fish = other.GetComponent<FishMovement>();
        if (fish != null)
        {
            if (caughtFishes.Contains(fish)) return;

            caughtFishes.Add(fish);
            fish.LockToHook(hookTip);
            
            // Stack fish visually
            fish.transform.localPosition = new Vector3(0, -0.5f * (caughtFishes.Count - 1), 0);

            if (caughtFishes.Count >= capacity)
            {
                TriggerCatch();
            }
        }
    }

    void TriggerCatch()
    {
        state = HookState.AutoPulling;
        autoPull = false; 
        OnFishCaught?.Invoke(caughtFishes);
    }

    public void ClearFishes()
    {
        caughtFishes.Clear();
    }

    public void StartDrop()
    {
        if (state == HookState.Idle)
        {
            ClearFishes(); 
            state = HookState.Dropping;
            ComputeBottomLocalY();
        }
    }

    public void StartAutoPull(bool force = false)
    {
        if (force || state == HookState.AutoPulling)
        {
            autoPull = true;
            state = HookState.AutoPulling;
        }
    }

    public void StopAutoPull() => autoPull = false;

    public void StartReturn()
    {
        state = HookState.Returning;
        autoPull = false;
    }

    public void StartShaking() { shaking = true; }
    public void StopShaking() 
    { 
        shaking = false; 
        transform.localPosition = new Vector3(0, transform.localPosition.y, 0); 
    }

    void LateUpdate()
    {
        if (shaking)
        {
            float shake = Mathf.Sin(Time.time * 40f) * shakeIntensity;
            transform.localPosition = new Vector3(shake, transform.localPosition.y, 0);
        }
    }
}