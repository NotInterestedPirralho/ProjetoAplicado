using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    private bool defendendo;

    // ---- Animator / params ----
    Animator anim;
    int hashHit;        // Trigger "Hit"
    int hashIsDead;     // Bool "IsDead" (opcional)
    bool hasHit, hasIsDead;

    // pequeno “cooldown” para não spammar o Hit
    float hitCooldown = 0.15f;
    float lastHitTime = -999f;

    void Start()
    {
        vidaAtual = vidaMaxima;

        anim = GetComponent<Animator>();
        if (anim)
        {
            hashHit    = Animator.StringToHash("Hit");
            hashIsDead = Animator.StringToHash("IsDead");

            // detetar de forma segura se os parâmetros existem no controller
            foreach (var p in anim.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == hashHit) hasHit = true;
                if (p.type == AnimatorControllerParameterType.Bool    && p.nameHash == hashIsDead) hasIsDead = true;
            }
        }
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

        // Dispara a animação de hit (se existir) com um pequeno cooldown
        if (hasHit && Time.time - lastHitTime >= hitCooldown)
        {
            anim.SetTrigger(hashHit);
            lastHitTime = Time.time;
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

        // informa o Animator que está morto (se existir o parâmetro)
        if (hasIsDead) anim.SetBool(hashIsDead, true);

        // chama o controlador para tocar a animação de morte
        var controller = GetComponent<PlayerController2D>();
        if (controller != null)
            controller.Die();
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
            TomarDano(5); // perde 10 de vida ao encostar no inimigo
        }
    } 
}
