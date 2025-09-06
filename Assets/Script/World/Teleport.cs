using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Targets")]
    public Transform teleportPivot;  // Player teleport destination
    public Transform cameraPivot;    // Camera teleport destination

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.F;

    private bool isPlayerInside = false;
    private Transform player;
    private PlayerController playerController;
    private Camera mainCamera;

    private void Start()
    {
        // Cache main camera
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.transform;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            player = null;
            playerController = null;

            // Hide tooltip when leaving
            ToooltipManager._Instance.HideTooltip();
        }
    }

    private void Update()
    {
        if (!isPlayerInside) return;

        // Always show tooltip when inside trigger
        ToooltipManager._Instance.ShowTooltip("Press F to Teleport");

        // Start teleport if F is pressed
        if (Input.GetKeyDown(interactKey) && teleportPivot != null && player != null)
        {
            // Use FadeManager for smooth transition
            if (FadeUI.Instance != null)
            {
                StartCoroutine(FadeUI.Instance.FadeOutIn(() =>
                {
                    DoTeleport();
                }));
            }
            else
            {
                // Fallback: instant teleport if no FadeManager
                DoTeleport();
            }
        }
    }

    private void DoTeleport()
    {
        // Move player
        player.position = teleportPivot.position;

        // Move camera if pivot assigned
        if (mainCamera != null && cameraPivot != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            camPos.x = cameraPivot.position.x;
            camPos.y = cameraPivot.position.y;
            mainCamera.transform.position = camPos;
        }

        // Stop player movement immediately
        if (playerController != null)
        {
            playerController.Direction = Vector3.zero;
            playerController.SetNewTarget(teleportPivot.position);
        }

        // Hide tooltip after teleport
        ToooltipManager._Instance.HideTooltip();
    }
}
