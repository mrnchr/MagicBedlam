using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _camera;
    private Color _colorPlayer;

    [SyncVar]
    [SerializeField]
    private float _speed;

    public override void OnStartClient()
    {
        Debug.Log("Player:OnStartClient()");
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player:OnStartLocalPlayer()");
        _colorPlayer = GetComponent<MeshRenderer>().material.color;

        _camera = Spawner.Instance.Camera;
        _camera.SetParent(transform);
        _camera.localPosition = new Vector3(0, 0.7f, 0);

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

    // TODO: it may be necessary to compare colors for compliance
    [Command]
    public void CmdGiveColor() => Spawner.Instance.GetColor(_colorPlayer);

    [Command]
    public void CmdMove(Vector3 direction) {
        direction = direction.normalized * _speed; // NOTE: that nobody can to increase the direction
        direction.y = _rb.velocity.y;
        _rb.velocity = direction;
    }

    // it calls to decrease input lag
    public void Move(Vector3 direction) {
        direction *= _speed;
        direction.y = _rb.velocity.y;
        _rb.velocity = direction;
    }

    private void Start() {
        Debug.Log("Player:Start()");
    }
}