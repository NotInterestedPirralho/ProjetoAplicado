using UnityEngine;
using UnityEngine.InputSystem; // novo input system

public class PauseManager : MonoBehaviour
{
    public GameObject painelPause;

    private bool jogoPausado = false;

    void Start()
    {
        if (painelPause != null)
            painelPause.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) // Novo Input System
        {
            if (jogoPausado)
                RetomarJogo();
            else
                PausarJogo();
        }
    }

    public void PausarJogo()
    {
        Time.timeScale = 0f;
        jogoPausado = true;
        if (painelPause != null)
            painelPause.SetActive(true);
    }

    public void RetomarJogo()
    {
        Time.timeScale = 1f;
        jogoPausado = false;
        if (painelPause != null)
            painelPause.SetActive(false);
    }

    public void SairDoJogo()
    {
        Debug.Log("Sair do jogo...");
        Application.Quit();
    }
}
