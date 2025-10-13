using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
<<<<<<< Updated upstream
    [SerializeField] private GameObject deathPanel; // painel “You Died”
    [SerializeField] private GameObject winPanel;   // painel “You Win” (opcional)
=======
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject winPanel;          // <- NOVO: painel de vitï¿½ria
>>>>>>> Stashed changes

    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

<<<<<<< Updated upstream
=======
    [Header("Inimigos / Vitï¿½ria")]
    [Tooltip("Arrasta aqui todos os inimigos da cena (componentes Enemy). A vitï¿½ria acontece quando todos morrerem.")]
    [SerializeField] private Enemy[] enemies;              // <- NOVO
    private int enemiesVivos = 0;                          // <- NOVO

    [Header("Comportamento")]
    [Tooltip("Se este objeto estiver marcado como DontDestroyOnLoad, destrï¿½i-o ao entrar no MainMenu.")]
    [SerializeField] private bool destroyOnMenu = true;

>>>>>>> Stashed changes
    // spawns iniciais
    private Vector3 playerStartPos, botStartPos;
    private Quaternion playerStartRot, botStartRot;

    private void Awake()
    {
<<<<<<< Updated upstream
        // garante que começa tudo escondido e tempo normal
        Time.timeScale = 1f;
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
=======
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
    // ====== SCREENS ==========================================================
    public void ShowDeathScreen()
    {
=======
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // subscreve aos eventos de morte dos inimigos (se existirem)
        enemiesVivos = 0;
        if (enemies != null)
        {
            foreach (var e in enemies)
            {
                if (e == null) continue;
                enemiesVivos++;
                e.Died += OnEnemyDied;                     // <- NOVO
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // remove subscriï¿½ï¿½es
        if (enemies != null)
        {
            foreach (var e in enemies)
            {
                if (e == null) continue;
                e.Died -= OnEnemyDied;                     // <- NOVO
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sempre que muda de cena, garante que o jogo nï¿½o fica ï¿½pausadoï¿½
        Time.timeScale = 1f;

        // E que nï¿½o hï¿½ overlays activos
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO

        // Se este manager for persistente, remove-o ao entrar no menu principal
        if (destroyOnMenu && scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    // chamado quando o player morre
    public void ShowDeathScreen()
    {
        if (winPanel) winPanel.SetActive(false);         // <- NOVO: garante que sï¿½ uma estï¿½ visï¿½vel
>>>>>>> Stashed changes
        if (deathPanel) deathPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

<<<<<<< Updated upstream
    public void ShowWinScreen()
    {
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    // ====== ACÇÕES DOS BOTÕES ================================================
=======
    // === VITï¿½RIA ===
    private void OnEnemyDied()                              // <- NOVO
    {
        enemiesVivos--;
        if (enemiesVivos <= 0)
            ShowWinningScreen();
    }

    public void ShowWinningScreen()                         // <- NOVO
    {
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f; // pausa para mostrar UI de vitï¿½ria
    }

    // Botï¿½o ï¿½Next Levelï¿½ (se tiveres cenas em Build Settings)
    public void NextLevel()                                 // <- OPCIONAL
    {
        Time.timeScale = 1f;
        Scene active = SceneManager.GetActiveScene();
        int nextIndex = active.buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex, LoadSceneMode.Single);
        else
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); // fallback
    }

    // Opï¿½ï¿½o A: reiniciar a cena (reset total)
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        if (winPanel) winPanel.SetActive(false);
=======
        if (winPanel) winPanel.SetActive(false);         // <- NOVO
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
>>>>>>> Stashed changes
    }
}

// Interfaces opcionais (se quiseres resetar vida/IA no Respawn)
public interface IResettableHealth { void ResetHealth(); }
public interface IResettableAI { void ResetAI(); }
