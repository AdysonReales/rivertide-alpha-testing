using UnityEngine;
using System.Collections.Generic;

public static class SaveLoad
{
    const string saveKey = "FISHING_SAVE_V1";

    [System.Serializable]
    class SaveData
    {
        public int money;
        public List<SerializableFish> fishes = new List<SerializableFish>();
        public Dictionary<string,int> items = new Dictionary<string,int>();
    }

    [System.Serializable]
    public class SerializableFish
    {
        public string name;
        public string fishDataName;
        public float kg;
    }

    public static void SaveGame()
    {
        var s = new SaveData();
        s.money = GameManager.Instance.money;
        foreach(var f in GameManager.Instance.playerInventory.fishes)
        {
            s.fishes.Add(new SerializableFish{
                name = f.id,
                fishDataName = f.data.fishName,
                kg = f.kg
            });
        }
        s.items = GameManager.Instance.playerInventory.items;
        string json = JsonUtility.ToJson(s);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

public static Inventory LoadInventory()
{
    if (!PlayerPrefs.HasKey("inventory"))
        return null;

    string json = PlayerPrefs.GetString("inventory");
    return JsonUtility.FromJson<Inventory>(json);
}


public static void SaveInventory(Inventory inv)
{
    string json = JsonUtility.ToJson(inv, true);
    PlayerPrefs.SetString("inventory", json);
    PlayerPrefs.Save();
}


    public static void LoadGame()
    {
        if (!PlayerPrefs.HasKey(saveKey)) return;
        string json = PlayerPrefs.GetString(saveKey);
        var s = JsonUtility.FromJson<SaveData>(json);
        if (GameManager.Instance == null) return;
        GameManager.Instance.money = s.money;
        GameManager.Instance.playerInventory = new Inventory();
        if (s.items != null) GameManager.Instance.playerInventory.items = s.items;
        if (s.fishes != null)
        {
            foreach(var sf in s.fishes)
            {
                var fd = GameManager.Instance.allFishData.Find(x => x.fishName == sf.fishDataName);
                if (fd != null)
                {
                    GameManager.Instance.playerInventory.fishes.Add(new FishInstance(fd, sf.kg));
                }
            }
        }
    }
}
