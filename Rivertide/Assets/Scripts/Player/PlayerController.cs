using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Rigidbody2D rb;
    private float moveInput;
    private Animator animator;
    private SpriteRenderer sr;

    [Header("Interact UI")]
    public TMP_Text interactText;

    private IInteractable currentInteractable;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (interactText != null) interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        moveInput = 0f;

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

        if (moveInput != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
        
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
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
