using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int healthMaximo = 100;
    public int health = 100;
    public int dano = 10;

    [Header("Contacto")]
    [Tooltip("Se TRUE, o inimigo causa dano ao Player quando encosta.")]
    public bool causarDanoPorContacto = false; // deixa FALSE enquanto testas

    private EnemyHealthUI barraVida;
    private Animator anim;
    private bool isDead;
    private bool reportedDeath;
    [SerializeField] private float hitStun = 0.05f;
    private float lastHitTime = -999f;

    // <-- Adicionámos isto
    private DeathManager deathManager;

    void Start()
    {
        anim = GetComponent<Animator>();
        health = Mathf.Clamp(health, 0, healthMaximo);

        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida) barraVida.enemy = this;

        // vamos buscar o DeathManager da cena
        deathManager = FindFirstObjectByType<DeathManager>();

        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.NotifySpawned();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        if (Time.time - lastHitTime < hitStun) return;
        lastHitTime = Time.time;

        health = Mathf.Max(0, health - amount);
        if (health <= 0)
        {
            Die();
            return;
        }

        if (anim)
        {
            foreach (var p in anim.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger && p.name == "Hit")
                {
                    anim.SetTrigger("Hit");
                    break;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!causarDanoPorContacto) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            var p = collision.gameObject.GetComponent<Player>();
            if (p != null) p.TomarDano(dano);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!causarDanoPorContacto) return;
        if (other.CompareTag("Player"))
        {
            var p = other.GetComponent<Player>();
            if (p != null) p.TomarDano(dano);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (barraVida) barraVida.InimigoMorreu();

        // desliga IA para ele parar
        var ai = GetComponent<OrcController2D>();
        if (ai) ai.enabled = false;

        // desliga física
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        // diz ao jogo que ganhaste
        if (deathManager != null)
        {
            deathManager.ShowWinScreen();
        }
        else
        {
            Debug.LogWarning("DeathManager não encontrado, não consigo mostrar ecrã de vitória!");
        }

        ReportDeathOnce();

       
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        ReportDeathOnce();
    }

    void ReportDeathOnce()
    {
        if (reportedDeath) return;
        reportedDeath = true;

        if (EnemySpawner.Instance != null)
            EnemySpawner.Instance.NotifyDeath();
    }
}
