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

    [Header("Deteção")]
    [SerializeField] float alcanceVisao = 12f;
    [SerializeField] float alcanceAtaque = 1.2f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask playerMask;

    [Header("Pontos de checagem")]
    [SerializeField] Transform groundCheck;   // empty no pé
    [SerializeField] Transform frenteCheck;   // empty à frente ao nível do pé
    [SerializeField] Transform attackPoint;   // empty à frente à altura da arma

    [Header("Combate")]
    [SerializeField] int danoAtaque = 10;
    [SerializeField] float cooldownAtaque = 0.8f;   // tempo entre ataques

    [Header("Recuo após atacar")]
    [SerializeField] float tempoRetirada = 0.35f;
    [SerializeField] float velocidadeRetirada = 3f;

    [Header("Salto (anti-spam)")]
    [SerializeField] bool permitirSalto = true;     // desliga se não quiseres saltos
    [SerializeField] float intervaloSalto = 0.6f;   // mínimo entre saltos
    [SerializeField] float margemBorda = 0.45f;     // profundidade do raycast para detetar “sem chão”
    [SerializeField] float alcanceParede = 0.35f;   // comprimento do raycast frontal

    Rigidbody2D rb;
    Animator anim;
    Enemy enemy;

    Transform alvo;
    bool podeAtacar = true;
    bool viradoDireita = true;
    bool emRetirada = false;

    float proximoSalto = 0f;

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
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go) alvo = go.transform;
    }

    void Update()
    {
        if (!alvo) return;
        if (!rb.simulated) return; // morreu

        if (emRetirada)
        {
            // durante a retirada, não persegue/ataca; velocidade vem da corrotina
            SetSpeedParam(Mathf.Abs(rb.linearVelocity.x));
            return;
        }

        float dist = Vector2.Distance(transform.position, alvo.position);

        // Fora de visão → Idle
        if (dist > alcanceVisao)
        {
            Parar();
            return;
        }

        // Dentro do alcance de ataque → ataca (com cooldown)
        if (dist <= alcanceAtaque && podeAtacar)
        {
            Parar();
            StartCoroutine(Atacar());
            return;
        }

        // Perseguir
        float dir = Mathf.Sign(alvo.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);
        SetSpeedParam(Mathf.Abs(rb.linearVelocity.x));
        VirarSePrecisar(dir);

        // SALTO SÓ QUANDO PRECISO
        if (permitirSalto && NoChao() && Time.time >= proximoSalto)
        {
            bool paredeAFrente = ObstaculoBaixoAFrente();
            bool beiraSemChao = BordaSemChaoAFrente();

            // Regra: salta se houver parede à frente; ou se houver beira mas o player estiver claramente acima
            bool precisaSaltar = paredeAFrente || (beiraSemChao && (alvo.position.y - transform.position.y) > 0.5f);

            if (precisaSaltar)
            {
                proximoSalto = Time.time + intervaloSalto;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator Atacar()
    {
        podeAtacar = false;
        if (anim) anim.SetTrigger(idAttack);

        // espera para sincronizar com o frame do impacto
        yield return new WaitForSeconds(0.15f);

        // aplicar dano numa área à frente
        if (attackPoint)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 0.4f, playerMask);
            foreach (var h in hits)
            {
                var p = h.GetComponent<Player>();
                if (p != null) p.TomarDano(danoAtaque);
            }
        }

        // recuar depois de atacar
        if (alvo != null)
        {
            emRetirada = true;
            float dirLonge = Mathf.Sign(transform.position.x - alvo.position.x); // para longe do player
            float t = 0f;
            while (t < tempoRetirada)
            {
                rb.linearVelocity = new Vector2(dirLonge * velocidadeRetirada, rb.linearVelocity.y);
                SetSpeedParam(Mathf.Abs(rb.linearVelocity.x));
                t += Time.deltaTime;
                yield return null;
            }
            emRetirada = false;
        }

        // cooldown entre ataques
        yield return new WaitForSeconds(cooldownAtaque);
        podeAtacar = true;
    }

    // --------- Helpers ----------
    void Parar()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        SetSpeedParam(0f);
    }

    void SetSpeedParam(float s)
    {
        if (anim) anim.SetFloat(idSpeed, s);
    }

    void VirarSePrecisar(float dir)
    {
        if (dir > 0 && !viradoDireita) Flip();
        else if (dir < 0 && viradoDireita) Flip();
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
        Vector2 dir = viradoDireita ? Vector2.right : Vector2.left;
        var hit = Physics2D.Raycast(frenteCheck.position, dir, alcanceParede, groundMask);
        return hit.collider != null;
    }

    bool BordaSemChaoAFrente()
    {
        if (!frenteCheck) return false;
        var hit = Physics2D.Raycast(frenteCheck.position, Vector2.down, margemBorda, groundMask);
        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, 0.4f);
        if (groundCheck) Gizmos.DrawWireSphere(groundCheck.position, 0.1f);

        if (frenteCheck)
        {
            Vector3 dir = (viradoDireita ? Vector3.right : Vector3.left) * alcanceParede;
            Gizmos.DrawLine(frenteCheck.position, frenteCheck.position + dir);
            Gizmos.DrawLine(frenteCheck.position, frenteCheck.position + Vector3.down * margemBorda);
        }
    }
}
