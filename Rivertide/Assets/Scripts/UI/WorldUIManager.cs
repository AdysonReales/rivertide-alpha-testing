using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager Instance;
    

    private void Awake()
    {
        Debug.Log("WorldUIManager has initialized!");
        Instance = this;
    }

    public CanvasGroup worldCanvasGroup;

    public void DisableWorldUI()
    {

        worldCanvasGroup.interactable = false;
        worldCanvasGroup.blocksRaycasts = false;
    }

    public void EnableWorldUI()
    {
        worldCanvasGroup.interactable = true;
        worldCanvasGroup.blocksRaycasts = true;
    }
}
