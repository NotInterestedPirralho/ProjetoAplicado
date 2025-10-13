using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject winPanel;          // <- NOVO: painel de vit�ria

    [Header("Refer�ncias")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

    [Header("Inimigos / Vit�ria")]
    [Tooltip("Arrasta aqui todos os inimigos da cena (componentes Enemy). A vit�ria acontece quando todos morrerem.")]
    [SerializeField] private Enemy[] enemies;              // <- NOVO
    private int enemiesVivos = 0;                          // <- NOVO

    [Header("Comportamento")]
    [Tooltip("Se este objeto estiver marcado como DontDestroyOnLoad, destr�i-o ao entrar no MainMenu.")]
    [SerializeField] private bool destroyOnMenu = true;

    // spawns iniciais
    private Vector3 playerStartPos, botStartPos;
    private Quaternion playerStartRot, botStartRot;

    private void Awake()
    {
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO

        if (player != null)
        {
            playerStartPos = player.position;
            playerStartRot = player.rotation;
        }
        if (bot != null)
        {
            botStartPos = bot.position;
            botStartRot = bot.rotation;
        }
    }

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

        // remove subscri��es
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
        // Sempre que muda de cena, garante que o jogo n�o fica �pausado�
        Time.timeScale = 1f;

        // E que n�o h� overlays activos
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
        if (winPanel) winPanel.SetActive(false);         // <- NOVO: garante que s� uma est� vis�vel
        if (deathPanel) deathPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    // === VIT�RIA ===
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
        Time.timeScale = 0f; // pausa para mostrar UI de vit�ria
    }

    // Bot�o �Next Level� (se tiveres cenas em Build Settings)
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

    // Op��o A: reiniciar a cena (reset total)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex, LoadSceneMode.Single);
    }

    // Op��o B: respawn sem recarregar a cena
    public void Respawn()
    {
        Time.timeScale = 1f;

        // player
        if (player != null)
        {
            var rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d) { rb2d.linearVelocity = Vector2.zero; rb2d.angularVelocity = 0f; }
            var rb3d = player.GetComponent<Rigidbody>();
            if (rb3d) { rb3d.linearVelocity = Vector3.zero; rb3d.angularVelocity = Vector3.zero; }

            player.SetPositionAndRotation(playerStartPos, playerStartRot);

            var hp = player.GetComponent<IResettableHealth>();
            if (hp != null) hp.ResetHealth();
        }

        // bot
        if (bot != null)
        {
            var rb2d = bot.GetComponent<Rigidbody2D>();
            if (rb2d) { rb2d.linearVelocity = Vector2.zero; rb2d.angularVelocity = 0f; }
            var rb3d = bot.GetComponent<Rigidbody>();
            if (rb3d) { rb3d.linearVelocity = Vector3.zero; rb3d.angularVelocity = Vector3.zero; }

            bot.SetPositionAndRotation(botStartPos, botStartRot);

            var hp = bot.GetComponent<IResettableHealth>();
            if (hp != null) hp.ResetHealth();

            var ai = bot.GetComponent<IResettableAI>();
            if (ai != null) ai.ResetAI();
        }

        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);         // <- NOVO

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

// Interfaces opcionais para os teus scripts (se quiseres integrar reset de vida/IA)
public interface IResettableHealth { void ResetHealth(); }
public interface IResettableAI { void ResetAI(); }
