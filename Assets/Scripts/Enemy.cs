using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int healthMaximo = 100;
    public int health = 100;
    public int dano = 5;

    [Header("Contacto")]
    [Tooltip("Se TRUE, o inimigo causa dano ao Player quando encosta.")]
    public bool causarDanoPorContacto = true;

    EnemyHealthUI barraVida;
    Animator anim;
    bool isDead;

    // anti-spam (não dar múltiplos hits no mesmo frame de contacto)
    [SerializeField] float hitStun = 0.05f;
    float lastHitTime = -999f;

    void Start()
    {
        anim = GetComponent<Animator>();
        health = Mathf.Clamp(health, 0, healthMaximo);

        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida) barraVida.enemy = this;
    }

    // ---------- Dano Recebido ----------
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

        // Toca "Hit" se existir
        if (anim)
        {
            foreach (var p in anim.parameters)
                if (p.type == AnimatorControllerParameterType.Trigger && p.name == "Hit")
                { anim.SetTrigger("Hit"); break; }
        }
    }

    // ---------- Contacto com Player (dano por encostar) ----------
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

    // ---------- Morte ----------
    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (barraVida) barraVida.InimigoMorreu();

        // desligar AI & física imediatamente
        var ai = GetComponent<OrcController2D>();
        if (ai) ai.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        // avisa o spawner para preparar o próximo
        EnemySpawner.Instance?.OnEnemyDied();

        // desaparece já
        Destroy(gameObject);
    }

}
