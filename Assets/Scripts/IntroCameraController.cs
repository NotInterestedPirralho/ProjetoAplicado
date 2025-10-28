using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class IntroCameraController : MonoBehaviour
{
    [Header("Câmaras")]
    [SerializeField] private CinemachineCamera introCam;
    [SerializeField] private CinemachineCamera fightCam;

    [Header("Timing")]
    [SerializeField] private float introDuration = 2f;          // quanto tempo mostra a arena
    [SerializeField] private float delayBeforeControl = 0.3f;    // pausa extra depois do blend

    [Header("Player (opcional)")]
    [SerializeField] private PlayerController2D playerController;

    private void Start()
    {
        // bloquear input do player no início (opcional mas dá mais polimento)
        if (playerController != null)
            playerController.enabled = false;

        // garantir prioridades iniciais
        if (introCam != null) introCam.Priority = 20;
        if (fightCam != null) fightCam.Priority = 10;

        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        // 1. Mostrar a arena inteira durante X segundos
        yield return new WaitForSeconds(introDuration);

        // 2. Passar o controlo da câmara para a fightCam (que segue o player)
        if (fightCam != null) fightCam.Priority = 30;
        if (introCam != null) introCam.Priority = 5;

        // 3. Esperar um bocadinho para deixar o blend acabar
        yield return new WaitForSeconds(delayBeforeControl);

        // 4. Voltar a dar controlo ao jogador
        if (playerController != null)
            playerController.enabled = true;
    }
}
