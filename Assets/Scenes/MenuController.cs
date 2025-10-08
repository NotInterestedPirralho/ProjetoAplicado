using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Painéis do Menu")]
    public GameObject mainPanel;
    public GameObject settingsPanel;

    // Função chamada ao clicar no botão "Jogar"
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Função chamada ao clicar no botão "Sair"
    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Abre o painel de definições
    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Volta ao menu principal
    public void BackToMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
