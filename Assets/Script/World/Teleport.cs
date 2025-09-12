using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Targets")]
    public Transform playerPivot;   // Where the player will teleport
    public Transform cameraPivot;   // Optional: where the camera will teleport

    private Camera mainCamera;
    private bool isTeleporting = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isTeleporting) return; // ignore if a teleport is already in progress

        PlayerController playerController = other.GetComponent<PlayerController>();
        StartCoroutine(TeleportCoroutine(other.transform, playerController));
    }

    private IEnumerator TeleportCoroutine(Transform playerTransform, PlayerController playerController)
    {
        isTeleporting = true;

        // If there's a FadeManager, use it (fade out -> teleport -> fade in)
        if (FadeUI.Instance != null)
        {
            // FadeOutIn takes a callback executed between fade-out and fade-in
            yield return StartCoroutine(FadeUI.Instance.FadeOutIn(() =>
            {
                DoTeleport(playerTransform, playerController);
            }));
        }
        else
        {
            // Fallback: instant teleport
            DoTeleport(playerTransform, playerController);
            yield return null;
        }

        isTeleporting = false;
    }

        private void DoTeleport(Transform playerTransform, PlayerController playerController)
    {
        // Teleport player
        if (playerPivot != null && playerTransform != null)
        {
            playerTransform.position = playerPivot.position;

            // ✅ Proper way: stop player movement
            if (playerController != null)
                playerController.SetNewTarget(playerPivot.position);
        }

        // Teleport camera (if assigned)
        if (mainCamera != null && cameraPivot != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            camPos.x = cameraPivot.position.x;
            camPos.y = cameraPivot.position.y;
            mainCamera.transform.position = camPos;
        }

        // Hide tooltip if present
        if (ToooltipManager._Instance != null)
            ToooltipManager._Instance.HideTooltip();
    }
}