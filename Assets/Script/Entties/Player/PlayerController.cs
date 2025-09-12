using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private ItemHandler items;
    private Flashlight flashlight;
    private Animation animationHandler;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        items = GetComponent<ItemHandler>();
        flashlight = GetComponent<Flashlight>();
        animationHandler = GetComponent<Animation>();
    }

    void Update()
    {
        if (FadeUI.Instance != null && FadeUI.Instance.IsFading) return;

        movement.HandleMovement();
        items.HandleItems();
        flashlight.HandleFlashlight();
        animationHandler.HandleAnimation();
    }

    public void SetNewTarget(Vector3 newPos)
    {
        movement.ResetTarget(newPos);
    }

    public Vector3 GetDirection()
    {
        return movement.Direction;
    }
}