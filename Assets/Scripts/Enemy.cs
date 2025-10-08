using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int healthMaximo = 100;
    public int health;
    public int dano = 10;

    private EnemyHealthUI barraVida;

    void Start()
    {
        health = healthMaximo;

        // tenta pegar o script da barra se estiver como child
        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida != null)
        {
            barraVida.enemy = this;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Notifica a barra que o inimigo morreu
        if (barraVida != null)
        {
            barraVida.InimigoMorreu();
        }

        // destrói o inimigo após alguns segundos (opcional)
        Destroy(gameObject, 0.1f);
    }
}
