using UnityEngine;
using UnityEngine.UI; // necessário para usar Image

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthFill;   // arraste a imagem da barra (Fill) aqui no Inspector
    public Player player;      // referência ao Player

    void Start()
    {
        AtualizarBarra();
    }

    void Update()
    {
        AtualizarBarra();
    }

    void AtualizarBarra()
    {
        // fillAmount espera um valor entre 0 e 1
        healthFill.fillAmount = (float)player.GetVidaAtual() / player.vidaMaxima;
    }
}
