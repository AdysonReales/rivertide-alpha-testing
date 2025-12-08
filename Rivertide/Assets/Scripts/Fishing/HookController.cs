using UnityEngine;
using System.Collections;

public class HookController : MonoBehaviour
{
    public float dropSpeed = 6f;
    public float maxDepth = -4f;
    public float riseSpeed = 4f;
    public bool isDown = false;
    private Rigidbody2D rb;
    private bool hooked = false;
    private FishMovement attachedFish;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void BeginDrop()
    {
        isDown = true;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        while (transform.localPosition.y > maxDepth && !hooked && (Input.GetKey(KeyCode.F) || Input.GetMouseButton(0)))
        {
            transform.Translate(Vector3.down * dropSpeed * Time.deltaTime);
            yield return null;
        }
        // If not hooked, start idle and check collisions
    }

    void Update()
    {
        // player can nudge left/right slightly with A/D — make oscillation.
        float h = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        transform.Translate(Vector3.right * h * 0.8f * Time.deltaTime); // limited swing that returns naturally due to gravity effect not modeled here.

        // If hooked, start pull routine when player spams F or click
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var fish = other.GetComponent<FishMovement>();
        if (fish && !hooked)
        {
            hooked = true;
            attachedFish = fish;
            fish.gameObject.SetActive(false); // hide the swimming fish
            StartCoroutine(PullUpRoutine());
        }
    }

    IEnumerator PullUpRoutine()
    {
        UIManager.Instance.ShowPullPrompt(true);
        float t = 0f;
        float progress = 0f;
        float required = 30f; // spam count threshold / or hold threshold
        while (transform.position.y < Camera.main.transform.position.y + 2f) // until surface
        {
            // spam detection
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            {
                progress += trapperMultiplier();
                // optionally show progress UI
            }
            // also allow hold to slowly raise
            if (Input.GetKey(KeyCode.F) || Input.GetMouseButton(0))
            {
                transform.Translate(Vector3.up * (riseSpeed * 0.5f) * Time.deltaTime);
            }
            // slight auto descent to encourage pulling
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
            yield return null;
        }
        // caught
        UIManager.Instance.ShowPullPrompt(false);
        // Create FishInstance from attachedFish
        FishInstance fi = new FishInstance(attachedFish.fishData, attachedFish.GetKG());
        Destroy(attachedFish.gameObject);
        FishingManager.Instance.OnFishCaught(fi);
        Destroy(gameObject);
        yield break;
    }

    float trapperMultiplier()
    {
        return FishingManager.Instance.trapperEquipped ? 1.5f : 1f;
    }
}
