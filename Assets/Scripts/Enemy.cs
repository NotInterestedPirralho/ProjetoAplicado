using UnityEngine;
using System; // necessário para o evento Action

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Atributos")]
    public int healthMaximo = 100;
    public int health;
    public int dano = 10;

    [Header("Referências")]
    private EnemyHealthUI barraVida;
    Animator anim;
    bool isDead;
    int idHit, idIsDead;

    // ?? Evento de morte — o DeathManager vai ouvir isto
    public event Action Died;

    void Start()
    {
        health = healthMaximo;
        anim = GetComponent<Animator>();
        idHit = Animator.StringToHash("Hit");
        idIsDead = Animator.StringToHash("IsDead");

<<<<<<< HEAD
=======
        // tenta obter o script da barra, se estiver como filho
>>>>>>> e03aad9210e9192e5d8cd4ae34c771a0ce47c922
        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida != null) barraVida.enemy = this;
    }

    public void TakeDamage(int amount)
    {
<<<<<<< HEAD
        if (isDead) return;
=======
        if (health <= 0) return; // já está morto
>>>>>>> e03aad9210e9192e5d8cd4ae34c771a0ce47c922

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
<<<<<<< HEAD
        if (isDead) return;
        isDead = true;

        if (barraVida != null) barraVida.InimigoMorreu();

        if (anim) anim.SetBool(idIsDead, true);

        // Opcional: desativar colisï¿½es/AI imediatamente
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        var ai = GetComponent<OrcController2D>();
        if (ai) ai.enabled = false;

        // Destruir depois do clip (ajusta 1.0 ao tamanho do teu Death)
        Destroy(gameObject, 1.0f);
=======
        // notifica a barra de vida
        if (barraVida != null)
            barraVida.InimigoMorreu();

        // ?? avisa o DeathManager
        Died?.Invoke();
        
        // opcional: destruir o inimigo após pequeno atraso
        Destroy(gameObject, 0.1f);
>>>>>>> e03aad9210e9192e5d8cd4ae34c771a0ce47c922
    }
}
