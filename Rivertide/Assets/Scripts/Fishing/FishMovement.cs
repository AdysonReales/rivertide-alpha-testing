using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public FishData fishData;
    public float speed = 1f;
    private Rigidbody2D rb;
    private bool leftToRight = true;
    private float kgValue;
    public SpriteRenderer sr;

    public void Setup(FishData data, bool spinner)
    {
        fishData = data;
        kgValue = Random.Range(data.minKg, data.maxKg);
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        // size by kg: normalize between minKg and maxKg
        float size = Mathf.Lerp(0.6f, 1.6f, (kgValue - data.minKg) / Mathf.Max(0.0001f, (data.maxKg - data.minKg)));
        transform.localScale = new Vector3(size,size,1);
        speed = Random.Range(0.6f, 1.6f) * (1f + (kgValue / 10f)); // heavy = slightly slower/faster — tweak
        if (Random.value > 0.5f) leftToRight = false;
        // spinner makes fish roam into more vertical range — handled by swim logic
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float dir = leftToRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, Mathf.Sin(Time.time * speed) * 0.4f);
        // Flip sprite based on dir
        sr.flipX = !leftToRight;
    }

    public float GetKG() => kgValue;
}
