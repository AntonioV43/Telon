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

            // ✅ Get all 4 corners of the sprite bounds
            Bounds b = sr.bounds;
            Vector3[] points = new Vector3[4];
            points[0] = b.min; // bottom-left
            points[1] = new Vector3(b.min.x, b.max.y, b.min.z); // top-left
            points[2] = new Vector3(b.max.x, b.min.y, b.min.z); // bottom-right
            points[3] = b.max; // top-right

            foreach (var p in points)
            {
                Vector2 dirToPoint = p - flashlightPivot.position;
                float angleToPoint = Vector2.Angle(flashlightPivot.up, dirToPoint);

                if (angleToPoint < flashlightLight.pointLightOuterAngle / 2 &&
                    dirToPoint.magnitude <= flashlightLight.pointLightOuterRadius)
                {
                    isLit = true;
                    break; // ✅ At least one corner is lit → reveal the whole sprite
                }
            }

            // ✅ Extra: if touching player, always visible
            if (!isLit && playerCollider != null && obj.IsTouching(playerCollider))
                isLit = true;

            // Fade in/out smoothly
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