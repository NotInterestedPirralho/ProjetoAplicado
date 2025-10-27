using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class OrcController2D : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float velocidade = 2.2f;
    [SerializeField] float forcaPulo = 7f;

    [Header("Deteção do Player")]
    [SerializeField] float alcanceVisao = 8f;      // raio para começar a perseguir
    [SerializeField] float alcanceAtaque = 1.2f;   // raio para atacar
    [SerializeField] LayerMask playerMask;
    Transform alvo;

    [Header("Ground / Obstáculos")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform groundCheck;   // no pé
    [SerializeField] Transform frenteCheck;   // à frente, baixo
    [SerializeField] float rayChaoDist = 0.4f;

    [Header("Ataque")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float raioHit = 0.4f;
    [SerializeField] int danoAtaque = 10;
    [SerializeField] float cooldownAtaque = 2f;
    bool podeAtacar = true;
    bool viradoDireita = true;
    bool aAtacar = false;

    Rigidbody2D rb;
    Animator anim;
    Enemy enemy;
    int idSpeed;
    int idAttack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        idSpeed = Animator.StringToHash("Speed");
        idAttack = Animator.StringToHash("Attack");
    }

    void Start()
    {
        ProcurarPlayerAtivo();
    }

    void Update()
    {
        if (!rb.simulated) return;

        if (!alvo || !alvo.gameObject.activeInHierarchy)
            ProcurarPlayerAtivo();

        if (!alvo) return;

        float dist = Vector2.Distance(transform.position, alvo.position);
        float diffAltura = alvo.position.y - transform.position.y;

        if (dist <= alcanceAtaque)
        {
            // dentro da zona de ataque: parar e atacar
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // virar para o player
            float dir = Mathf.Sign(alvo.position.x - transform.position.x);
            if (dir > 0 && !viradoDireita) Flip();
            else if (dir < 0 && viradoDireita) Flip();

            // iniciar ataque cíclico
            if (!aAtacar)
            {
                aAtacar = true;
                StartCoroutine(LoopAtaque());
            }
        }
        else if (dist <= alcanceVisao)
        {
            // fora da range de ataque mas dentro da visão → perseguir
            aAtacar = false;
            float dir = Mathf.Sign(alvo.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);

            if (dir > 0 && !viradoDireita) Flip();
            else if (dir < 0 && viradoDireita) Flip();

            // só salta se não houver chão à frente e o player estiver acima
            if (SemChaoAFrente() && NoChao() && diffAltura > 0.6f && !PlayerPorCima())
                TentarSaltar();
        }
        else
        {
            // fora da visão → parado
            aAtacar = false;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        AtualizarAnimSpeed();
    }

    IEnumerator LoopAtaque()
    {
        while (aAtacar)
        {
            if (podeAtacar)
            {
                podeAtacar = false;

                if (anim) anim.SetTrigger(idAttack);
                yield return new WaitForSeconds(0.25f); // tempo até o golpe acertar

                if (attackPoint)
                {
                    Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, raioHit, playerMask);
                    foreach (var h in hits)
                    {
                        var p = h.GetComponent<Player>();
                        if (p != null)
                            p.TomarDano(danoAtaque);
                    }
                }

                // espera o cooldown antes do próximo ataque
                yield return new WaitForSeconds(cooldownAtaque);
                podeAtacar = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    // ===========================
    // AUXILIARES
    // ===========================
    bool NoChao()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
    }

    bool SemChaoAFrente()
    {
        if (!frenteCheck) return false;
        RaycastHit2D chaoFrente = Physics2D.Raycast(frenteCheck.position, Vector2.down, rayChaoDist, groundMask);
        return chaoFrente.collider == null;
    }

    bool PlayerPorCima()
    {
        // Verifica se há player logo acima do orc para evitar saltos loucos
        Vector2 origem = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origem, Vector2.up, 1.5f, playerMask);
        return hit.collider != null;
    }

    void TentarSaltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
    }

    void ProcurarPlayerAtivo()
    {
        var jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (var j in jogadores)
        {
            if (j.activeInHierarchy)
            {
                alvo = j.transform;
                break;
            }
        }
    }

    void AtualizarAnimSpeed()
    {
        if (anim) anim.SetFloat(idSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    void Flip()
    {
        viradoDireita = !viradoDireita;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, raioHit);
        }

        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }

        if (frenteCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(frenteCheck.position, frenteCheck.position + Vector3.down * rayChaoDist);
        }

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, alcanceVisao);

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
