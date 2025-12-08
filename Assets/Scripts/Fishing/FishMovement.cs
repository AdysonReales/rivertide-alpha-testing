using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public FishData data;
    public float kg;
    public float speed = 1f;

    private Rect swimArea;
    private Vector3 moveDirection;
    private SpriteRenderer sr;
    private Transform hookParent;
    private bool isCaught = false;

    public void Initialize(FishData fishData, float weight, Rect swimRect, float speedMultiplier = 1f)
    {
        data = fishData;
        kg = weight;
        swimArea = swimRect;
        speed = speedMultiplier;

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.sprite;

        // --- NEW: SCALE FISH BASED ON RARITY ---
        float sizeMultiplier = 1f;
        switch (data.rarity)
        {
            case FishData.Rarity.Common:
                sizeMultiplier = 1f; // Standard size
                break;
            case FishData.Rarity.Uncommon:
                sizeMultiplier = 1.3f; // A bit bigger
                break;
            case FishData.Rarity.Rare:
                sizeMultiplier = 1.6f; // Big
                break;
            case FishData.Rarity.UltraRare:
                sizeMultiplier = 2.2f; // Huge!
                break;
        }
        // Apply the scale (Keep Z as 1)
        transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, 1f);
        // ---------------------------------------

        moveDirection = Random.value < 0.5f ? Vector3.left : Vector3.right;
        FlipSprite();
    }

    private void Update()
    {
        if (hookParent != null) // locked to hook
        {
            transform.position = hookParent.position;
            return;
        }

        if (!isCaught)
        {
            transform.position += moveDirection * speed * Time.deltaTime;

            if (transform.position.x < swimArea.xMin || transform.position.x > swimArea.xMax)
            {
                moveDirection *= -1;
                FlipSprite();
            }
        }
    }

    void FlipSprite()
    {
        if (sr != null)
            // Note: We multiply by localScale.x to keep the size, just invert sign
            sr.flipX = moveDirection.x > 0;
    }

    public void LockToHook(Transform hook)
    {
        hookParent = hook;
        isCaught = true;
    }

    public void StartReturnToSwim()
    {
        if (hookParent != null)
        {
            hookParent = null;
            isCaught = false;
        }

        moveDirection = Random.value < 0.5f ? Vector3.left : Vector3.right;
        FlipSprite();
    }
}