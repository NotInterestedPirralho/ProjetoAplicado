using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;

    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

    [Header("Comportamento")]
    [Tooltip("Se este objeto estiver marcado como DontDestroyOnLoad, destrói-o ao entrar no MainMenu.")]
    [SerializeField] private bool destroyOnMenu = true;

    // spawns iniciais
    private Vector3 playerStartPos, botStartPos;
    private Quaternion playerStartRot, botStartRot;

    private void Awake()
    {
        if (deathPanel != null) deathPanel.SetActive(false);

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
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sempre que muda de cena, garante que o jogo não fica “pausado”
        Time.timeScale = 1f;

        // E que não há overlay a bloquear cliques
        if (deathPanel != null) deathPanel.SetActive(false);

        // Se este manager for persistente, remove-o ao entrar no menu principal
        if (destroyOnMenu && scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    // chamado quando o player morre
    public void ShowDeathScreen()
    {
        if (deathPanel != null) deathPanel.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    // Opção A: reiniciar a cena (reset total)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex, LoadSceneMode.Single);
    }

    // Opção B: respawn sem recarregar a cena
    public void Respawn()
    {
        Time.timeScale = 1f;

        // player
        if (player != null)
        {
            var rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d) { rb2d.velocity = Vector2.zero; rb2d.angularVelocity = 0f; }
            var rb3d = player.GetComponent<Rigidbody>();
            if (rb3d) { rb3d.velocity = Vector3.zero; rb3d.angularVelocity = Vector3.zero; }

            player.SetPositionAndRotation(playerStartPos, playerStartRot);

            var hp = player.GetComponent<IResettableHealth>();
            if (hp != null) hp.ResetHealth();
        }

        // bot
        if (bot != null)
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

        if (deathPanel != null) deathPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        // repõe imediatamente antes de trocar de cena
        Time.timeScale = 1f;
        if (deathPanel != null) deathPanel.SetActive(false);

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

// Interfaces opcionais para os teus scripts (se quiseres integrar reset de vida/IA)
public interface IResettableHealth { void ResetHealth(); }
public interface IResettableAI { void ResetAI(); }
