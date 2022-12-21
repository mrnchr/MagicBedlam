using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[ExecuteInEditMode]
public class Boost : MonoBehaviour
{

    [SerializeField] private float _jumpMultiplier;
    [SerializeField] private float _delay;

    [Header("Test Data")]
    [SerializeField] private bool _canEdit;
    [SerializeField] private bool _isPrepared;
    private Vector3 _startPosition; 


    private int _numOfPlayers;
    private bool isGoneOut;

#if UNITY_EDITOR

    private void Update() {
        if(!_canEdit) {
            _startPosition = transform.position;
        }
        else if(_isPrepared) {
            transform.position = _startPosition - Vector3.up * 0.19f;
        }
        else {
            transform.position = _startPosition;
        }
    }

#endif

    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !other.isTrigger && !isGoneOut) {
            other.GetComponent<Mover>().JumpMultiplier = _jumpMultiplier;
            _numOfPlayers++;
            if(_numOfPlayers == 1) {
                transform.position -= Vector3.up * 0.19f;
            }
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player") && !other.isTrigger && !isGoneOut) {
            other.GetComponent<Mover>().JumpMultiplier = 1;
            _numOfPlayers--;
            if(_numOfPlayers == 0) {
                StartCoroutine(WaitForBoost(other.attachedRigidbody.velocity.y > 0.1f));
                StartCoroutine(WaitForDelay());
            }   
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
