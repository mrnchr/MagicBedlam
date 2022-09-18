using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _forceJump;
    
    [Header("Mouse Controller")]
    [SerializeField] private Vector2 _mouseSensitivity;
    [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    private float _rotationX;
    private Rigidbody _rb;
    private GameObject _camera;
    private bool _isJump;

    void Start()
    {
        _rotationX = 0f;
        _rb = GetComponent<Rigidbody>();  
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _isJump = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _isJump = false;
    }

    void FixedUpdate()
    {

        //Mouse Controller
        _rotationX -= Input.GetAxis("Mouse Y") * _mouseSensitivity.y;
        _rotationX = Mathf.Clamp(_rotationX, _minViewY, _maxViewY);

        transform.Rotate(0, Input.GetAxis("Mouse X") * _mouseSensitivity.x, 0);
        _camera.transform.localEulerAngles = new Vector3(_rotationX, 0, 0);

        // Movement
        float velY = _rb.velocity.y;

        _rb.velocity = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) 
            * _speed;

        _rb.velocity += new Vector3(0, velY, 0);

        if (!_isJump && Input.GetKey(KeyCode.Space))
        {
            _isJump = true;

            _rb.velocity = Vector3.Scale(_rb.velocity, new Vector3(1, 0, 1)); // velocity.y = 0; otherwise force up will be depressed force down
            _rb.AddForce(Vector3.up * _forceJump, ForceMode.Impulse);
        }
    }
}
