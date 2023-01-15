using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    ///     Representation a lift object
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Lift : MonoBehaviour
    {
        [Tooltip("The movement speed of the lift")]
        [SerializeField]
        [Range(0, 20)]
        protected float _speed;
        [Tooltip("Stop time at endpoints")]
        [SerializeField]
        [Range(0, 10)]
        protected float _delay;
        [Tooltip("The lowest point of the lift")]
        [SerializeField]
        protected Transform _bottom;
        [Tooltip("The highest point of the lift")]
        [SerializeField]
        protected Transform _top;
        [Tooltip("Own Rigidbody component")]
        [SerializeField]
        protected Rigidbody _rb;

        protected bool _canMove;
        protected float _movementTime;
        protected float _timer;

        protected void Start()
        {
            _canMove = true;
            _movementTime = (_top.position.y - _bottom.position.y) / _speed;
        }

        [ServerCallback]
        protected void FixedUpdate()
        {
            if (_canMove)
            {
                _timer += Time.fixedDeltaTime * Mathf.Sign(_speed);
                _rb.MovePosition(Vector3.Lerp(_bottom.position, _top.position, _timer / _movementTime));

                if (_timer <= 0 || _timer >= _movementTime)
                {
                    StartCoroutine(WaitForStop());
                }
            }
        }

        [ServerCallback]
        protected void OnCollisionEnter(Collision other)
        {
            if (other.transform.position.y < transform.position.y)
            {
                StartCoroutine(WaitForStop());
            }
        }

        [Server]
        protected IEnumerator WaitForStop()
        {
            _canMove = false;
            _speed = -_speed;
            yield return new WaitForSeconds(_delay);
            _canMove = true;
        }

        protected void Reset() {
            _rb = GetComponent<Rigidbody>();
        }

        protected void OnValidate()
        {
            if (_bottom?.position.y > _top?.position.y)
            {
                Debug.LogWarning($"Y coordinate of the Top object should be greater than Y coordinate of the Bottom object in the {this.GetType().ToString()} component");
            }
        }
    }
}
