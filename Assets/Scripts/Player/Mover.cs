using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Player movements
    /// </summary>
    public class Mover : NetworkBehaviour
    {
        /// <summary>
        /// Object simulating camera not on the local player    
        /// </summary>
        [SerializeField] protected Transform _fakeCamera;
        [SerializeField] protected PlayerAudioSource _ownAudio;

        [Header("Movement")]
        [Tooltip("Jump force")]
        [SerializeField]
        [SyncVar]
        [Range(0, 20)]
        protected float _jumpForce;
        [Tooltip("Speed when running")]
        [SerializeField]
        [SyncVar]
        [Range(0, 20)]
        protected float _speed;
        [Tooltip("Speed when swimming")]
        [SerializeField]
        [SyncVar]
        [Range(0, 20)]
        protected float _swimSpeed;
        [Tooltip("Objects which player can jump on")]
        [SerializeField] 
        protected LayerMask _jumpMask;
        [Tooltip("Objects which player can stand on and can not move")]
        [SerializeField] 
        protected LayerMask _floorMask;
        [Tooltip("Own Ridigbody component")]
        [SerializeField] 
        protected Rigidbody _rb;
        [Tooltip("Own Animator component")]
        [SerializeField] 
        protected Animator _anim;
        [Tooltip("The lowest point of the player")]
        [SerializeField] 
        protected Transform _jumpChecker;
        [SyncVar] public bool _canClimb;
        [SyncVar] protected bool _canJump;
        [SyncVar] public bool _canSwimJump;
        [SyncVar] public bool _isSwim;
        [SyncVar] protected float _jumpMultiplier;

        [Header("Mouse Controller")]
        [Tooltip("Mouse sensitivity on the X and Y axes")]
        [SerializeField]
        [SyncVar] 
        protected Vector2 _mouseSensitivity;
        [Tooltip("Value when looking up")]
        [SerializeField]
        [SyncVar]
        protected float _minViewY;
        [Tooltip("Value when looking down")]
        [SerializeField]
        [SyncVar]
        protected float _maxViewY;

        [Header("Scrambling Ashore")]
        [Tooltip("The lowest point of the player")]
        [SerializeField] 
        protected Transform _lowerCoastChecker;
        [Tooltip("The highest point where the player can reach")]
        [SerializeField] 
        protected Transform _upperCoastChecker;

        [SyncVar] protected float xRot;

        public bool CanSwimJump
        {
            set
            {
                _canSwimJump = value;
            }
        }

        public Transform FakeCamera
        {
            get
            {
                return _fakeCamera;
            }
        }

        public float JumpMultiplier
        {
            set
            {
                _jumpMultiplier = value;
            }
        }

        public void Move(Vector3 direction)
        {
            Vector3 dir;
            if (!_isSwim)
            {
                dir = transform.TransformDirection(direction) * _speed;
                dir.y = _rb.velocity.y;
                _rb.velocity = dir;

                if (direction.y != 0)
                {
                    if (_canJump && Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore))
                    {
                        _rb.AddForce(Vector3.up * _jumpForce * _jumpMultiplier, ForceMode.VelocityChange);
                    }
                    else if (_canClimb)
                    {
                        _rb.AddForce(Vector3.up * _swimSpeed, ForceMode.VelocityChange);
                        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
                    }
                }
            }
            else
            {
                dir = transform.TransformDirection(direction) * _swimSpeed;
                if (!_canSwimJump)
                    dir.y = 0;
                _rb.AddForce(dir, ForceMode.VelocityChange);
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
            }
        }

        public override void OnStartLocalPlayer()
        {
            Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            camera.SetParent(transform);
            camera.localPosition = _fakeCamera.localPosition;
            camera.localRotation = Quaternion.Euler(Vector3.zero);

            Destroy(_fakeCamera.gameObject);
            _fakeCamera = camera;

            InputManager.singleton.SetMover(this);
        }

        public override void OnStartServer()
        {
            _canJump = true;
            _jumpMultiplier = 1;
        }

        public override void OnStopLocalPlayer()
        {
            InputManager.singleton.SetMover(null);
        }

        // it calls to decrease input lag
        public void Rotate(Vector2 direction)
        {
            transform.Rotate(0, direction.x * _mouseSensitivity.x, 0);
            //_rb.angularVelocity = new Vector3(0, direction.x * _mouseSensitivity.x, 0);

            xRot -= direction.y * _mouseSensitivity.y;
            xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);
            FakeCamera.localRotation = Quaternion.Euler(xRot, 0, 0);
        } 

        [Server]
        protected void ClimbAshore()
        {
            if (RaycastToCoast(_lowerCoastChecker))
            {
                if (!RaycastToCoast(_upperCoastChecker))
                {
                    _canClimb = true;
                }
                else
                {
                    _canClimb = false;
                }
            }
            else
            {
                _canClimb = false;
            }
        }

        [Command]
        public void CmdMove(Vector3 direction)
        {
            Vector3 dir;
            if (!_isSwim)
            {
                dir = transform.TransformDirection(direction) * _speed;
                dir.y = _rb.velocity.y;
                _rb.velocity = dir;

                if (direction.y != 0)
                {
                    if (_canJump && Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore))
                    {
                        _ownAudio.RpcPlay(PlayerSound.Jump);
                        Vector3 vel = _rb.velocity;
                        vel.y = 0;
                        _rb.velocity = vel;
                        _rb.AddForce(Vector3.up * _jumpForce * _jumpMultiplier, ForceMode.VelocityChange);
                        StartCoroutine(WaitForFall());
                        StartCoroutine(WaitForJump());
                    }
                    else if (_canClimb)
                    {
                        _rb.AddForce(Vector3.up * _swimSpeed, ForceMode.VelocityChange);
                        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
                    }
                }
            }
            else
            {
                dir = transform.TransformDirection(direction) * _swimSpeed;
                if (!_canSwimJump)
                    dir.y = 0;
                _rb.AddForce(dir, ForceMode.VelocityChange);
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
            }

            _anim.SetBool("IsRun", _isSwim || _canClimb || dir.x * dir.x + dir.z * dir.z > 0.1f);
        }

        [Command]
        public void CmdRotate(Vector2 direction)
        {
            transform.Rotate(0, direction.x * _mouseSensitivity.x, 0);
            //_rb.angularVelocity = new Vector3(0, direction.x * _mouseSensitivity.x, 0);

            xRot -= direction.y * _mouseSensitivity.y;
            xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);
            _fakeCamera.localRotation = Quaternion.Euler(xRot, 0, 0);
        }

        [ServerCallback]
        protected void FixedUpdate()
        {
            if (_isSwim || _canClimb)
            {
                ClimbAshore();
            }
        }

        [ServerCallback]
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _isSwim = true;
            }
        }

        [ServerCallback]
        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _isSwim = false;
            }
        }

        [Server]
        protected bool RaycastToCoast(Transform checker)
        {
            return Physics.Raycast(checker.position, checker.forward, 0.6f, _floorMask);
        }

        [Server]
        protected IEnumerator WaitForFall()
        {
            _anim.speed = 0;

            while (!Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore))
            {
                yield return null;
            }

            _anim.speed = 1;
        }

        [Server]
        protected IEnumerator WaitForJump()
        {
            _canJump = false;
            yield return new WaitForSeconds(0.2f);
            _canJump = true;
        }

        protected void Reset() {
            TryGetComponent<Rigidbody>(out _rb);
            TryGetComponent<Animator>(out _anim);
        }
    }
}
