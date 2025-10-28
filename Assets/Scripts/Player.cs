using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    [Tooltip("Se TRUE, o player leva dano ao encostar no inimigo. Normalmente deixa FALSE.")]
    public bool danoPorToque = false;

    private bool defendendo;

    // Animator / params
    private Animator anim;
    private int hashHit;
    private int hashIsDead;
    private bool hasHit, hasIsDead;

    // Referência ao controlador de movimento/combate normal
    private PlayerController2D controller2D;

    // cooldown para não spammar o Hit
    private float hitCooldown = 0.15f;
    private float lastHitTime = -999f;

    void Start()
    {
        vidaAtual = vidaMaxima;

        anim = GetComponent<Animator>();
        controller2D = GetComponent<PlayerController2D>();

        if (anim)
        {
            hashHit = Animator.StringToHash("Hit");
            hashIsDead = Animator.StringToHash("IsDead");

            // detetar de forma segura se os parâmetros existem no controller
            foreach (var p in anim.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger &&
                    p.nameHash == hashHit)
                {
                    hasHit = true;
                }

                if (p.type == AnimatorControllerParameterType.Bool &&
                    p.nameHash == hashIsDead)
                {
                    hasIsDead = true;
                }
            }
        }
    }

    // =====================
    // Vida / Dano
    // =====================
    public void TomarDano(int dano)
    {
        if (defendendo)
        {
            Debug.Log("Defendeu o ataque!");
            return;
        }

        // toca animação de hit se existir e se cooldown permitir
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

        // avisa o Animator que morreu (se existir bool IsDead)
        if (hasIsDead)
        {
            anim.SetBool(hashIsDead, true);
        }

        // chama lógica de morrer do controller
        if (controller2D != null)
        {
            controller2D.Die();
        }
    }

    // =====================
    // Estados
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
    // Colisão com inimigos (só se quiseres dano por toque)
    // =====================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!danoPorToque) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TomarDano(5);
        }
    }
}
