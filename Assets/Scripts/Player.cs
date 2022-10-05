using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SyncVar] [SerializeField] private float _speed;

    [Header("Mouse Controller")]
    [SyncVar] [SerializeField] private Vector2 _mouseSensitivity;
    [SyncVar] [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SyncVar] [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    private Transform _camera;

    [SyncVar] float xRot;
    [SyncVar] private Color _colorPlayer;

    public override void OnStartServer()
    {
        Debug.Log("Player:OnStartServer()");
        _colorPlayer = Spawner.Instance.GiveColor();
    }

    public override void OnStartClient()
    {
        Debug.Log("Player:OnStartClient()");
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player:OnStartLocalPlayer()");
        
        if(isClientOnly)
            gameObject.name = $"(Self) {gameObject.name}";

        _camera = Spawner.Instance.Camera;
        _camera.SetParent(transform);
        _camera.localPosition = new Vector3(0, 0.7f, 0);
        _camera.localRotation = Quaternion.Euler(Vector3.zero);

        InputManager.Instance.SetPlayer(this);
    }

    public override void OnStopClient()
    {
        Debug.Log("Player:OnStopClient()");
        Spawner.Instance.Camera.SetParent(null);
    }

    public override void OnStopLocalPlayer()
    {
        Debug.Log("Player:OnStopLocalPlayer()");
    }

    public override void OnStopServer()
    {
        Debug.Log("Player:OnStopServer()");
        Spawner.Instance.GetColor(_colorPlayer);
    }

    #region Move
    [Command]
    private void CmdMove(Vector3 direction) {
        Vector3 dir = transform.TransformDirection(direction).normalized * _speed; // NOTE: that nobody can to increase the direction
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    // it calls to decrease input lag
    public void Move(Vector3 direction) {
        CmdMove(direction);
        
        Vector3 dir = transform.TransformDirection(direction).normalized * _speed; 
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }
    #endregion

    #region Rotation
    [Command]
    private void CmdRotate(Vector2 direction) {
        Vector2 dir = direction.normalized;
        transform.Rotate(new Vector3(0, dir.x, 0));
    }

    // it calls to decrease input lag
    public void Rotate(Vector2 direction) {
        CmdRotate(direction);
        Vector2 dir = direction.normalized;
        transform.Rotate(new Vector3(0, dir.x * _mouseSensitivity.x, 0));

        xRot -= dir.y;
        xRot = Mathf.Clamp(xRot, _minViewY, _maxViewY);
        _camera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
    #endregion

    private void Start() {
        Debug.Log("Player:Start()");
        GetComponent<MeshRenderer>().material.color = _colorPlayer;
    }
}