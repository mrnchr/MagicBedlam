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

        // NOTE: not to swap these strings. Color have to be got before scores 
        // that tablekeeper could to decide whether add the player on the table or not
        _ownColor = NetworkInteraction.Instance.GetColor(connectionToClient.address);
        _scores = NetworkInteraction.Instance.GetScores(connectionToClient.address);

        Debug.Log($"The player with ip {connectionToClient.address} got color {_ownColor} and {_scores} scores");
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
    //     NetworkInteraction.Instance.SetConnection(conn);
    //     GameMenu.Instance.SetColor();
    // }

    [Server]
    public void Dead() {
        GetComponent<Telekinesis>().DropObject();
        transform.position = Spawner.Instance.CalculateSpawnPosition();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player:OnStartLocalPlayer");
        if(isClientOnly)
            gameObject.name = $"(Self) {gameObject.name}";

        ColorChanger.Instance.SetColor(_ownColor);
    }

    public override void OnStopServer()
    {
        if(!isLocalPlayer)
            NetworkInteraction.Instance.SaveInfo(connectionToClient.address, _ownColor, _scores);
    }
}