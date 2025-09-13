using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Chase Settings")]
    public float moveSpeed = 2f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackCooldown = 1.5f; // seconds between hits
    public float vanishTime = 15f;      // how long enemy disappears when killed

    private Transform player;
    private Rigidbody2D rb;
    private StatManager stats;
    private StatManager playerStats;

    private Vector2 movement;
    private float lastAttackTime = 0f;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<StatManager>();

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<StatManager>();
        }

        if (stats != null)
        {
            // Hook into StatManager "death"
            stats.OnDeath += HandleDeath;
        }
    }

    private void Update()
    {
        if (player == null || isDead) return;

        // ✅ Always chase player
        Vector2 direction = (player.position - transform.position).normalized;
        movement = direction;

        // ✅ Try to damage if close enough
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 1f && Time.time >= lastAttackTime + attackCooldown)
        {
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && rb != null && movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleDeath()
    {
        if (!isDead)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} vanishes for {vanishTime} seconds!");

            // Disable visuals and collider
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            // Start respawn
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(vanishTime);

        if (stats != null)
            stats.ResetHealth();

        // Re-enable
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        isDead = false;
        Debug.Log($"{gameObject.name} has respawned with full HP!");
    }
}