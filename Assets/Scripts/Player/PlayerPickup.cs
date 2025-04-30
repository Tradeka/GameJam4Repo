using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering.Universal;

public class PlayerPickup : MonoBehaviour
{
    public LayerMask pickupLayer;            
    private string heldMaterialName;

    public List<GameObject> pickupItems;
    public TMP_Text numText; 

    private int totalItems = 0;
    private int itemsCollected = 0;

    public AudioSource pickupSound;

    private void Awake()
    {
        totalItems = pickupItems.Count;
    }

    private void Update()
    {
        numText.text = $"{itemsCollected}/{totalItems}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & pickupLayer) != 0)
        {
            itemsCollected++;
            PickupMaterial(collision.gameObject);
            pickupSound.Play();
        }
    }

    void PickupMaterial(GameObject material)
    {
        Light2D pickupLight = material.GetComponentInChildren<Light2D>();
        pickupLight.enabled = false;
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

        FindFirstObjectByType<PlayerMovement>().CollectMaterial();
    }
}
