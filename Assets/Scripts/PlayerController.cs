using UnityEngine;
using UnityEngine.InputSystem; // Novo Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 7f;

    [Header("Combate")]
    public float duracaoAtaque = 0.3f; // duração do ataque
    public float alcanceAtaque = 1f;   // raio de alcance do ataque
    public int danoAtaque = 10;        // dano aplicado
    private bool atacando = false;

    [Header("Pulo")]
    public int maxPulos = 2; // máximo de pulos permitidos
    private int pulosRestantes;

    private Rigidbody2D rb;
    private Player player; // referência ao script Player
    private Vector2 movimento;
    private bool pular;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        pulosRestantes = maxPulos; // começa podendo pular 2x
    }

    void Update()
    {
        LerMovimento();
        LerPulo();
        LerAcoes();
    }

    void FixedUpdate()
    {
        AplicarMovimento();
        AplicarPulo();
    }

    // =====================
    // Movimento
    // =====================
    void LerMovimento()
    {
        movimento = Vector2.zero;

        if (Keyboard.current.aKey.isPressed)
            movimento.x = -1;
        if (Keyboard.current.dKey.isPressed)
            movimento.x = 1;
    }

    void AplicarMovimento()
    {
        rb.linearVelocity = new Vector2(movimento.x * velocidade, rb.linearVelocity.y);
    }

    // =====================
    // Pulo
    // =====================
    void LerPulo()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && pulosRestantes > 0)
            pular = true;
    }

    void AplicarPulo()
    {
        if (pular)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            pulosRestantes--; // gasta um pulo
            pular = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            pulosRestantes = maxPulos;
        }
    }

    // =====================
    // Ataque, Defesa e Interação
    // =====================
    void LerAcoes()
    {
        // Ataque com o botão esquerdo do mouse
        if (Mouse.current.leftButton.wasPressedThisFrame && !atacando)
            StartCoroutine(Atacar());

        // Defesa com o botão direito do mouse
        player.SetDefendendo(Mouse.current.rightButton.isPressed);

        // Interação com a tecla E
        if (Keyboard.current.eKey.wasPressedThisFrame)
            Interagir();
    }

    System.Collections.IEnumerator Atacar()
    {
        atacando = true;
        Debug.Log("Atacando!");

        // Detecta inimigos dentro do alcance
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAtaque);
        foreach (Collider2D inimigo in inimigos)
        {
            Enemy e = inimigo.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(danoAtaque); // aplica dano
            }
        }

        yield return new WaitForSeconds(duracaoAtaque);
        atacando = false;
    }

    void Interagir()
    {
        Debug.Log("Interagindo!");
    }

    public bool EstaAtacando()
    {
        return atacando;
    }

    // =====================
    // Para visualizar o alcance no Editor
    // =====================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
    }
}
