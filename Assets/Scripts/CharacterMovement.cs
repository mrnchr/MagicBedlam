using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
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

    private Vector3 CharacterMovementInput;
    private Vector2 CharacterMouseInput;
    private float xRot;

    void Start()
    {

    }

    void FixedUpdate()
    {
        CharacterMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        CharacterMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MoveCharacter();
        MoveCamera();        
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
}
