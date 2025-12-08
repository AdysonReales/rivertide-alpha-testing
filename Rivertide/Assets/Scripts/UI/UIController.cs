using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("World UI")]
    public GameObject inventoryButton;
    public TextMeshProUGUI worldMoneyText;
    public GameObject darkBackground;

    [HideInInspector] public bool isUIBlocked = false;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Block or unblock world UI (money + inventory button)
    /// </summary>
public void BlockUI(bool block)
{
    isUIBlocked = block;
    if (inventoryButton != null) inventoryButton.SetActive(!block);
    if (worldMoneyText != null) worldMoneyText.gameObject.SetActive(!block);
}

    public void ShowDarkBackground(bool show)
    {
        if (darkBackground != null)
            darkBackground.SetActive(show);
    }
}
