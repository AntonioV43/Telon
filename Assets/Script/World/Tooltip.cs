using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string text;

    private void OnMouseEnter()
    {
        // ✅ Use FindFirstObjectByType instead of deprecated FindObjectOfType
        Flashlight flashlight = Object.FindFirstObjectByType<Flashlight>();
        if (flashlight != null && flashlight.IsObjectVisible(gameObject))
        {
            ToooltipManager._Instance.ShowTooltip(text);
        }
    }

    private void OnMouseExit()
    {
        ToooltipManager._Instance.HideTooltip();
    }
}