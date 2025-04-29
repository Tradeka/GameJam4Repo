using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Image forceGaugeFill;

    [Header("Variables")]
    [SerializeField][Range(0f, 100f)] private float speed;
    [SerializeField][Range(0f, 100f)] private float minJumpStrength;
    [SerializeField][Range(0f, 100f)] private float maxJumpStrength;
    [SerializeField][Range(0f, 10f)] private float maxJumpHold;

    private float jumpStartTime;
    private bool canJump;
    private Vector2 movement;
    private bool isHoldingJump = false;
    private Coroutine resetFillCoroutine;
    private bool canMove;
    private float jumpForce;

    // --- New fields for materials and upgrade ---
    private int collectedMaterials = 0;
    [SerializeField][Range(0f, 10f)] public int materialsForUpgrade = 10;
    private bool jumpUpgraded = false;

    //New fields for trajectory indicator
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private int numberOfPoints;

    private GameObject[] points;


    private void Awake()
    {
        canMove = true;
        canJump = false;
        rb = GetComponent<Rigidbody2D>();

        //Initialize point array
        points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(pointPrefab, transform.position, Quaternion.identity);
            points[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        // Apply X movement
        if (canMove)
        {
            rb.linearVelocityX = movement.x * speed;
        }
    }

    private void Update()
    {
        if (isHoldingJump && canJump)
        {
            float heldTime = Time.time - jumpStartTime;
            float fillAmount = Mathf.Clamp01(heldTime / maxJumpHold);
            forceGaugeFill.fillAmount = fillAmount;

            float predictedJumpForce = Mathf.Lerp(minJumpStrength, maxJumpStrength, fillAmount);

            for (int i = 0; i < numberOfPoints; i++)
            {
                float t = i * 0.1f; // interval
                Vector2 pointPos = PointPosition(t, predictedJumpForce);
                points[i].transform.position = pointPos;
                points[i].SetActive(true);
            }
        }
        else
        {
            // Hide points when not charging jump
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i].SetActive(false);
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && canJump)
        {
            rb.linearVelocityX = 0;
            canMove = false;
            jumpStartTime = Time.time;
            isHoldingJump = true;

            if (resetFillCoroutine != null)
            {
                StopCoroutine(resetFillCoroutine);
            }
        }

        if (context.canceled && canJump)
        {
            canMove = true;
            float heldTime = Time.time - jumpStartTime;
            float holdPercent = Mathf.Clamp01(heldTime / maxJumpHold);
            float jumpForce = Mathf.Lerp(minJumpStrength, maxJumpStrength, holdPercent);

            rb.linearVelocityY = jumpForce;
            canJump = false;
            isHoldingJump = false;

            resetFillCoroutine = StartCoroutine(ResetFillGauge());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }

    private IEnumerator ResetFillGauge()
    {
        float startFill = forceGaugeFill.fillAmount;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            forceGaugeFill.fillAmount = Mathf.Lerp(startFill, 0f, t);
            yield return null;
        }

        forceGaugeFill.fillAmount = 0f;
    }
    public void CollectMaterial()
    {
        collectedMaterials++;
        Debug.Log("Collected Material! Total: " + collectedMaterials);

        if (collectedMaterials >= materialsForUpgrade && !jumpUpgraded)
        {
            UpgradeJump();
        }
    }

    private void UpgradeJump()
    {
        jumpUpgraded = true;
        maxJumpStrength *= 2f;
        Debug.Log("Jump strength upgraded! New maxJumpStrength: " + maxJumpStrength);
    }

    private Vector2 PointPosition(float t, float jumpForce)
    {
        Vector2 startPos = transform.position;

        float vx = movement.x * 5;
        float vy = jumpForce;

        // physics formula: s = ut + 0.5at^2. half the y of the velocity so that it doesn't stretch out too far 
        Vector2 position = startPos + new Vector2(vx * t, vy * t/2) + 0.5f * Physics2D.gravity * t * t;
        return position;
    }
}
