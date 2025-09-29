using UnityEngine;

public class KillWhenInvisible : MonoBehaviour
{
    private Enemy enemy;
    private Player player;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = GetComponent<Player>();
    }

    void OnBecameInvisible()
    {
        if (enemy != null)
            enemy.TakeDamage(enemy.healthMaximo);

        if (player != null)
            player.TomarDano(player.vidaMaxima);
    }
}
