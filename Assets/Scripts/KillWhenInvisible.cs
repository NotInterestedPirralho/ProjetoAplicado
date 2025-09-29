using UnityEngine;
using System.Collections;

public class KillWhenInvisible : MonoBehaviour
{
    private Enemy enemy;
    private Player player;

    private Coroutine morrerCoroutine;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = GetComponent<Player>();
    }

    void OnBecameInvisible()
    {
        // inicia a contagem de 2s para morrer
        if (morrerCoroutine == null)
            morrerCoroutine = StartCoroutine(MorrerDepoisDeTempo(1f));
    }

    void OnBecameVisible()
    {
        // cancela se voltou para a tela
        if (morrerCoroutine != null)
        {
            StopCoroutine(morrerCoroutine);
            morrerCoroutine = null;
        }
    }

    private IEnumerator MorrerDepoisDeTempo(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        if (enemy != null)
        {
            enemy.TakeDamage(enemy.healthMaximo); // mata o inimigo
        }

        if (player != null)
        {
            player.TomarDano(player.vidaMaxima); // mata o player
        }
    }
}
