using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 target;
    private bool isBlocked = false;
    private float collisionTimer = 0f;
    public float speed = 3f;

    void Update()
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

    // Stop if colliding for more than 0.3s
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