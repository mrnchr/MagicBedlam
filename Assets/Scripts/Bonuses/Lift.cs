using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lift : NetworkBehaviour
{
    [SerializeField] private Transform _bottom;
    [SerializeField] private Transform _top;
    [SerializeField] private float _speed;
    [SerializeField] private float _delay;
    [SerializeField] private Rigidbody _rb;

    private bool _canMove;
    private float _timer;
    private float _movementTime;
    
    public override void OnStartServer() {
        Debug.Log("Lift:OnStartServer");

        _canMove = true;
        _movementTime = (_top.position.y - _bottom.position.y) / _speed; 
    }

    [ServerCallback]
    private void FixedUpdate() {
        if(_canMove) {
            _timer += Time.fixedDeltaTime * Mathf.Sign(_speed);
            _rb.MovePosition(Vector3.Lerp(_bottom.position, _top.position, _timer / _movementTime));
            
            if(_timer <= 0 || _timer >= _movementTime) {
                StartCoroutine(WaitForStop());
            }
        }
    }

    [ServerCallback]
    private IEnumerator WaitForStop() {
        _canMove = false;
        _speed = -_speed;
        yield return new WaitForSeconds(_delay);
        _canMove = true;
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision other) {
        if(other.transform.position.y < transform.position.y) {
            StartCoroutine(WaitForStop());
        }
    }
}
