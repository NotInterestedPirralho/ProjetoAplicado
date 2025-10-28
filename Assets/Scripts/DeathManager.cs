using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;         // Painel de Derrota
    [SerializeField] private GameObject winPanel;           // Painel de Vitória
    [SerializeField] private CanvasGroup darkBackground;    // Fundo escuro com CanvasGroup

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform bot;

    [Header("Comportamento")]
    [SerializeField] private bool destroyOnMenu = true;

    [Header("Efeitos de Fim")]
    [SerializeField] private bool doSlowMo = true;
    [SerializeField] private float slowMoScale = 0.2f;
    [SerializeField] private float slowMoDurationRealtime = 0.25f;
    [SerializeField] private float fadeDurationRealtime = 0.25f;
    [SerializeField, Range(0f, 1f)] private float darkAlpha = 0.75f;

    private bool locked; // já mostrei um painel? então bloquear input

    private void Awake()
    {
        // garantir que tudo começa escondido/limpo
        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        if (darkBackground)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

        locked = false;
        Time.timeScale = 1f;
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // sempre que entro numa cena nova, desbloqueia jogo
        Time.timeScale = 1f;
        locked = false;

        if (deathPanel) deathPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        if (darkBackground)
        {
            darkBackground.alpha = 0f;
            darkBackground.interactable = false;
            darkBackground.blocksRaycasts = false;
        }

        if (destroyOnMenu && scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    // -------- CHAMADO PELO PLAYER QUANDO ELE MORRE -----------
    public void ShowDeathScreen()
    {
        if (locked) return;
        StartCoroutine(ShowEndScreenRoutine(deathPanel));
    }

    // -------- CHAMADO QUANDO O PLAYER GANHA -----------
    public void ShowWinScreen()
    {
        if (locked) return;
        StartCoroutine(ShowEndScreenRoutine(winPanel));
    }

    private IEnumerator ShowEndScreenRoutine(GameObject targetPanel)
    {
        locked = true;

        // 1. slow motion curto
        if (doSlowMo)
        {
            Time.timeScale = slowMoScale;
            yield return new WaitForSecondsRealtime(slowMoDurationRealtime);
        }

        // 2. pausa total
        Time.timeScale = 0f;

        // 3. fade do fundo escuro
        if (darkBackground)
        {
            darkBackground.blocksRaycasts = true;
            yield return StartCoroutine(FadeCanvasGroup(
                darkBackground,
                darkBackground.alpha,
                darkAlpha,
                fadeDurationRealtime
            ));
            darkBackground.interactable = true;
        }

        // 4. mostrar painel final (derrota ou vitória)
        if (targetPanel) targetPanel.SetActive(true);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float durationRealtime)
    {
        if (!cg || Mathf.Approximately(durationRealtime, 0f))
        {
            if (cg) cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.alpha = from;

        while (t < durationRealtime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / durationRealtime);
            yield return null;
        }

        cg.alpha = to;
    }

    private void Update()
    {
        if (!locked) return;

        // atalhos enquanto o painel de fim está activo
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.R))
            RestartLevel();

        if (Input.GetKeyDown(KeyCode.Escape))
            BackToMenu();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex, LoadSceneMode.Single);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
