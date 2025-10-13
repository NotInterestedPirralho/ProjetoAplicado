using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public int healthMaximo = 100;
    public int health;
    public int dano = 10;

    private EnemyHealthUI barraVida;
    Animator anim;
    bool isDead;
    int idHit, idIsDead;

    void Start()
    {
        health = healthMaximo;
        anim = GetComponent<Animator>();
        idHit = Animator.StringToHash("Hit");
        idIsDead = Animator.StringToHash("IsDead");

        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida != null) barraVida.enemy = this;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Die();
            return;
        }

        if (anim) anim.SetTrigger(idHit);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (barraVida != null) barraVida.InimigoMorreu();

        if (anim) anim.SetBool(idIsDead, true);

        // Opcional: desativar colis�es/AI imediatamente
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        var ai = GetComponent<OrcController2D>();
        if (ai) ai.enabled = false;

        // Destruir depois do clip (ajusta 1.0 ao tamanho do teu Death)
        Destroy(gameObject, 1.0f);
    }
}
