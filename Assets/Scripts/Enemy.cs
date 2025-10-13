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
<<<<<<< Updated upstream
=======

    // ?? Evento de morte — o DeathManager vai ouvir isto
    public event Action Died;
>>>>>>> Stashed changes

    void Start()
    {
        health = healthMaximo;

<<<<<<< Updated upstream
        // tenta pegar o script da barra se estiver como child
=======
        // tenta obter o script da barra, se estiver como filho
>>>>>>> Stashed changes
        barraVida = GetComponentInChildren<EnemyHealthUI>();
        if (barraVida != null)
        {
            barraVida.enemy = this;
        }
    }

    public void TakeDamage(int amount)
    {
<<<<<<< Updated upstream
=======
        if (health <= 0) return; // já está morto

>>>>>>> Stashed changes
        health -= amount;
        if (health <= 0)
        {
            Die();
<<<<<<< Updated upstream
        }
    }

    private void Die()
    {
        // Notifica a barra que o inimigo morreu
        if (barraVida != null)
        {
            barraVida.InimigoMorreu();
        }

        // mostra a win screen
        var deathManager = FindObjectOfType<DeathManager>();
        if (deathManager != null)
        {
            deathManager.ShowWinScreen();
        }

        // destrói o inimigo após alguns segundos (opcional)
=======
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
>>>>>>> Stashed changes
        Destroy(gameObject, 0.1f);
    }
}
