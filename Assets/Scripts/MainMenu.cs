using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class MainMenu : MonoBehaviour
{
    #region Singleton
    private static MainMenu _instance;

    public static MainMenu Instance {
        get {
            return _instance;
        }
    }
    #endregion

    [SerializeField] private Button _launchGame;
    [SerializeField] private TMP_Text _waitHost;
    [SerializeField] private TMP_Text _connected;
    [SerializeField] private TMP_Text _forClientText;
    [SerializeField] private TMP_Text _forHostText;

    [SerializeField] private TMP_InputField _hostIP;
    
    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _launchGame.gameObject.SetActive(false);
        _waitHost.gameObject.SetActive(false);
        _connected.gameObject.SetActive(false);
    }

    public void Exit() {
        Application.Quit();
    }

    public void ForHost() {
        if(!NetworkClient.active && Application.platform != RuntimePlatform.WebGLPlayer) {
            NetworkInteraction.Instance.StartHost();
            _forHostText.text = "Stop Host";

            _launchGame.gameObject.SetActive(true);
            _connected.gameObject.SetActive(true);
        }
        else if(NetworkServer.active && NetworkClient.isConnected) {
            NetworkInteraction.Instance.StopHost();
            _forHostText.text = "Launch Host";

            _launchGame.gameObject.SetActive(false);
            _connected.gameObject.SetActive(false);
        }
    }

    public void ForClient() {
        if(!NetworkClient.active) {
            NetworkInteraction.Instance.networkAddress = _hostIP.text;
            NetworkInteraction.Instance.StartClient();
        }
        else {
            NetworkInteraction.Instance.StopClient();
        }
    }

    public void ChangeClientMenu(bool isConnected) {
        _forClientText.text = isConnected ? "Stop Client" : "Launch Client";
        _waitHost.gameObject.SetActive(isConnected);
        Debug.Log("Main menu is changed");
    }

    [Server]
    public void ChangeConnected() {
        Debug.Log("MainMenu:ChangeConnected~print connected");
        _connected.text = $"Connected: {NetworkServer.connections.Count}";
    }

    [Server]
    public void PlayGame() {
        NetworkInteraction.Instance.ServerChangeScene("Island");
    }
}
