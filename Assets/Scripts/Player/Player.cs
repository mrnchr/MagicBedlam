using System.Collections;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Interaction the player with the network
    /// </summary>
    public class Player : NetworkBehaviour
    {
        static public Player localPlayer { get;  protected set; }

        [Tooltip("Own Renderer component")]
        [SerializeField] 
        protected Renderer _ownMesh;
        [SerializeField]
        protected PlayerAudioSource _ownAudio;

        [SyncVar(hook = nameof(WhenChangeOwnColor))] protected Color _ownColor;
        [SyncVar(hook = nameof(WhenChangeScores))] protected int _scores;

        public Color OwnColor
        {
            get
            {
                return _ownColor;
            }
        }

        /// <summary>
        /// The player drops an object and respawns in the new place
        /// </summary>
        [Server]
        public void Die()
        {
            GetComponent<Telekinesis>().DropObject();
            transform.position = Spawner.singleton.CalculateSpawnPosition();
            _ownAudio.RpcPlay(PlayerSound.Death);
        }

        /// <summary>
        /// Add a score
        /// </summary>
        /// <returns>A new number of scores</returns>
        [Server]
        public int IncrementScores()
        {
            ++_scores;
            _ownAudio.RpcPlay(PlayerSound.Score);
            Debug.Log($"Now the {GameData.singleton.GetColorName(_ownColor).ToUpper()} player has {_scores} scores");
            return _scores;
        }

        public override void OnStartClient()
        {
            Debug.Log("StartClient");
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Player:OnStartLocalPlayer");
            if (isClientOnly)
                gameObject.name = $"(Self) {gameObject.name}";

            localPlayer = this;

            ColorChanger.singleton?.SetColor(_ownColor);
            _ownMesh.gameObject.layer = 7;
        }

        public override void OnStartServer()
        {
            Debug.Log($"Connection on the server: {connectionToClient}");

            // NOTE: not to swap these strings. Color have to be got before scores 
            // that tablekeeper could to decide whether add the player on the table or not
            _ownColor = PlayerInfoKeeper.singleton.GetColor(connectionToClient.address);
            _scores = PlayerInfoKeeper.singleton.GetScores(connectionToClient.address);

            transform.position = Spawner.singleton.CalculateSpawnPosition();

            Debug.Log($"The player with ip {connectionToClient.address} got {GameData.singleton.GetColorName(_ownColor)} color and {_scores} scores");
        }

        public override void OnStopServer()
        {
            if (!isLocalPlayer)
                PlayerInfoKeeper.singleton.SaveInfo(connectionToClient.address, _ownColor, _scores);
        }

        protected void ChangeColorOfEyes(Color bodyColor) {
            Color brightColor = bodyColor;
            for (int i = 0; i < 3; i++)
            {
                if (brightColor[i] == 0)
                {
                    brightColor[i] = 0.75f;
                }
            }

            Material eyes = _ownMesh.materials[2];
            eyes.color = brightColor;
            eyes.EnableKeyword("_EMISSION");
            eyes.SetColor("_EmissionColor", bodyColor);
        }

        protected void WhenChangeOwnColor(Color oldValue, Color newValue)
        {
            Debug.Log("The own color is changed");
            TableKeeper.singleton?.AddPlayerInfo(newValue);
            _ownMesh.material.color = newValue;

            ChangeColorOfEyes(newValue);
        }

        protected void WhenChangeScores(int oldValue, int newValue)
        {
            TableKeeper.singleton?.ChangePlayerTable(_ownColor, newValue);
        }

        protected void Reset() {
            gameObject.tag = "Player";
        }
    }
}