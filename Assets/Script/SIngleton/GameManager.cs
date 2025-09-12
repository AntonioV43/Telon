using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public PlayerController player;  // assign your player in Inspector
    public Camera mainCamera;        // assign your main camera in Inspector

    [Header("Game State")]
    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (!mainCamera) mainCamera = Camera.main;
    }

    // ✅ Pause / Resume game
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;

        // Show / Hide pause menu (if you have UI manager later)
        if (IsPaused)
            ToooltipManager._Instance.ShowTooltip("Game Paused - Press ESC to Resume");
        else
            ToooltipManager._Instance.HideTooltip();
    }

    // ✅ Teleport player + camera
    public void TeleportPlayer(Vector3 playerPos, Vector3? cameraPos = null)
    {
        if (player != null)
        {
            player.transform.position = playerPos;
            player.SetNewTarget(playerPos); // stop movement
        }

        if (mainCamera != null && cameraPos.HasValue)
        {
            Vector3 cam = mainCamera.transform.position;
            cam.x = cameraPos.Value.x;
            cam.y = cameraPos.Value.y;
            mainCamera.transform.position = cam;
        }
    }

    // ✅ Teleport with fade
    public void TeleportWithFade(Vector3 playerPos, Vector3? cameraPos = null)
    {
        if (FadeUI.Instance != null)
        {
            StartCoroutine(FadeUI.Instance.FadeOutIn(() =>
            {
                TeleportPlayer(playerPos, cameraPos);
            }));
        }
        else
        {
            TeleportPlayer(playerPos, cameraPos);
        }
    }

    // ✅ Scene management (future-proof)
    public void LoadScene(string sceneName)
    {
        if (FadeUI.Instance != null)
        {
            StartCoroutine(FadeUI.Instance.FadeOutIn(() =>
            {
                SceneManager.LoadScene(sceneName);
            }));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}