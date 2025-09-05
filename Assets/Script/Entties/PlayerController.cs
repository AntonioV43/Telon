using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic; // ✅ For HashSet

public class PlayerController : MonoBehaviour
{
    private Vector2 target;
    private bool isBlocked = false;
    private float collisionTimer = 0f;

    [Header("Movement")]
    public float speed = 3f;
    public Vector3 Direction { get; set; }

    [Header("Item Handling")]
    public float maxPickupDistance = 1.5f;
    public float dropDistance = 1f;
    public Transform holdingPoint;
    public string pickableTag = "Pickable";
    private GameObject itemHolding;

    [Header("Flashlight")]
    public Transform flashlightPivot;
    public Light2D flashlightLight;
    public LayerMask hiddenLayerMask;
    public float fadeSpeed = 5f;

    // ✅ Track which objects are visible
    private HashSet<GameObject> visibleObjects = new HashSet<GameObject>();

    void Update()
    {
        ClicktoMove();
        HoldingItem();
        RotateFlashlight();
        UpdateVisibility();
    }

    // ✅ Check if a GameObject is currently visible
    public bool IsObjectVisible(GameObject obj)
    {
        return visibleObjects.Contains(obj);
    }

    private void RotateFlashlight()
    {
        if (flashlightPivot == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        flashlightPivot.up = mousePos - (Vector2)flashlightPivot.position;
    }

    private void UpdateVisibility()
    {
        visibleObjects.Clear();

        // Find all objects in the hidden layer
        Collider2D[] hiddenObjects = Physics2D.OverlapCircleAll(
            transform.position,
            20f,
            hiddenLayerMask
        );

        foreach (var obj in hiddenObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            // Calculate direction and angle relative to flashlight
            Vector2 dirToObj = obj.transform.position - flashlightPivot.position;
            float angleToObj = Vector2.Angle(flashlightPivot.up, dirToObj);

            // Is object inside flashlight cone?
            bool isLit = angleToObj < flashlightLight.pointLightOuterAngle / 2 &&
                        dirToObj.magnitude <= flashlightLight.pointLightOuterRadius;

            // Smooth alpha transition
            Color c = sr.color;
            float targetAlpha = isLit ? 1f : 0f;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            sr.color = c;

            // ✅ Add to visibleObjects if lit
            if (isLit)
                visibleObjects.Add(obj.gameObject);
        }
    }

    private void HoldingItem()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (itemHolding)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dropDirection = (mousePos - (Vector2)transform.position).normalized;
                Vector2 dropPos = (Vector2)transform.position + dropDirection * dropDistance;

                itemHolding.transform.position = dropPos;
                itemHolding.transform.parent = null;

                if (itemHolding.GetComponent<Rigidbody2D>())
                    itemHolding.GetComponent<Rigidbody2D>().simulated = true;

                itemHolding = null;
            }
            else
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                // ✅ Don't interact with invisible objects
                if (hit.collider != null &&
                    hit.collider.CompareTag(pickableTag) &&
                    IsObjectVisible(hit.collider.gameObject))
                {
                    float distance = Vector2.Distance(transform.position, hit.collider.transform.position);

                    if (distance <= maxPickupDistance)
                    {
                        itemHolding = hit.collider.gameObject;
                        itemHolding.transform.position = holdingPoint.position;
                        itemHolding.transform.parent = transform;

                        if (itemHolding.GetComponent<Rigidbody2D>())
                            itemHolding.GetComponent<Rigidbody2D>().simulated = false;
                    }
                    else
                    {
                        Debug.Log("Item too far away to pick up!");
                    }
                }
            }
        }
    }

    private void ClicktoMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // ✅ Ignore invisible interactables
            if (hit.collider != null && hit.collider.CompareTag("Interactable") &&
                IsObjectVisible(hit.collider.gameObject))
            {
                Transform pivot = hit.collider.transform.Find("Pivot");
                if (pivot != null)
                    target = pivot.position;
                else
                    Debug.LogWarning("Pivot not found on " + hit.collider.name);
            }
            else
            {
                target = mousePosition;
            }

            Vector2 dir = target - (Vector2)transform.position;
            Direction = dir.normalized;

            isBlocked = false;
            collisionTimer = 0f;
        }

        if (!isBlocked)
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((Vector2)transform.position != target)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= 0.1f)
                isBlocked = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionTimer = 0f;
        isBlocked = false;
    }
}