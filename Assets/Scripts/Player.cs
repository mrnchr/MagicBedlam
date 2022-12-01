using System.Collections;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    [SerializeField] private Transform _fakeCamera;

    public Transform FakeCamera {
        get {
            return _fakeCamera;
        }
    }

    [Header("Movement")]
    [SerializeField] private MeshRenderer _ownMesh;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _jumpChecker;
    [SyncVar] [SerializeField] private LayerMask _floorMask;
    [SyncVar] [SerializeField] private float _speed;
    [SyncVar] [SerializeField] private float _jumpForce;

    [Header("Mouse Controller")]
    [SyncVar] [SerializeField] private Vector2 _mouseSensitivity;
    [SyncVar] [SerializeField] [Tooltip("Value when looking up")] private float _minViewY;
    [SyncVar] [SerializeField] [Tooltip("Value when looking down")] private float _maxViewY;

    [SyncVar(hook = nameof(WhenChangeSelfColor))] private Color _selfColor;

    private float xRot;

    public void WhenChangeSelfColor(Color oldColor, Color newColor) {
        _ownMesh.material.color = newColor;
    }

    public override void OnStartServer()
    {
        Debug.Log("Player:OnStartServer()");
        Debug.Log($"Connection on the server: {connectionToClient}");
        _selfColor = GameManager.Instance.GetPlayerInfoByConn(connectionToClient.connectionId).playerColor;
        StartCoroutine(WaitForRpc()); // Rpc does not run immediately
    }

    [Server]
    private IEnumerator WaitForRpc() {
        yield return new WaitForSeconds(0.1f);
        RpcSetLocalClient(connectionToClient.connectionId);
    }

    [ClientRpc]
    private void RpcSetLocalClient(int conn) {        
        if(!isLocalPlayer) return;
        Spawner.Instance.SetConnection(conn);
        GameMenu.Instance.SetColor();
    }

    [Server]
    public void Dead() {
        GetComponent<Telekinesis>().DropObject();
        Spawner.Instance.Respawn(gameObject.transform);
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

        Spawner.Instance.Camera.SetParent(transform);
        Spawner.Instance.Camera.localPosition = _fakeCamera.localPosition;
        Spawner.Instance.Camera.localRotation = Quaternion.Euler(Vector3.zero);

        Destroy(_fakeCamera.gameObject);
        _fakeCamera = Spawner.Instance.Camera;

        InputManager.Instance.SetPlayer(this);
    }

    public override void OnStopClient()
    {
        Debug.Log("Player:OnStopClient()");
    }

    public override void OnStopLocalPlayer()
    {
        Debug.Log("Player:OnStopLocalPlayer()");
    }

    public override void OnStopServer()
    {
        Debug.Log("Player:OnStopServer()");
        if(isClientOnly) {
            //Spawner.Instance.AddColor(_colorPlayer);
        }
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
        Spawner.Instance.Camera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
    #endregion

    private void Start() {
        Debug.Log("Player:Start()");
    }
}