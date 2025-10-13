using UnityEngine;
using System; // necessário para o evento Action

public class Enemy : MonoBehaviour
{
    [Header("Atributos")]
    public int healthMaximo = 100;
    public int health;
    public int dano = 10;

    [Header("Referências")]
    private EnemyHealthUI barraVida;

    // ?? Evento de morte — o DeathManager vai ouvir isto
    public event Action Died;

    void Start()
    {
        health = healthMaximo;

        // tenta obter o script da barra, se estiver como filho
        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida != null)
        {
            barraVida.enemy = this;
        }
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return; // já está morto

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        // notifica a barra de vida
        if (barraVida != null)
            barraVida.InimigoMorreu();

        // ?? avisa o DeathManager
        Died?.Invoke();

        // opcional: destruir o inimigo após pequeno atraso
        Destroy(gameObject, 0.1f);
    }
}
