using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Main menu
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Tooltip("Button which launches a session")]
        [SerializeField] 
        protected GameObject _launchGame;
        [Tooltip("Object with text about host waiting")]
        [SerializeField] 
        protected GameObject _waitHost;
        [Tooltip("Object with text about a number of connections")]
        [SerializeField] 
        protected TMP_Text _connected;
        [Tooltip("Text field which changes on the pressing client button")]
        [SerializeField] 
        protected TMP_Text _forClientText;
        [Tooltip("Text field which changes on the pressing host button")]
        [SerializeField] 
        protected TMP_Text _forHostText;
        [Tooltip("Input field where host ip is entered")]
        [SerializeField] 
        protected HostIpField _hostIP;

        [Header("Development Data")]
        [Tooltip("Enable launch session on the start with one player")]
        [SerializeField] 
        protected bool _startHostAtOnce;

        protected void Awake()
        {
            _launchGame.SetActive(false);
            _waitHost.SetActive(false);
            _connected.gameObject.SetActive(false);
        }

        protected void Start()
        {
            NetworkInteraction.singleton.OnClientEvent += ChangeClientMenu;
            NetworkInteraction.singleton.OnServerEvent += ChangeConnected;

            Cursor.lockState = CursorLockMode.None;
#if UNITY_EDITOR
            if (_startHostAtOnce)
            {
                ChangeHostConnection();
                LaunchGame();
            }
#endif
        }

        protected void OnDisable()
        {
            NetworkInteraction.singleton.OnClientEvent -= ChangeClientMenu;
            NetworkInteraction.singleton.OnServerEvent -= ChangeConnected;
        }

        /// <summary>
        /// Exit from game
        /// </summary>
        public void Exit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Start or stop the host and show or hide related menu
        /// </summary>
        public void ChangeHostConnection()
        {
            if (!NetworkClient.active && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                NetworkInteraction.singleton.StartHost();
                _forHostText.text = "Stop Host";

                _launchGame.SetActive(true);
                _connected.gameObject.SetActive(true);
            }
            else if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkInteraction.singleton.StopHost();
                _forHostText.text = "Launch Host";

                _launchGame.SetActive(false);
                _connected.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Start or stop the client and show or hide related menu
        /// </summary>
        public void ChangeClientConnection()
        {
            if (!NetworkClient.active)
            {
                NetworkInteraction.singleton.networkAddress = _hostIP.Text;
                NetworkInteraction.singleton.StartClient();
            }
            else
            {
                NetworkInteraction.singleton.StopClient();
            }
        }

        /// <summary>
        /// Launch the game
        /// </summary>
        [Server]
        public void LaunchGame()
        {
            NetworkInteraction.singleton.ServerChangeScene("Castle");
        }

        protected void ChangeClientMenu()
        {
            _forClientText.text = NetworkClient.isConnected ? "Stop Client" : "Launch Client";
            _waitHost.SetActive(NetworkClient.isConnected);
        }

        [Server]
        protected void ChangeConnected()
        {
            Debug.Log("MainMenu:NetworkInteraction");
            _connected.text = $"Connected: {NetworkServer.connections.Count}";
        }
    }
}
