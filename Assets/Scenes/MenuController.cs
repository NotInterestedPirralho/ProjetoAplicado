using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Fun��o chamada ao clicar no bot�o "Jogar"
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");  // Substitua "MainScene" pelo nome correto da sua cena do jogo
    }

    // Fun��o chamada ao clicar no bot�o "Sair"
    public void ExitGame()
    {
        Application.Quit();  // Fecha o jogo
    }
}