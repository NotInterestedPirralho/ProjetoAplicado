using UnityEngine;
using UnityEngine.InputSystem; // Novo Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
<<<<<<< Updated upstream
    public float velocidade = 5f;
    public float forcaPulo = 7f;

    [Header("Combate")]
    public float duracaoAtaque = 0.3f; // duração do ataque
    public float alcanceAtaque = 1f;   // raio de alcance do ataque
    public int danoAtaque = 10;        // dano aplicado
    private bool atacando = false;
=======
    [SerializeField] float velocidade = 5f;
    [SerializeField] float forcaPulo = 10f;   // aumenta um pouco o impulso

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;    // arrasta no Inspector (ponto nos pés)
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] LayerMask groundMask;     // Layer = Ground

    [Header("Combate")]
    [SerializeField] float duracaoAtaque = 0.3f;
    [SerializeField] float alcanceAtaque = 1f;
    [SerializeField] int danoAtaque = 10;
    [SerializeField] LayerMask inimigoMask;    // Layer = Enemy
>>>>>>> Stashed changes

    [Header("Pulo")]
    public int maxPulos = 2; // máximo de pulos permitidos
    private int pulosRestantes;

    private Rigidbody2D rb;
    private Player player; // referência ao script Player
    private Vector2 movimento;
    private bool pular;

<<<<<<< Updated upstream
    void Start()
    {
=======
    Vector2 moveInput;
    bool jumpQueued;
    bool jumpHeld;
    int pulosRestantes;
    bool atacando;
    bool facingRight = true;

    void Awake(){
>>>>>>> Stashed changes
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        pulosRestantes = maxPulos; // começa podendo pular 2x
    }

    void Update()
    {
        LerMovimento();
        LerPulo();
        LerAcoes();
    }

    void FixedUpdate()
    {
        AplicarMovimento();
        AplicarPulo();
    }

    // =====================
    // Movimento
    // =====================
    void LerMovimento()
    {
        movimento = Vector2.zero;

<<<<<<< Updated upstream
        if (Keyboard.current.aKey.isPressed)
            movimento.x = -1;
        if (Keyboard.current.dKey.isPressed)
            movimento.x = 1;
    }

    void AplicarMovimento()
    {
        rb.linearVelocity = new Vector2(movimento.x * velocidade, rb.linearVelocity.y);
    }

    // =====================
    // Pulo
    // =====================
    void LerPulo()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && pulosRestantes > 0)
            pular = true;
    }

    void AplicarPulo()
    {
        if (pular)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            pulosRestantes--; // gasta um pulo
            pular = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pulosRestantes = maxPulos;
        }
    }

    // =====================
    // Ataque, Defesa e Interação
    // =====================
    void LerAcoes()
    {
        // Ataque com o botão esquerdo do mouse
        if (Mouse.current.leftButton.wasPressedThisFrame && !atacando)
            StartCoroutine(Atacar());

        // Defesa com o botão direito do mouse
        player.SetDefendendo(Mouse.current.rightButton.isPressed);

        // Interação com a tecla E
        if (Keyboard.current.eKey.wasPressedThisFrame)
            Interagir();
    }

    System.Collections.IEnumerator Atacar()
    {
        atacando = true;
        Debug.Log("Atacando!");

        // Detecta inimigos dentro do alcance
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque);
        foreach (Collider2D inimigo in inimigos)
        {
            Enemy e = inimigo.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(danoAtaque); // aplica dano
            }
=======
        // pulo (permite 1..maxPulos conforme configuração)
        if (jumpQueued && pulosRestantes > 0){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            pulosRestantes--;
            jumpQueued = false;
        }

        // salto variável (corta a ascensão quando soltas a tecla)
        if (!jumpHeld && rb.linearVelocity.y > 0f){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
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
            pulosRestantes = maxPulos; // não forces a 2; usa o valor do Inspector
    }

    void Flip(){
        facingRight = !facingRight;
        var s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    // ------------------ INPUT ACTIONS (PlayerInput -> Send Messages) ------------------
    public void OnMove(InputValue v){
        moveInput = v.Get<Vector2>();
    }

    public void OnJump(InputValue v){
        if (v.isPressed){ jumpQueued = true; jumpHeld = true; }
        else            { jumpHeld   = false; }
    }

    public void OnAttack(InputValue v){
        Debug.Log("OnAttack: " + v.isPressed);
        if (!v.isPressed || atacando) return;
        StartCoroutine(Atacar());
        anim.SetTrigger("Attack"); // precisa do Trigger "Attack" no Animator
    }

    public void OnDefend(InputValue v){
        Debug.Log("OnDefend: " + v.isPressed);
        player.SetDefendendo(v.isPressed);
    }

    public void OnInteract(InputValue v){
        if (v.isPressed) Debug.Log("Interagir");
    }
    // ----------------------------------------------------------------------------------

    System.Collections.IEnumerator Atacar(){
        atacando = true;

        // dano por área simples; garante inimigoMask = Layer Enemy e inimigos com Collider2D
        var hits = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque, inimigoMask);
        foreach (var h in hits){
            var e = h.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(danoAtaque);
>>>>>>> Stashed changes
        }

        yield return new WaitForSeconds(duracaoAtaque);
        atacando = false;
    }

<<<<<<< Updated upstream
    void Interagir()
    {
        Debug.Log("Interagindo!");
    }

    public bool EstaAtacando()
    {
        return atacando;
    }

    // =====================
    // Para visualizar o alcance no Editor
    // =====================
    private void OnDrawGizmosSelected()
    {
=======
    void OnDrawGizmosSelected(){
        if (groundCheck){
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
>>>>>>> Stashed changes
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
