using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public FishData[] allFish;  
    public GameObject fishPrefab;

    public float spawnInterval = 1.5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    void SpawnFish()
    {
        FishData data = ChooseFish();

        // spawn position (left or right)
        float y = Random.Range(-3f, 3f);
        float x = Random.Range(0, 2) == 0 ? -11f : 11f;

        GameObject f = Instantiate(fishPrefab, new Vector3(x, y, 0), Quaternion.identity);

        FishMovement movement = f.GetComponent<FishMovement>();
        movement.data = data;

        // apply fish sprite
        f.GetComponent<SpriteRenderer>().sprite = data.sprite;
    }

    FishData ChooseFish()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var fish in allFish)
        {
            cumulative += fish.baseChancePercent;
            if (roll <= cumulative)
                return fish;
        }

        return allFish[Random.Range(0, allFish.Length)];
    }
}
