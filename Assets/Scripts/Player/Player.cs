using System.Collections;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour 
{
    private static Player _localPlayer;

    public static Player LocalPlayer {
        get {
            return _localPlayer;
        }
    }

    [SerializeField] private SkinnedMeshRenderer _ownMesh;
    [SyncVar(hook = nameof(WhenChangeOwnColor))] private Color _ownColor;
    [SyncVar(hook = nameof(WhenChangeScores))] private int _scores;

    public Color OwnColor {
        get {
            return _ownColor;
        }
    }

    public void WhenChangeOwnColor(Color oldValue, Color newValue) {
        Debug.Log("The own color is changed");
        TableKeeper.Instance.AddPlayerInfo(newValue);
        _ownMesh.material.color = newValue;
        
        Color brightColor = newValue;
        for(int i = 0; i < 3; i++) {
            if(brightColor[i] == 0) {
                brightColor[i] = 3f/4f;
            }
        }

        Material eyes = _ownMesh.materials[2];
        eyes.color = brightColor;
        eyes.EnableKeyword("_EMISSION");
        eyes.SetColor("_EmissionColor", newValue);
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

        Debug.Log($"The player with ip {connectionToClient.address} got {GameData.Instance.GetColorName(_ownColor)} color and {_scores} scores");
    }

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

            _localPlayer = this;

        ColorChanger.Instance.SetColor(_ownColor);
        _ownMesh.gameObject.layer = 7;
    }

    public override void OnStopServer()
    {
        if(!isLocalPlayer)
            NetworkInteraction.Instance.SaveInfo(connectionToClient.address, _ownColor, _scores);
    }
}