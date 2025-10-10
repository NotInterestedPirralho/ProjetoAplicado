using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel; // painel “You Died”
    [SerializeField] private GameObject winPanel;   // painel “You Win” (opcional)

    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

    // spawns iniciais
    private Vector3 playerStartPos, botStartPos;
    private Quaternion playerStartRot, botStartRot;

    private void Awake()
    {
        // garante que começa tudo escondido e tempo normal
        Time.timeScale = 1f;
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        // guarda posições/rotações iniciais
        if (player)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
        }
        if (bot)
        {
            botStartPos = bot.position;
            botStartRot = bot.rotation;
        }
    }

    // ====== SCREENS ==========================================================
    public void ShowDeathScreen()
    {
        if (deathPanel) deathPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    public void ShowWinScreen()
    {
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    // ====== ACÇÕES DOS BOTÕES ================================================
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex, LoadSceneMode.Single);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    // (Opcional) respawn sem recarregar a cena
    public void Respawn()
    {
        Time.timeScale = 1f;

        // Player
        if (player)
        {
            var rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d) { rb2d.velocity = Vector2.zero; rb2d.angularVelocity = 0f; }
            var rb3d = player.GetComponent<Rigidbody>();
            if (rb3d) { rb3d.velocity = Vector3.zero; rb3d.angularVelocity = Vector3.zero; }

            player.SetPositionAndRotation(playerStartPos, playerStartRot);

            var hp = player.GetComponent<IResettableHealth>();
            if (hp != null) hp.ResetHealth();
        }

        // Bot
        if (bot)
        {
            var rb2d = bot.GetComponent<Rigidbody2D>();
            if (rb2d) { rb2d.velocity = Vector2.zero; rb2d.angularVelocity = 0f; }
            var rb3d = bot.GetComponent<Rigidbody>();
            if (rb3d) { rb3d.velocity = Vector3.zero; rb3d.angularVelocity = Vector3.zero; }

            bot.SetPositionAndRotation(botStartPos, botStartRot);

            var hp = bot.GetComponent<IResettableHealth>();
            if (hp != null) hp.ResetHealth();
            var ai = bot.GetComponent<IResettableAI>();
            if (ai != null) ai.ResetAI();
        }

        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }
}

// Interfaces opcionais (se quiseres resetar vida/IA no Respawn)
public interface IResettableHealth { void ResetHealth(); }
public interface IResettableAI { void ResetAI(); }
