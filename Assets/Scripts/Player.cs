using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    private bool defendendo;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    // =====================
    // Vida
    // =====================
    public void TomarDano(int dano)
    {
        if (defendendo)
        {
            Debug.Log("Defendeu o ataque!");
            return;
        }

        vidaAtual -= dano;
        Debug.Log("Vida do Player: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        Debug.Log("Player morreu!");

        // 👉 Em vez de destruir o jogador, chama a animação de morte:
        var controller = GetComponent<PlayerController2D>();
        if (controller != null)
            controller.Die();

        // ❌ Não destróis o objeto, o Animator faz a animação de morte.
        // Destroy(gameObject); <-- removido
    }

    // =====================
    // Controle de estados
    // =====================
    public void SetDefendendo(bool estado)
    {
        defendendo = estado;
    }

    public bool EstaDefendendo()
    {
        return defendendo;
    }

    public int GetVidaAtual()
    {
        return vidaAtual;
    }

    // =====================
    // Colisão com inimigos
    // =====================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TomarDano(10); // perde 10 de vida ao encostar no inimigo
        }
    }
}
