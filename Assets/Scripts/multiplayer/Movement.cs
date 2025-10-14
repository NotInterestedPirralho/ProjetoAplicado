using UnityEngine;
using UnityEngine.InputSystem; // Importante: novo sistema de input

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 4f;
    public float maxVelocityChange = 10f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // impede que caia
        rb.freezeRotation = true; // evita que gire ao colidir
    }

    // Chamado automaticamente pelo novo Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        Vector2 targetVelocity = moveInput * walkSpeed;
        Vector2 velocity = rb.linearVelocity;

        // Só aplica força se houver input
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);

            rb.AddForce(velocityChange, ForceMode2D.Impulse);
        }
    }
}
