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
    if (worldCanvasGroup == null)
    {
        Debug.LogError("WorldUIManager: worldCanvasGroup is NULL!");
        return;
    }

    worldCanvasGroup.interactable = false;
    worldCanvasGroup.blocksRaycasts = false;
}

public void EnableWorldUI()
{
    if (worldCanvasGroup == null)
    {
        Debug.LogError("WorldUIManager: worldCanvasGroup is NULL!");
        return;
    }

    worldCanvasGroup.interactable = true;
    worldCanvasGroup.blocksRaycasts = true;
}

}
