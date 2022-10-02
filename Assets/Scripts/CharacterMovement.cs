using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private Transform _feet;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _forceJump;
    
    [Header("Mouse Controller")]
    [SerializeField] private Transform _camera;
    [SerializeField] private Vector2 _mouseSensitivity;
    [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    [Header("Upstairs")]
    [SerializeField] private Transform _lowerStepChecker;
    [SerializeField] private Transform _upperStepCheker;
    [SerializeField] private float _stepHeight;
    [SerializeField] private float _stepSmooth;
    [SerializeField] private float _stepDistance;
    [SerializeField] private float _footLength;

    private Vector3 CharacterMovementInput;
    private Vector2 CharacterMouseInput;
    private float xRot;

    public override void OnStartLocalPlayer() {

    } 

    private void Start()
    {
        Vector3 checkerPos = _lowerStepChecker.position;
        checkerPos.y += _stepHeight;
        _upperStepCheker.position = checkerPos;
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer) return;

        CharacterMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        CharacterMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MoveCharacter();
        MoveCamera();
        StepChecker();
    }

    private void MoveCharacter()
    {
        Vector3 moveVector = transform.TransformDirection(CharacterMovementInput) * _speed;
        _rb.velocity = new Vector3(moveVector.x, _rb.velocity.y, moveVector.z);

        if (Physics.CheckSphere(_feet.position, 0.1f, _floorMask) && Input.GetKey(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * _forceJump, ForceMode.Impulse);
        }
    }

    private void MoveCamera()
    {
        xRot -= CharacterMouseInput.y * _mouseSensitivity.x;
        xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);

        transform.Rotate(0, CharacterMouseInput.x * _mouseSensitivity.x, 0); 
        _camera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }

    private void StepChecker()
    {
        if (Physics.Raycast(_lowerStepChecker.position, _rb.velocity.normalized, _stepDistance * Mathf.Sqrt(2)))
        {
            if(!Physics.Raycast(_upperStepCheker.position, _rb.velocity.normalized, (_stepDistance + _footLength) * Mathf.Sqrt(2)))
            {
                _rb.position += new Vector3(0, _stepSmooth, 0);
            }
        }
    }
}
