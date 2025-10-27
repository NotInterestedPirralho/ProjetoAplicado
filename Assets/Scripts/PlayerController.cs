using UnityEngine;
using UnityEngine.InputSystem; // Novo Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 7f;

    [Header("Combate")]
    public float duracaoAtaque = 0.3f;
    public float alcanceAtaque = 1f;
    public int danoAtaque = 10;
    private bool atacando = false;

    [Header("Pulo")]
    public int maxPulos = 2;
    private int pulosRestantes;

    [Header("UI/Managers")]
    [SerializeField] private DeathManager deathManager; // opcional (fallback com Find)

    private Rigidbody2D rb;
    private Player player;
    private Animator anim;

    private Vector2 movimento;
    private bool pular;

    // -------- Flip ----------
    private bool facingRight = true;

    // Animação / estados
    bool hasSpeedParam, hasAttackParam, hasDefendParam, hasDeathParam, hasIsDeadParam, hasHitParam;
    bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();

        pulosRestantes = maxPulos;

        // Detectar parâmetros existentes no Animator (seguro)
        hasSpeedParam = HasParam("Speed", AnimatorControllerParameterType.Float);
        hasAttackParam = HasParam("Attack", AnimatorControllerParameterType.Trigger);
        hasDefendParam = HasParam("Defend", AnimatorControllerParameterType.Bool);
        hasDeathParam = HasParam("Death", AnimatorControllerParameterType.Trigger);
        hasIsDeadParam = HasParam("IsDead", AnimatorControllerParameterType.Bool);
        hasHitParam = HasParam("Hit", AnimatorControllerParameterType.Trigger);
    }

    void Update()
    {
        if (isDead) return;

        LerMovimento();
        LerPulo();
        LerAcoes();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        AplicarMovimento();
        AplicarPulo();

        if (hasSpeedParam)
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    // =====================
    // Movimento
    // =====================
    void LerMovimento()
    {
        movimento = Vector2.zero;

        if (Keyboard.current.aKey.isPressed) movimento.x = -1;
        if (Keyboard.current.dKey.isPressed) movimento.x = 1;
    }

    void AplicarMovimento()
    {
        rb.linearVelocity = new Vector2(movimento.x * velocidade, rb.linearVelocity.y);

        if (movimento.x > 0f && !facingRight) Flip();
        else if (movimento.x < 0f && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        var t = transform;
        Vector3 s = t.localScale;
        s.x *= -1f;
        t.localScale = s;
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
        if (!pular) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        pulosRestantes--;
        pular = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            pulosRestantes = maxPulos;
    }

    // =====================
    // Ataque, Defesa e Interação
    // =====================
    void LerAcoes()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !atacando)
            StartCoroutine(Atacar());

        bool defending = Mouse.current.rightButton.isPressed;
        player.SetDefendendo(defending);
        if (hasDefendParam) anim.SetBool("Defend", defending);

        if (Keyboard.current.eKey.wasPressedThisFrame)
            Interagir();
    }

    System.Collections.IEnumerator Atacar()
    {
        atacando = true;
        if (hasAttackParam) anim.SetTrigger("Attack");

        Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque);
        foreach (Collider2D inimigo in inimigos)
        {
            Enemy e = inimigo.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(danoAtaque);
        }

        yield return new WaitForSeconds(duracaoAtaque);
        atacando = false;
    }

    void Interagir()
    {
        Debug.Log("Interagindo!");
    }

    public bool EstaAtacando() => atacando;

    // =====================
    // Hit
    // =====================
    public void Hit()
    {
        if (isDead) return;
        if (hasHitParam) anim.SetTrigger("Hit");
    }

    // =====================
    // Morte
    // =====================
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        var input = GetComponent<PlayerInput>();
        if (input) input.enabled = false;

        if (hasIsDeadParam) anim.SetBool("IsDead", true);
        if (hasDeathParam) anim.SetTrigger("Death");
        if (hasSpeedParam) anim.SetFloat("Speed", 0f);

        // >>> chamar o ecrã de DERROTA <<<
        if (deathManager == null)
            deathManager = FindObjectOfType<DeathManager>();
        deathManager?.ShowDeathScreen();
    }

    // Chamado por Animation Event no fim do clip de morte (opcional)
    void OnDeathAnimationEnd()
    {
        Debug.Log("Morte concluída – chamar UI/respawn aqui.");
    }

    // =====================
    // Debug/Editor
    // =====================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }

    // ==== Utilitário: verificar parâmetros do Animator de forma segura ====
    bool HasParam(string name, AnimatorControllerParameterType type)
    {
        if (!anim) return false;
        foreach (var p in anim.parameters)
            if (p.type == type && p.name == name)
                return true;
        return false;
    }
}
