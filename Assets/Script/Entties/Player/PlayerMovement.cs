using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;

    private Vector2 target;
    private Vector2 movement;
    private Rigidbody2D rb;

    public Vector3 Direction { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = transform.position;
    }

    public void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target = mousePosition;

            Vector2 dir = target - (Vector2)transform.position;
            Direction = dir.normalized;
        }

        // Calculate direction every frame
        Vector2 diff = target - (Vector2)transform.position;
        if (diff.magnitude > 0.05f)
            movement = diff.normalized;
        else
            movement = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    public void ResetTarget(Vector3 newPos)
    {
        target = newPos;
        movement = Vector2.zero;
        Direction = Vector3.zero;
    }
}