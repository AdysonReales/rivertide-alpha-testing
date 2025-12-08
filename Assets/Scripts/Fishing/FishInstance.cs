[System.Serializable]
public class FishInstance
{
    public string id; // unique id maybe name + timestamp
    public FishData data;
    public float kg;

    public FishInstance(FishData d, float weight)
    {
        data = d;
        kg = weight;
        id = d.fishName + "_" + System.DateTime.Now.Ticks;
    }
}
