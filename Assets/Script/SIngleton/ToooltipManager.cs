using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToooltipManager : MonoBehaviour
{
    public static ToooltipManager _Instance;
    public TextMeshProUGUI textComponent;
    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Instance = this;
        }
    }

    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ShowTooltip(string text)
    {
        gameObject.SetActive(true);
        textComponent.text = text;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
