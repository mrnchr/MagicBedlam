using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private TMP_InputField _hostIP;
    
    private void Awake() {
        _instance = this;

        _launchGame.gameObject.SetActive(false);
        _waitHost.gameObject.SetActive(false);
        _connected.gameObject.SetActive(false);
    }

    public void Exit() {
        Application.Quit();
    }

    public void LaunchHost() {
        if(!NetworkClient.active) {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Spawner.Instance.StartHost();
            }

            _launchGame.gameObject.SetActive(true);
            _connected.gameObject.SetActive(true);
        }
    }

    public void ConnectToHost() {
        if(!NetworkClient.active) {
            Spawner.Instance.networkAddress = _hostIP.text;
            Spawner.Instance.StartClient();

            _waitHost.gameObject.SetActive(true);
        }
    }

    [Server]
    public void ChangeConnected() {
        Debug.Log("MainMenu:ChangeConnected~print connected");
        _connected.text = $"Connected: {NetworkServer.connections.Count}";
    }
}
