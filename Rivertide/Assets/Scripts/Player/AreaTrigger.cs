using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class AreaTrigger : MonoBehaviour, IInteractable
{
    public enum AreaType { River, Shop, Truck }
    public AreaType areaType;

    [SerializeField] private string customInteractText; // Optional override

    private void Reset()
    {
        // Ensure the collider is a trigger
        if (TryGetComponent<Collider2D>(out var col))
            col.isTrigger = true;
    }

    public void OnInteract()
    {
        switch (areaType)
        {
            case AreaType.River:
                SceneManager.LoadScene("Fishing");
                break;
            case AreaType.Shop:
                UIManager.Instance.OpenShop();
                break;
            case AreaType.Truck:
                UIManager.Instance.OpenDisplay();
                break;
        }
    }

    public string GetInteractText()
    {
        if (!string.IsNullOrEmpty(customInteractText)) return customInteractText;

        return areaType switch
        {
            AreaType.River => "Press E to Fish",
            AreaType.Shop => "Press E to Talk",
            AreaType.Truck => "Press E to Display Fish",
            _ => "Press E"
        };
    }
}
