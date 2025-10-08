using UnityEngine;

public class Player : MonoBehaviour, IResettableHealth   // <- implementa a interface para Respawn opcional
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    private bool defendendo;

    [Header("Refs")]
    [SerializeField] private DeathManager deathManager;   // <- arrasta no Inspector (ou é encontrado no Start)

    void Start()
    {
        vidaAtual = vidaMaxima;
        if (deathManager == null) deathManager = FindObjectOfType<DeathManager>(); // fallback
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

        // Mostra a Death Screen e pausa o jogo
        if (deathManager == null) deathManager = FindObjectOfType<DeathManager>();
        deathManager?.ShowDeathScreen();

        // Desativa o jogador sem destruir
        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d) { rb2d.velocity = Vector2.zero; rb2d.angularVelocity = 0f; rb2d.simulated = false; }

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;
    }

    // =====================
    // Respawn support (se usares DeathManager.Respawn)
    // =====================
    public void ResetHealth()
    {
        vidaAtual = vidaMaxima;

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d) rb2d.simulated = true;

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = true;
    }

    // =====================
    // Controle de estados
    // =====================
    public void SetDefendendo(bool estado) => defendendo = estado;
    public bool EstaDefendendo() => defendendo;
    public int GetVidaAtual() => vidaAtual;

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
