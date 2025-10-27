using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject mainMenuPanel;      // arrasta aqui MainMenuPanel
    [SerializeField] private GameObject levelSelectPanel;   // arrasta aqui LevelSelectPanel

    // -------- MENU PRINCIPAL --------

    public void OnPlayPressed()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnMultiplayerPressed()
    {
        Debug.Log("Multiplayer ainda não implementado.");
    }

    public void OnSettingsPressed()
    {
        Debug.Log("Settings ainda não implementado.");
    }

    public void OnExitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // -------- SELECT NÍVEL --------

    public void OnReturnPressed()
    {
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnLevel1Pressed()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnLevel2Pressed()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
