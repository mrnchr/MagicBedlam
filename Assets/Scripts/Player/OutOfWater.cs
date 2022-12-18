using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OutOfWater : MonoBehaviour
{
    [SerializeField] private Mover _mover;

    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Water")) {
            _mover.CanSwimJump = true;
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Water")) {
            _mover.CanSwimJump = false;
        }
    }
}
