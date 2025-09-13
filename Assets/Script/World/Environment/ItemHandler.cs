using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public float maxPickupDistance = 1.5f;
    public float dropDistance = 1f;
    public Transform holdingPoint;
    public string pickableTag = "Pickable";

    private GameObject itemHolding;
    private PlayerMovement movement;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    public void HandleItems()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (itemHolding)
                DropItem();
            else
                TryPickupItem();
        }
    }

    private void DropItem()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dropDirection = (mousePos - (Vector2)transform.position).normalized;
        Vector2 dropPos = (Vector2)transform.position + dropDirection * dropDistance;

        itemHolding.transform.position = dropPos;
        itemHolding.transform.parent = null;

        if (itemHolding.GetComponent<Rigidbody2D>())
            itemHolding.GetComponent<Rigidbody2D>().simulated = true;

        itemHolding = null;
    }

    private void TryPickupItem()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(pickableTag))
        {
            float distance = Vector2.Distance(transform.position, hit.collider.transform.position);

            if (distance <= maxPickupDistance)
            {
                itemHolding = hit.collider.gameObject;
                itemHolding.transform.position = holdingPoint.position;
                itemHolding.transform.parent = transform;

                if (itemHolding.GetComponent<Rigidbody2D>())
                    itemHolding.GetComponent<Rigidbody2D>().simulated = false;
            }
        }
    }
}
