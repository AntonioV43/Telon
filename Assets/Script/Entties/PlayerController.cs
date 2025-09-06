using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

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

    [Header("Visuals / Animation")]
    public SpriteRenderer bodyRenderer; // assign Player's SpriteRenderer
    public Animator animator;           // optional: assign Animator if you use one

    // Track which objects are visible
    private HashSet<GameObject> visibleObjects = new HashSet<GameObject>();

    void Update()
    {
        ClicktoMove();
        HoldingItem();
        RotateFlashlight();
        UpdateVisibility();
        UpdateFacingByCursor(); // ✅ NEW — face player & update animations
    }

    // ✅ Check if object is currently visible
    public bool IsObjectVisible(GameObject obj)
    {
        return visibleObjects.Contains(obj);
    }

    // 🔹 Rotate flashlight toward cursor
    private void RotateFlashlight()
    {
        if (flashlightPivot == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        flashlightPivot.up = mousePos - (Vector2)flashlightPivot.position;
    }

    // 🔹 Update character facing & animation based on cursor
    private void UpdateFacingByCursor()
    {
        if (!animator)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - (Vector2)transform.position;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        // Determine facing direction
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        int facing;
        bool horizontalDominant = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);

        if (horizontalDominant)
            facing = dir.x >= 0f ? 1 : 3;
        else
            facing = dir.y >= 0f ? 0 : 2;

        // ✅ Send facing direction to Animator
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        animator.SetInteger("Facing", facing);

        // ✅ Use bool for walk/idle
        bool isMoving = Vector2.Distance(transform.position, target) > 0.05f && !isBlocked;
        animator.SetBool("IsMoving", isMoving);
    }

    // 🔹 Hide/Show objects depending on flashlight
    private void UpdateVisibility()
    {
        visibleObjects.Clear();

        Collider2D[] hiddenObjects = Physics2D.OverlapCircleAll(
            transform.position,
            20f,
            hiddenLayerMask
        );

        foreach (var obj in hiddenObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            Vector2 dirToObj = obj.transform.position - flashlightPivot.position;
            float angleToObj = Vector2.Angle(flashlightPivot.up, dirToObj);

            bool isLit = angleToObj < flashlightLight.pointLightOuterAngle / 2 &&
                        dirToObj.magnitude <= flashlightLight.pointLightOuterRadius;

            Color c = sr.color;
            float targetAlpha = isLit ? 1f : 0f;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            sr.color = c;

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