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
    [SerializeField] private Transform _jumpChecker;
    [SyncVar] [SerializeField] private LayerMask _floorMask;
    [SyncVar] [SerializeField] private float _speed;
    [SyncVar] [SerializeField] private float _jumpForce;

    [Header("Mouse Controller")]
    [SyncVar] [SerializeField] private Vector2 _mouseSensitivity;
    [SyncVar] [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SyncVar] [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    private float xRot;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player:OnStartLocalPlayer()");
        
        if(isClientOnly)
            gameObject.name = $"(Self) {gameObject.name}";

        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        camera.SetParent(transform);
        camera.localPosition = _fakeCamera.localPosition;
        camera.localRotation = Quaternion.Euler(Vector3.zero);

        Destroy(_fakeCamera.gameObject);
        _fakeCamera = camera;

        InputManager.Instance.SetMover(this);
    }

    // NOTE: commands normalizes vectors and they didn't work correctly on PS
    // Local methods makes calls to decrease input lag

    // FIX: I removed normalization in order to the player moves correctly,
    // but may be thus allowed to change direction for the worse. 
    [Command]
    public void CmdMove(Vector3 direction) {
        Vector3 dir = transform.TransformDirection(direction) * _speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;

        if(Physics.CheckSphere(_jumpChecker.position, 0.1f, _floorMask)) {
            _rb.velocity += Vector3.up * direction.y * _jumpForce;
        }
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
}
