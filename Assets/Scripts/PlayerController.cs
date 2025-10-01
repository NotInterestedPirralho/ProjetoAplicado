using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float velocidade = 5f;
    [SerializeField] float forcaPulo = 7f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;          // arrasta no Inspector
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] LayerMask groundMask;           // escolhe a Layer Ground

    [Header("Combate")]
    [SerializeField] float duracaoAtaque = 0.3f;
    [SerializeField] float alcanceAtaque = 1f;
    [SerializeField] int danoAtaque = 10;
    [SerializeField] LayerMask inimigoMask;          // Layer dos inimigos

    [Header("Pulo")]
    [SerializeField] int maxPulos = 2;

    Rigidbody2D rb;
    Animator anim;
    Player player;

    Vector2 moveInput;
    bool jumpQueued;
    int pulosRestantes;
    bool atacando;
    bool facingRight = true;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    void Start(){
        pulosRestantes = maxPulos;
    }

    void FixedUpdate(){
        // mover
        rb.linearVelocity = new Vector2(moveInput.x * velocidade, rb.linearVelocity.y);

        // virar sprite
        if (moveInput.x > 0.05f && !facingRight) Flip();
        else if (moveInput.x < -0.05f && facingRight) Flip();

        // pulo (NOVA VERSÃO — permite 1 ou mais pulos consoante maxPulos)
        if (jumpQueued && pulosRestantes > 0){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
        pulosRestantes--;
        jumpQueued = false;
        }


        // parâmetros do Animator
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("IsGrounded", IsGrounded());
    }

    bool IsGrounded(){
        return groundCheck
            ? Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask)
            : false;
    }

    void OnCollisionEnter2D(Collision2D c){
    if (c.gameObject.CompareTag("Ground"))
        pulosRestantes = maxPulos = 2;
    }


    void Flip(){
        facingRight = !facingRight;
        var s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    // ------------------ INPUT ACTIONS (PlayerInput -> Send Messages) ------------------
    public void OnMove(InputValue v){ moveInput = v.Get<Vector2>(); }
    public void OnJump(UnityEngine.InputSystem.InputValue v)
    {
    if (v.isPressed) jumpQueued = true;
    }

    public void OnAttack(UnityEngine.InputSystem.InputValue v)
    {
    if (!v.isPressed || atacando) return;
    StartCoroutine(Atacar());
    anim.SetTrigger("Attack");
    }

    public void OnDefend(UnityEngine.InputSystem.InputValue v)
    {
    player.SetDefendendo(v.isPressed);
    }

    public void OnInteract(InputValue v){ if (v.isPressed) Debug.Log("Interagir"); }
    // ----------------------------------------------------------------------------------

    System.Collections.IEnumerator Atacar(){
        atacando = true;
        var hits = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque, inimigoMask);
        foreach (var h in hits){
            var e = h.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(danoAtaque);
        }
        yield return new WaitForSeconds(duracaoAtaque);
        atacando = false;
    }

    void OnDrawGizmosSelected(){
        if (groundCheck) Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
