using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeathZone : MonoBehaviour
{
    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            WinTracker.Instance.PlayerDeath(other.GetComponent<Player>(), null);
        }
    }
}
