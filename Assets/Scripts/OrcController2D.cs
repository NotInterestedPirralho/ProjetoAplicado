using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class OrcController2D : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float velocidade = 2.5f;

    [Header("Deteção")]
    [SerializeField] float alcanceVisao = 12f;
    [SerializeField] float alcanceAtaque = 1.2f;
    [SerializeField] LayerMask playerMask;

    [Header("Combate")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float raioAtaque = 0.5f;
    [SerializeField] int danoAtaque = 10;
    [SerializeField] float cooldownAtaque = 2f;

    Rigidbody2D rb;
    Animator anim;
    Enemy enemy;

    Transform alvo;
    bool podeAtacar = true;
    bool viradoDireita = true;

    // se está colado ao player
    bool paradoNoPlayer = false;

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
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) alvo = go.transform;
    }

    void Update()
    {
        if (!alvo) return;
        if (!rb.simulated) return;

        float dist = Vector2.Distance(transform.position, alvo.position);

        if (dist > alcanceVisao)
        {
            paradoNoPlayer = false;
            DescongelarX();
            PararMovimento();
            return;
        }

        if (dist <= alcanceAtaque)
        {
            paradoNoPlayer = true;
            CombateCorpoACorpo();
            return;
        }

        // perseguir
        paradoNoPlayer = false;
        PerseguirJogador();
    }

    // ---------------- PERSEGUIR ----------------
    void PerseguirJogador()
    {
        DescongelarX(); // voltar a deixar mexer no X

        float dir = Mathf.Sign(alvo.position.x - transform.position.x);

        VirarSePreciso(dir);

        rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);

        SetSpeedParam(Mathf.Abs(rb.linearVelocity.x));
    }

    // ---------------- COMBATE ----------------
    void CombateCorpoACorpo()
    {
        // congela X para não empurrar o player nem deslizar
        CongelarX();

        // para totalmente
        rb.linearVelocity = Vector2.zero;
        SetSpeedParam(0f);

        VirarParaAlvo();

        if (podeAtacar)
        {
            StartCoroutine(FazerAtaque());
        }
    }

    IEnumerator FazerAtaque()
    {
        podeAtacar = false;

        if (anim && TemParametro(anim, idAttack, AnimatorControllerParameterType.Trigger))
        {
            anim.SetTrigger(idAttack);
        }

        yield return new WaitForSeconds(0.15f);

        if (attackPoint)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, raioAtaque, playerMask);
            foreach (var h in hits)
            {
                Player p = h.GetComponent<Player>();
                if (p != null)
                {
                    p.TomarDano(danoAtaque);
                }
            }
        }

        yield return new WaitForSeconds(cooldownAtaque);

        podeAtacar = true;
    }

    // ---------------- HELPERS ----------------
    void PararMovimento()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        SetSpeedParam(0f);
    }

    void VirarParaAlvo()
    {
        if (!alvo) return;
        float dir = Mathf.Sign(alvo.position.x - transform.position.x);
        VirarSePreciso(dir);
    }

    void VirarSePreciso(float dir)
    {
        if (dir > 0 && !viradoDireita) Flip();
        else if (dir < 0 && viradoDireita) Flip();
    }

    void Flip()
    {
        viradoDireita = !viradoDireita;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void SetSpeedParam(float s)
    {
        if (!anim) return;
        if (TemParametro(anim, idSpeed, AnimatorControllerParameterType.Float))
            anim.SetFloat(idSpeed, s);
    }

    bool TemParametro(Animator a, int hash, AnimatorControllerParameterType type)
    {
        foreach (var p in a.parameters)
        {
            if (p.type == type && p.nameHash == hash)
                return true;
        }
        return false;
    }

    // ---- CONGELAR / DESCONGELAR X NO RIGIDBODY ----
    void CongelarX()
    {
        // Mantém Y livre (gravidade) mas trava X
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void DescongelarX()
    {
        // Permite voltar a andar no X
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, raioAtaque);
        }
    }
}
