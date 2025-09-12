using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class Flashlight : MonoBehaviour
{
    public Transform flashlightPivot;
    public Light2D flashlightLight;
    public LayerMask hiddenLayerMask;
    public float fadeSpeed = 5f;

    private HashSet<GameObject> visibleObjects = new HashSet<GameObject>();
    private Collider2D playerCollider;

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    public void HandleFlashlight()
    {
        RotateFlashlight();
        UpdateVisibility();
    }

    private void RotateFlashlight()
    {
        if (!flashlightPivot) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        flashlightPivot.up = mousePos - (Vector2)flashlightPivot.position;
    }

    private void UpdateVisibility()
    {
        visibleObjects.Clear();

        Collider2D[] hiddenObjects = Physics2D.OverlapCircleAll(transform.position, 20f, hiddenLayerMask);

        foreach (var obj in hiddenObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            bool isLit = false;

            // ✅ Check flashlight cone
            Vector2 dirToObj = obj.transform.position - flashlightPivot.position;
            float angleToObj = Vector2.Angle(flashlightPivot.up, dirToObj);

            if (angleToObj < flashlightLight.pointLightOuterAngle / 2 &&
                dirToObj.magnitude <= flashlightLight.pointLightOuterRadius)
            {
                isLit = true;
            }

            // ✅ Extra check: touching player collider
            if (!isLit && playerCollider != null && obj.IsTouching(playerCollider))
            {
                isLit = true;
            }

            // Fade in/out
            Color c = sr.color;
            float targetAlpha = isLit ? 1f : 0f;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            sr.color = c;

            if (isLit)
                visibleObjects.Add(obj.gameObject);
        }
    }

    public bool IsObjectVisible(GameObject obj)
    {
        return visibleObjects.Contains(obj);
    }
}