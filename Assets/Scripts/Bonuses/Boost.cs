using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Boost : NetworkBehaviour
{
    [SerializeField] private float _jumpMultiplier;
    [SerializeField] private float _delay;

    private bool isGoneOut;

    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !other.isTrigger && !isGoneOut) {
            other.GetComponent<Mover>().JumpMultiplier = _jumpMultiplier;
            transform.position -= Vector3.up * 0.19f;
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && !other.isTrigger && !isGoneOut) {
            other.GetComponent<Mover>().JumpMultiplier = 1;
            StartCoroutine(WaitForBoost(other.attachedRigidbody.velocity.y > 0.1f));
            StartCoroutine(WaitForDelay());
        }
    }

    [ServerCallback]
    private IEnumerator WaitForBoost(bool isJump) {
        if(isJump) {
            transform.position += Vector3.up * 0.39f;
            yield return new WaitForSeconds(_delay);
            transform.position -= Vector3.up * 0.2f;
        }
        else {
            transform.position += Vector3.up * 0.19f;
        }
    }

    [ServerCallback]
    private IEnumerator WaitForDelay() {
        isGoneOut = true;
        yield return new WaitForSeconds(_delay);
        isGoneOut = false;
    }
}
