using UnityEngine;
using UnityEngine.InputSystem; // Novo Input System
using TMPro; // <<< MUITO IMPORTANTE para usar TextMeshProUGUI

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

    // DEFESA
    [Header("Defesa")]
    public float maxDefendDuration = 3f;   // podes segurar defesa até 3s
    public float defendCooldown = 10f;     // depois tens de esperar 10s
    private bool defending = false;
    private bool canDefend = true;
    private float defendTimer = 0f;
    private float defendCooldownTimer = 0f;

    // UI SHIELD
    [Header("UI")]
    public TextMeshProUGUI shieldText; // arrasta o ShieldText do Canvas aqui no Inspector

    private Rigidbody2D rb;
    private Player player;
    private Animator anim;

    private Vector2 movimento;
    private bool pular;

    private bool facingRight = true;

    // Animator params
    bool hasSpeedParam, hasAttackParam, hasDefendParam, hasDeathParam, hasIsDeadParam;
    bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();

        pulosRestantes = maxPulos;

        hasSpeedParam = HasParam("Speed", AnimatorControllerParameterType.Float);
        hasAttackParam = HasParam("Attack", AnimatorControllerParameterType.Trigger);
        hasDefendParam = HasParam("Defend", AnimatorControllerParameterType.Bool);
        hasDeathParam = HasParam("Death", AnimatorControllerParameterType.Trigger);
        hasIsDeadParam = HasParam("IsDead", AnimatorControllerParameterType.Bool);
    }

    void Update()
    {
        if (isDead) return;

        AtualizarCooldownDefesa();

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
    // COOLDOWN DA DEFESA
    // =====================
    void AtualizarCooldownDefesa()
    {
        if (defending)
        {
            defendTimer += Time.deltaTime;

            // se passou do máximo permitido
            if (defendTimer >= maxDefendDuration)
            {
                PararDefesaEIniciarCooldown();
            }

            // UI enquanto estás a defender (mostra quanto tempo ainda podes segurar)
            AtualizarShieldUI_DefendAtivo();
        }
        else
        {
            if (!canDefend)
            {
                defendCooldownTimer += Time.deltaTime;

                // cooldown acabou?
                if (defendCooldownTimer >= defendCooldown)
                {
                    canDefend = true;
                }

                // UI enquanto estás em cooldown
                AtualizarShieldUI_EmCooldown();
            }
            else
            {
                // defesa pronta de novo
                AtualizarShieldUI_Pronto();
            }
        }
    }

    void PararDefesaEIniciarCooldown()
    {
        defending = false;
        player.SetDefendendo(false);
        if (hasDefendParam) anim.SetBool("Defend", false);

        // começa cooldown
        canDefend = false;
        defendCooldownTimer = 0f;

        defendTimer = 0f;
    }

    // =====================
    // Movimento
    // =====================
    void LerMovimento()
    {
        movimento = Vector2.zero;

        if (defending)
            return;

        if (Keyboard.current.aKey.isPressed)
            movimento.x = -1;
        if (Keyboard.current.dKey.isPressed)
            movimento.x = 1;
    }

    void AplicarMovimento()
    {
        if (defending)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

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
        if (Keyboard.current.wKey.wasPressedThisFrame && pulosRestantes > 0 && !defending)
            pular = true;
    }

    void AplicarPulo()
    {
        if (pular)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            pulosRestantes--;
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
    // Ataque / Defesa / Interação
    // =====================
    void LerAcoes()
    {
        // ATAQUE
        if (Mouse.current.leftButton.wasPressedThisFrame && !atacando && !defending)
            StartCoroutine(Atacar());

        // DEFESA (botão direito)
        bool segurandoDefesa = Mouse.current.rightButton.isPressed;

        if (segurandoDefesa)
        {
            TentarIniciarOuManterDefesa();
        }
        else
        {
            if (defending)
            {
                PararDefesaEIniciarCooldown();
            }
        }

        // Interagir
        if (Keyboard.current.eKey.wasPressedThisFrame && !defending)
            Interagir();
    }

    void TentarIniciarOuManterDefesa()
    {
        if (defending)
        {
            player.SetDefendendo(true);
            if (hasDefendParam) anim.SetBool("Defend", true);
            return;
        }

        if (!canDefend)
            return;

        // começar defesa
        defending = true;
        defendTimer = 0f;
        player.SetDefendendo(true);
        if (hasDefendParam) anim.SetBool("Defend", true);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    System.Collections.IEnumerator Atacar()
    {
        atacando = true;
        if (hasAttackParam) anim.SetTrigger("Attack");

        Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque);
        foreach (Collider2D inimigo in inimigos)
        {
            Enemy e = inimigo.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(danoAtaque);
            }
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
    }
    void OnDeathAnimationEnd()
    {
        Debug.Log("Morte concluída – chamar UI/respawn aqui.");
    }

    // =====================
    // Utils Animator
    // =====================
    bool HasParam(string name, AnimatorControllerParameterType type)
    {
        if (!anim) return false;
        foreach (var p in anim.parameters)
            if (p.type == type && p.name == name)
                return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }

    // =====================
    // UI SHIELD HELPERS
    // =====================
    void AtualizarShieldUI_DefendAtivo()
    {
        if (!shieldText) return;

        float restante = Mathf.Clamp(maxDefendDuration - defendTimer, 0f, maxDefendDuration);
        shieldText.text = $"Shield: {restante:0.0}s";
        shieldText.color = Color.cyan;
    }

    void AtualizarShieldUI_EmCooldown()
    {
        if (!shieldText) return;

        float restante = Mathf.Clamp(defendCooldown - defendCooldownTimer, 0f, defendCooldown);
        shieldText.text = $"Shield CD: {restante:0.0}s";
        shieldText.color = Color.red;
    }

    void AtualizarShieldUI_Pronto()
    {
        if (!shieldText) return;

        shieldText.text = "Shield Ready";
        shieldText.color = Color.green;
    }
}
