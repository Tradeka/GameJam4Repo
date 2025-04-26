using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform restartPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null)
        {
            collision.transform.position = restartPoint.position;
        }
    }
}
