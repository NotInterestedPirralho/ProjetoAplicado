using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public SmoothSyncMovement movement;

    public GameObject camara;

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        camara.SetActive(true);
    }
}
