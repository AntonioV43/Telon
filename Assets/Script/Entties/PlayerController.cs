using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 target;
    private bool isBlocked = false;
    private float collisionTimer = 0f;
    public float speed = 3f;
    public Transform holdingPoint;
    public LayerMask pickableLayer;
    public Vector3 Direction { get; set; }
    private GameObject itemHolding;

    void Update()
    {
        ClicktoMove();
        HoldingItem();
    }

    private void HoldingItem()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (itemHolding)
            {
                // Drop item in the facing direction
                itemHolding.transform.position = transform.position + Direction;
                itemHolding.transform.parent = null;

                if (itemHolding.GetComponent<Rigidbody2D>())
                    itemHolding.GetComponent<Rigidbody2D>().simulated = true;

                itemHolding = null;
            }
            else
            {
                // Pickup item in front of player
                Collider2D pickupItem = Physics2D.OverlapCircle(transform.position + Direction, 0.5f, pickableLayer);
                if (pickupItem)
                {
                    itemHolding = pickupItem.gameObject;
                    itemHolding.transform.position = holdingPoint.position;
                    itemHolding.transform.parent = transform;

                    if (itemHolding.GetComponent<Rigidbody2D>())
                        itemHolding.GetComponent<Rigidbody2D>().simulated = false;
                }
            }
        }
    }

    private void ClicktoMove()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast to check if we clicked on a box
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Interactable"))
            {
                // Find pivot point inside the box
                Transform pivot = hit.collider.transform.Find("Pivot");
                if (pivot != null)
                {
                    target = pivot.position;
                }
                else
                {
                    Debug.LogWarning("Pivot not found on " + hit.collider.name);
                }
            }
            else
            {
                // If clicked on empty ground, move there instead
                target = mousePosition;
            }

            // Update Direction → normalized vector toward target
            Vector2 dir = target - (Vector2)transform.position;
            Direction = dir.normalized;

            // Reset movement block state
            isBlocked = false;
            collisionTimer = 0f;
        }

        // Move only if not blocked
        if (!isBlocked)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
        }
    }

    // Stop if colliding for more than 0.1s
    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((Vector2)transform.position != target)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= 0.1f)
            {
                isBlocked = true;
            }
        }
    }

    // Reset when leaving collision
    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionTimer = 0f;
        isBlocked = false;
    }
}
