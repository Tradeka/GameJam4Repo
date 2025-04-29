using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public LayerMask pickupLayer;            
    private string heldMaterialName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & pickupLayer) != 0)
        {
            PickupMaterial(collision.gameObject);
        }
    }

    void PickupMaterial(GameObject material)
    {
        heldMaterialName = material.name; // Save the material's name
        Debug.Log("Picked up: " + heldMaterialName);

        // Disable the material's visual and collider
        SpriteRenderer sr = material.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }

        Collider2D col = material.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        FindObjectOfType<PlayerMovement>().CollectMaterial();
    }
}
