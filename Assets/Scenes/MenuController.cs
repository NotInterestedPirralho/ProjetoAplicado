using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Função chamada ao clicar no botão "Jogar"
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");  // Substitua "MainScene" pelo nome correto da sua cena do jogo
    }

    // Função chamada ao clicar no botão "Sair"
    public void ExitGame()
    {
        Application.Quit();  // Fecha o jogo
    }
}