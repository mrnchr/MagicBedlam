using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Mover : NetworkBehaviour
{
    [SerializeField] private Transform _fakeCamera;

    public Transform FakeCamera {
        get {
            return _fakeCamera;
        }
    }

    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _jumpChecker;
    [SerializeField] private LayerMask _jumpMask;
    [SerializeField] private LayerMask _floorMask;
    [SyncVar] [SerializeField] private float _speed;
    [SyncVar] [SerializeField] private float _swimSpeed;
    [SyncVar] [SerializeField] private float _jumpForce;
    [SyncVar] private bool _canJump;
    [SyncVar] private float _jumpMultiplier;
    [SyncVar] public bool _isSwim;
    [SyncVar] public bool _canSwimJump;
    [SyncVar] public bool _canClimb;

    public float JumpMultiplier {
        set {
            _jumpMultiplier = value;
        }
    }

    public bool CanSwimJump {
        set {
            _canSwimJump = value;
        }
    }

    [Header("Mouse Controller")]
    [SyncVar] [SerializeField] private Vector2 _mouseSensitivity;
    [SyncVar] [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SyncVar] [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    private float xRot;

    [Header("Scrambling Ashore")]
    [SerializeField] private Transform _lowerCoastChecker;
    [SerializeField] private Transform _upperCoastChecker;

    public override void OnStartServer()
    {
        _canJump = true;
        _jumpMultiplier = 1;
        _rb.AddForce(Vector3.up*2, ForceMode.Acceleration);
    }

    public override void OnStartLocalPlayer()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        camera.SetParent(transform);
        camera.localPosition = _fakeCamera.localPosition;
        camera.localRotation = Quaternion.Euler(Vector3.zero);

        Destroy(_fakeCamera.gameObject);
        _fakeCamera = camera;

        InputManager.Instance.SetMover(this);
    }

    public override void OnStopLocalPlayer()
    {
        InputManager.Instance.SetMover(null);
    }

    [ServerCallback]
    private void FixedUpdate() {
        if(_isSwim || _canClimb) {
            ClimbAshore();
        }
    }
    
    [Server]
    private void ClimbAshore() {
        RaycastHit hit;
        if(RaycastToCoast(_lowerCoastChecker, out hit)) {
            if(!RaycastToCoast(_upperCoastChecker, out hit)) {
                _canClimb = true;
            }
            else {
                _canClimb = false;
            }
        }
        else {
            _canClimb = false;
        }
    } 

    [Server]
    private bool RaycastToCoast(Transform checker, out RaycastHit hit) {
        return Physics.Raycast(checker.position, checker.forward, out hit, 0.6f, _floorMask);
    }

    // NOTE: commands normalizes vectors and they didn't work correctly on PS
    // Local methods makes calls to decrease input lag

    // FIX: I removed normalization in order to the player moves correctly,
    // but may be thus allowed to change direction for the worse. 
    [Command]
    public void CmdMove(Vector3 direction) {
        Vector3 dir;
        if(!_isSwim) {
            dir = transform.TransformDirection(direction) * _speed;
            dir.y = _rb.velocity.y;
            _rb.velocity = dir;
            
            if(direction.y != 0) {
                if(_canJump && Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore)) {
                    _rb.AddForce(Vector3.up * _jumpForce * _jumpMultiplier, ForceMode.VelocityChange);
                    StartCoroutine(WaitForFall());
                    StartCoroutine(WaitForJump());
                }
                else if(_canClimb) {
                    _rb.AddForce(Vector3.up * _swimSpeed, ForceMode.VelocityChange);
                    _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
                }
            }
        }
        else {
            dir = transform.TransformDirection(direction) * _swimSpeed;
            if(!_canSwimJump)
                dir.y = 0;
            _rb.AddForce(dir, ForceMode.VelocityChange);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
        }

        _anim.SetBool("IsRun", _isSwim || _canClimb || dir.x * dir.x + dir.z * dir.z > 0.1f);
    }

    public void Move(Vector3 direction) {
        Vector3 dir;
        if(!_isSwim) {
            dir = transform.TransformDirection(direction) * _speed;
            dir.y = _rb.velocity.y;
            _rb.velocity = dir;

            if(direction.y != 0) {
                if(_canJump && Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore)) {
                    _rb.AddForce(Vector3.up * _jumpForce * _jumpMultiplier, ForceMode.VelocityChange);
                }
                else if(_canClimb) {
                    _rb.AddForce(Vector3.up * _swimSpeed, ForceMode.VelocityChange);
                    _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
                }
            }
        }
        else {
            dir = transform.TransformDirection(direction) * _swimSpeed;
            if(!_canSwimJump)
                dir.y = 0;
            _rb.AddForce(dir, ForceMode.VelocityChange);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _swimSpeed);
        }
    }

    [Server]
    private IEnumerator WaitForFall() {
        _anim.speed = 0;

        while(!Physics.CheckSphere(_jumpChecker.position, 0.1f, _jumpMask, QueryTriggerInteraction.Ignore))
        {
            yield return null;
        }
        
        _anim.speed = 1;        
    }

    [Server]
    private IEnumerator WaitForJump() {
        _canJump = false;
        yield return new WaitForSeconds(0.2f);
        _canJump = true;
    }

    #region Rotation
    [Command]
    private void CmdRotate(Vector2 direction) {
        transform.Rotate(new Vector3(0, direction.x * _mouseSensitivity.x, 0));

        xRot -= direction.y * _mouseSensitivity.y;
        xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);
        _fakeCamera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }

    // it calls to decrease input lag
    public void Rotate(Vector2 direction) {
        if(NetworkClient.active)
            CmdRotate(direction);

        //transform.Rotate(new Vector3(0, direction.x * _mouseSensitivity.x, 0));

        xRot -= direction.y * _mouseSensitivity.y;
        xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);
        FakeCamera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
    #endregion

    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Water")) {
            _isSwim = true;
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Water")) {
            _isSwim = false;
        }
    }
}
