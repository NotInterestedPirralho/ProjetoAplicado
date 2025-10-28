using UnityEngine;
using System.Collections; // ← ADICIONA ESTA LINHA

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Die
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class EnemyStateMachine : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float velocidade = 2.5f;
    public float alcanceVisao = 10f;
    public float alcanceAtaque = 1.5f;
    public float cooldownAtaque = 2f;
    public int danoAtaque = 10;
    public Transform attackPoint;
    public LayerMask playerMask;

    [Header("Patrulha")]
    public float distanciaPatrulha = 3f;
    private Vector2 pontoInicial;
    private int direcao = 1;

    private EnemyState estadoAtual = EnemyState.Idle;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private bool podeAtacar = true;
    private bool viradoDireita = true;
    private Enemy enemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        pontoInicial = transform.position;
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go) player = go.transform;
        MudarEstado(EnemyState.Idle);
    }

    void Update()
    {
        if (!player || !rb.simulated) return;

        switch (estadoAtual)
        {
            case EnemyState.Idle:
                EstadoIdle();
                break;
            case EnemyState.Patrol:
                EstadoPatrol();
                break;
            case EnemyState.Chase:
                EstadoChase();
                break;
            case EnemyState.Attack:
                EstadoAttack();
                break;
            case EnemyState.Die:
                EstadoDie();
                break;
        }
    }

    // =======================
    // ESTADOS
    // =======================
    void EstadoIdle()
    {
        anim.SetFloat("Speed", 0f);

        // Se vir player dentro da visão → muda para Chase
        if (PlayerVisivel())
            MudarEstado(EnemyState.Chase);
        else
            MudarEstado(EnemyState.Patrol);
    }

    void EstadoPatrol()
    {
        anim.SetFloat("Speed", 1f);

        // anda para a esquerda e direita
        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        // muda direção ao fim do alcance da patrulha
        if (Mathf.Abs(transform.position.x - pontoInicial.x) > distanciaPatrulha)
        {
            direcao *= -1;
            Flip();
        }

        if (PlayerVisivel())
            MudarEstado(EnemyState.Chase);
    }

    void EstadoChase()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        anim.SetFloat("Speed", Mathf.Abs(velocidade));

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);
        VirarParaJogador();

        if (dist <= alcanceAtaque)
        {
            MudarEstado(EnemyState.Attack);
        }
        else if (dist > alcanceVisao)
        {
            MudarEstado(EnemyState.Idle);
        }
    }

    void EstadoAttack()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);
        VirarParaJogador();

        if (podeAtacar)
            StartCoroutine(Atacar());
    }

    void EstadoDie()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Die");
        rb.simulated = false;
        Destroy(gameObject, 2f);
    }

    // =======================
    // AÇÕES / FUNÇÕES
    // =======================
    IEnumerator Atacar()
    {
        podeAtacar = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.2f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, 0.4f, playerMask);
        foreach (var h in hits)
        {
            Player p = h.GetComponent<Player>();
            if (p != null)
                p.TomarDano(danoAtaque);
        }

        yield return new WaitForSeconds(cooldownAtaque);
        podeAtacar = true;

        // Volta a perseguir se o jogador se afastar
        if (Vector2.Distance(transform.position, player.position) > alcanceAtaque)
            MudarEstado(EnemyState.Chase);
    }

    void MudarEstado(EnemyState novo)
    {
        estadoAtual = novo;
    }

    bool PlayerVisivel()
    {
        return Vector2.Distance(transform.position, player.position) < alcanceVisao;
    }

    void VirarParaJogador()
    {
        if (!player) return;
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (dir > 0 && !viradoDireita) Flip();
        else if (dir < 0 && viradoDireita) Flip();
    }

    void Flip()
    {
        viradoDireita = !viradoDireita;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.4f);
        }
    }
}
