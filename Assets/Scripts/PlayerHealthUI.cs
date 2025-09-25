using UnityEngine;
using UnityEngine.UI; // necess�rio para usar Slider

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthBar;   // arraste o Slider aqui no Inspector
    public Player player;      // refer�ncia ao Player

    void Start()
    {
        // configura a barra com os valores iniciais
        healthBar.maxValue = player.vidaMaxima;
        healthBar.value = player.GetVidaAtual();
    }

    void Update()
    {
        // atualiza a barra de vida todo frame
        healthBar.value = player.GetVidaAtual();
    }
}
