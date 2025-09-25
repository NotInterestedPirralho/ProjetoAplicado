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
    private bool atacando = false;

    private Rigidbody2D rb;
    private Player player; // referência ao script Player
    private Vector2 movimento;
    private bool pular;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
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
        if (Keyboard.current.wKey.wasPressedThisFrame)
            pular = true;
    }

    void AplicarPulo()
    {
        if (pular)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            pular = false;
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
        // Aqui você poderia disparar animação, ativar hitbox etc.
        yield return new WaitForSeconds(duracaoAtaque);
        atacando = false;
    }

    void Interagir()
    {
        Debug.Log("Interagindo!");
        // Aqui você colocaria lógica de interação com objetos (ex: abrir porta, pegar item)
    }

    public bool EstaAtacando()
    {
        return atacando;
    }
}
