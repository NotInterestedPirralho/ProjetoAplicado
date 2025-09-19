using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Carrega a cena de escolher personagem
    public void IrParaEscolhaPersonagem()
    {
        SceneManager.LoadScene("TelaEscolhaPersonagem");
    }

    // Fecha o jogo
    public void SairDoJogo()
    {
        Debug.Log("Jogo encerrado!");
        Application.Quit();
    }
}
