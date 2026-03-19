using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Rigidbody2D rb;
    public bool canMove = true;
    private float moveInput;
    private Animator animator;
    private SpriteRenderer sr;

    [Header("Interact UI")]
    public TMP_Text interactText;
    private IInteractable currentInteractable;

    // ---------- CAMERA ----------
    [Header("Camera Follow")]
    public Transform cameraTransform;      // Drag Main Camera here
    public float cameraSmoothSpeed = 0.15f;
    private Vector3 cameraOffset;
    // ----------------------------

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (cameraTransform != null)
        {
            cameraOffset = cameraTransform.position - transform.position;
        }
        else
        {
            Debug.LogWarning("PlayerController: Camera Transform not assigned!");
        }
    }

    private void Update()
    {
        moveInput = 0f;

        if (!canMove)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        // Horizontal movement
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            sr.flipX = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            sr.flipX = false;
        }

        animator.SetBool("isMoving", moveInput != 0);

        // Interact
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    // Camera movement AFTER player moves (prevents jitter)
    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 targetPos = transform.position + cameraOffset;
        targetPos.z = cameraTransform.position.z; // keep camera Z fixed

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPos,
            cameraSmoothSpeed
        );
    }

    public void ShowInteract(string text)
    {
        if (interactText == null) return;
        interactText.text = text;
        interactText.gameObject.SetActive(true);
    }

    public void HideInteract()
    {
        if (interactText == null) return;
        interactText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            ShowInteract(interactable.GetInteractText());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            HideInteract();
            currentInteractable = null;
        }
    }
}
