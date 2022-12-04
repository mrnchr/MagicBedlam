using System.Collections;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    // private static Player _localPlayer;

    // public static Player LocalPlayer {
    //     get {
    //         return _localPlayer;
    //     }
    // }

    // private void Awake() {
    //     if(isLocalPlayer) {
    //         _localPlayer = this;
    //     }

    //     Debug.Log("Local player is defined");
    // }

    [SerializeField] private MeshRenderer _ownMesh;
    [SyncVar(hook = nameof(WhenChangeOwnColor))] private Color _ownColor;
    [SyncVar(hook = nameof(WhenChangeScores))] private int _scores;

    public void WhenChangeOwnColor(Color oldValue, Color newValue) {
        Debug.Log("The own color is changed");
        _ownMesh.material.color = newValue;
        TableKeeper.Instance.AddPlayerInfo(newValue);
    }

    public void WhenChangeScores(int oldValue, int newValue) {
        TableKeeper.Instance.ChangePlayerTable(_ownColor, newValue);
    }

    [Server] 
    public int IncrementScores() {
        return ++_scores;
    }

    public override void OnStartServer()
    {
        Debug.Log($"Connection on the server: {connectionToClient}");
        _ownColor = ColorPicker.Instance.PickColor();
        //StartCoroutine(WaitForRpc()); // Rpc does not run immediately
    }

    // [Server]
    // private IEnumerator WaitForRpc() {
    //     yield return new WaitForSeconds(0.1f);
    //     RpcSetLocalClient(connectionToClient.connectionId);
    // }

    // [ClientRpc]
    // private void RpcSetLocalClient(int conn) {        
    //     if(!isLocalPlayer) return;
    //     Spawner.Instance.SetConnection(conn);
    //     GameMenu.Instance.SetColor();
    // }

    [Server]
    public void Dead() {
        GetComponent<Telekinesis>().DropObject();
        Spawner.Instance.Respawn(gameObject.transform);
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player:OnStartLocalPlayer");
        if(isClientOnly)
            gameObject.name = $"(Self) {gameObject.name}";

        ColorChanger.Instance.SetColor(_ownColor);

    }

    

    private void Start() {
        Debug.Log("Player:Start()");
    }
}