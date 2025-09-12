using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator animator;

    private PlayerMovement movement;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    public void HandleAnimation()
    {
        if (!animator) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        int facing = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y)
            ? (dir.x >= 0 ? 1 : 3) // Right / Left
            : (dir.y >= 0 ? 0 : 2); // Up / Down

        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        animator.SetInteger("Facing", facing);

        bool isMoving = Vector2.Distance(transform.position, movement.transform.position) > 0.05f;
        animator.SetBool("IsMoving", isMoving);
    }
}
