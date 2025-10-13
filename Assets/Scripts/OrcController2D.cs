using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class OrcController2D : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float velocidade = 2.5f;
    [SerializeField] float forcaPulo = 7f;

    [Header("Dete��o")]
    [SerializeField] float alcanceVisao = 12f;
    [SerializeField] float alcanceAtaque = 1.2f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask playerMask;

    [Header("Pontos de checagem")]
    [SerializeField] Transform groundCheck;     // um empty no p� do orc
    [SerializeField] Transform frenteCheck;     // um empty � frente do orc (na altura do p�)
    [SerializeField] Transform attackPoint;     // um empty � frente do orc (na altura da arma)

    [Header("Combate")]
    [SerializeField] int danoAtaque = 10;
    [SerializeField] float cooldownAtaque = 0.6f;

    Rigidbody2D rb;
    Animator anim;
    Enemy enemy;

    Transform alvo;              // Player
    bool podeAtacar = true;
    bool viradoDireita = true;

    // hashes
    int idSpeed, idAttack;

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
        // Encontrar player por Tag
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go) alvo = go.transform;
    }

    void Update()
    {
        if (!alvo) return;
        if (!rb.simulated) return; // caso morra e congeles f�sica

        float dist = Vector2.Distance(transform.position, alvo.position);

        // dentro da vis�o?
        if (dist > alcanceVisao)
        {
            // parado/idle
            SetSpeedParam(0f);
            return;
        }

        // dentro do alcance de ataque?
        if (dist <= alcanceAtaque && podeAtacar)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StartCoroutine(Atacar());
            return;
        }

        // Perseguir
        float dir = Mathf.Sign(alvo.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);
        SetSpeedParam(Mathf.Abs(rb.linearVelocity.x));

        // Virar sprite
        if (dir > 0 && !viradoDireita) Flip();
        else if (dir < 0 && viradoDireita) Flip();

        // Saltar se houver obst�culo baixo � frente ou um pequeno �buraco�
        if (NoChao() && (ObstaculoBaixoAFrente() || BordaSemChaoAFrente()))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
        }
    }

    IEnumerator Atacar()
    {
        podeAtacar = false;
        if (anim) anim.SetTrigger(idAttack);

        // pequena espera para sincronizar com o frame de impacto (ajusta 0.15f)
        yield return new WaitForSeconds(0.15f);

        // dano em �rea � frente
        if (attackPoint)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 0.4f, playerMask);
            foreach (var h in hits)
            {
                var p = h.GetComponent<Player>();
                if (p != null) p.TomarDano(danoAtaque);
            }
        }

        yield return new WaitForSeconds(cooldownAtaque);
        podeAtacar = true;
    }

    // --------- Helpers ----------
    void SetSpeedParam(float s)
    {
        if (anim) anim.SetFloat(idSpeed, s);
    }

    void Flip()
    {
        viradoDireita = !viradoDireita;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    bool NoChao()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
    }

    bool ObstaculoBaixoAFrente()
    {
        if (!frenteCheck) return false;
        // raycast curto � frente para apanhar paredes/blocos
        Vector2 dir = viradoDireita ? Vector2.right : Vector2.left;
        var hit = Physics2D.Raycast(frenteCheck.position, dir, 0.35f, groundMask);
        return hit.collider != null;
    }

    bool BordaSemChaoAFrente()
    {
        if (!frenteCheck) return false;
        // raycast para baixo � frente -> se n�o encontra ch�o, � borda
        var hit = Physics2D.Raycast(frenteCheck.position, Vector2.down, 0.4f, groundMask);
        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, 0.4f);
        if (groundCheck) Gizmos.DrawWireSphere(groundCheck.position, 0.1f);

        if (frenteCheck)
        {
            Gizmos.DrawLine(frenteCheck.position, frenteCheck.position + (viradoDireita ? Vector3.right : Vector3.left) * 0.35f);
            Gizmos.DrawLine(frenteCheck.position, frenteCheck.position + Vector3.down * 0.4f);
        }
    }
}
