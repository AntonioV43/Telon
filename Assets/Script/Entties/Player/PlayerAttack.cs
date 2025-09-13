using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public KeyCode attackKey = KeyCode.F;
    public int baseDamage = 10;
    public float chargeMultiplier = 2f; // damage multiplier
    public float maxChargeTime = 2f;    // time to full charge
    public float flashDuration = 0.2f;  // shutter flash time
    public float attackCooldown = 1.5f; // delay between attacks

    [Header("Flashlight Effects")]
    public Light2D flashlightLight;
    public float normalIntensity = 1f;
    public float chargeIntensity = 3f;
    public float flashIntensity = 8f;

    private Flashlight flashlight;
    private float chargeTimer = 0f;
    private bool isCharging = false;
    private float lastAttackTime = -999f;

    private void Start()
    {
        flashlight = GetComponent<Flashlight>();

        if (flashlightLight == null && flashlight != null)
            flashlightLight = flashlight.flashlightLight;
    }

    private void Update()
    {
        if (flashlight == null || flashlightLight == null) return;

        // ✅ Prevent spamming: check cooldown
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // ✅ Start charging
        if (Input.GetKeyDown(attackKey))
        {
            isCharging = true;
            chargeTimer = 0f;
            Debug.Log("Charging flashlight attack...");
        }

        // ✅ Charging
        if (isCharging && Input.GetKey(attackKey))
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer > maxChargeTime) chargeTimer = maxChargeTime;

            // Increase light intensity gradually
            float t = chargeTimer / maxChargeTime;
            flashlightLight.intensity = Mathf.Lerp(normalIntensity, chargeIntensity, t);
        }

        // ✅ Release attack
        if (isCharging && Input.GetKeyUp(attackKey))
        {
            float chargePercent = chargeTimer / maxChargeTime;
            int finalDamage = Mathf.RoundToInt(baseDamage + (baseDamage * chargePercent * chargeMultiplier));

            Debug.Log($"📸 Flashlight shutter attack released! Damage: {finalDamage}");

            // Flash + deal damage
            StartCoroutine(FlashEffect(finalDamage));

            // Reset charging state
            isCharging = false;
            chargeTimer = 0f;

            // ✅ Set cooldown
            lastAttackTime = Time.time;
        }
    }

    private System.Collections.IEnumerator FlashEffect(int damage)
    {
        // Super bright flash
        flashlightLight.intensity = flashIntensity;

        // Deal damage to all visible enemies
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f);
        foreach (var enemy in enemies)
        {
            StatManager enemyStats = enemy.GetComponent<StatManager>();
            if (enemyStats != null && flashlight.IsObjectVisible(enemy.gameObject))
            {
                enemyStats.TakeDamage(damage);
                Debug.Log($"📸 {enemy.name} took {damage} damage from shutter flash!");
            }
        }

        // Flash duration
        yield return new WaitForSeconds(flashDuration);

                // Reset to normal
                flashlightLight.intensity = normalIntensity;
            }
        }