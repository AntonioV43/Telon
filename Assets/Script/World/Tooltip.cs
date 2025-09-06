using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string text;

    private void OnMouseEnter()
    {
        // ✅ Use FindFirstObjectByType instead of FindObjectOfType
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && player.IsObjectVisible(gameObject))
        {
            ToooltipManager._Instance.ShowTooltip(text);
        }
    }

    private void OnMouseExit()
    {
        ToooltipManager._Instance.HideTooltip();
    }
}