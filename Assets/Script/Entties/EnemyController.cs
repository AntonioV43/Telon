using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Chase Settings")]
    public float detectionRange = 5f;
    public float moveSpeed = 2f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackCooldown = 1.5f; // seconds between hits

    private Transform player;
    private Rigidbody2D rb;
    private StatManager playerStats;
    private float lastAttackTime = 0f;

    private Vector2 movement;
    private bool isChasing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<StatManager>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            isChasing = true;
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction;

            // ✅ Try to damage if close enough
            if (distance < 1f && Time.time >= lastAttackTime + attackCooldown)
            {
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            isChasing = false;
            movement = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (isChasing && rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}