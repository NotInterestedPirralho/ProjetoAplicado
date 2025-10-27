using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;   // Painel de Derrota
    [SerializeField] private GameObject winPanel;     // Painel de Vitória
    [SerializeField] private CanvasGroup darkBackground; // CanvasGroup do Image preto full-screen

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

    [Header("Comportamento")]
    [Tooltip("Se este objeto estiver marcado como DontDestroyOnLoad, destrói-o ao entrar no MainMenu.")]
    [SerializeField] private bool destroyOnMenu = true;

    [Header("Efeitos de Fim")]
    [SerializeField] private bool doSlowMo = true;
    [SerializeField] private float slowMoScale = 0.2f;
    [SerializeField] private float slowMoDurationRealtime = 0.25f;
    [SerializeField] private float fadeDurationRealtime = 0.25f;
    [SerializeField, Range(0f, 1f)] private float darkAlpha = 0.75f;

    // spawns iniciais
    private Vector3 playerStartPos, botStartPos;
    private Quaternion playerStartRot, botStartRot;

    private bool locked; // bloqueia input quando um painel está activo

    private void Awake()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (darkBackground != null)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

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

        locked = false;
        Time.timeScale = 1f;
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sempre que muda de cena, garante que o jogo nao fica pausado
        Time.timeScale = 1f;
        locked = false;

        // Limpa UI
        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (darkBackground != null)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

        // Se este manager for persistente, remove-o ao entrar no menu principal
        if (destroyOnMenu && scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    // --- API: chama isto quando o PLAYER MORRE ---
    public void ShowDeathScreen()
    {
        if (locked) return;
        StartCoroutine(ShowEndScreenRoutine(deathPanel));
    }

    // --- API: chama isto quando o PLAYER GANHA ---
    public void ShowWinScreen()
    {
        if (locked) return;
        StartCoroutine(ShowEndScreenRoutine(winPanel));
    }

    private IEnumerator ShowEndScreenRoutine(GameObject targetPanel)
    {
        locked = true;

        // slow-motion curto
        if (doSlowMo)
        {
            Time.timeScale = slowMoScale;
            yield return new WaitForSecondsRealtime(slowMoDurationRealtime);
        }

        // pausa total
        Time.timeScale = 0f;

        // escurece o fundo (tempo real, não depende do timeScale)
        if (darkBackground != null)
        {
            darkBackground.blocksRaycasts = true;
            yield return StartCoroutine(FadeCanvasGroup(darkBackground, darkBackground.alpha, darkAlpha, fadeDurationRealtime));
            darkBackground.interactable = true;
        }

        // mostra o painel
        if (targetPanel != null) targetPanel.SetActive(true);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float durationRealtime)
    {
        if (cg == null || Mathf.Approximately(durationRealtime, 0f))
        {
            if (cg != null) cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.alpha = from;
        while (t < durationRealtime)
        {
            t += Time.unscaledDeltaTime; // usa tempo real
            cg.alpha = Mathf.Lerp(from, to, t / durationRealtime);
            yield return null;
        }
        cg.alpha = to;
    }

    private void Update()
    {
        if (!locked) return;

        // atalhos enquanto o painel está aberto
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.R))
            Retry();
        if (Input.GetKeyDown(KeyCode.Escape))
            BackToMenu();
    }

    // --- Botões / Ações ------------------------------------------------------

    public void Retry() => RestartLevel();

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
            if (rb2d) { rb2d.linearVelocity = Vector2.zero; rb2d.angularVelocity = 0f; } // se usares linearVelocity
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

        // limpar UI e desbloquear
        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (darkBackground != null)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

        locked = false;
    }

    public void BackToMenu()
    {
        // repõe imediatamente antes de trocar de cena
        Time.timeScale = 1f;

        if (deathPanel != null) deathPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (darkBackground != null)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

        locked = false;

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

// Interfaces opcionais para os teus scripts (se quiseres integrar reset de vida/IA)
public interface IResettableHealth { void ResetHealth(); }
public interface IResettableAI { void ResetAI(); }
