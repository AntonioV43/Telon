using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public string text;
    private void OnMouseEnter()
    {
        ToooltipManager._Instance.ShowTooltip(text);
    }
    private void OnMouseExit()
    {
        ToooltipManager._Instance.HideTooltip();
    }
}
