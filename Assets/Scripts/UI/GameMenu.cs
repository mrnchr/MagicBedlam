using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Interaction game and pause menu
    /// </summary>
    public class GameMenu : MonoBehaviour
    {
        public static GameMenu singleton { get; protected set; }

        [Tooltip("Play menu canvas")]
        [SerializeField] 
        protected GameObject _playMenu;
        [Tooltip("Pause menu canvas")]
        [SerializeField] 
        protected GameObject _pauseMenu;
        [Tooltip("Win menu canvas")]
        [SerializeField] 
        protected GameObject _winMenu;
        [Tooltip("The table of information about player")]
        [SerializeField] 
        protected RectTransform _playerTable;

        [Tooltip("Object displayed when win occured since time is out")]
        [SerializeField]
        protected GameObject _timeIsOut;

        public void Exit()
        {
            NetworkInteraction.singleton.Disconnect();
        }

        public void SetPauseMenu(bool isPause)
        {
            _playMenu.SetActive(!isPause);
            _pauseMenu.SetActive(isPause);

            Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void SetWinMenu(bool isOutOfTime)
        {
            _playMenu.SetActive(false);
            _pauseMenu.SetActive(false);
            _winMenu.SetActive(true);
            _timeIsOut.SetActive(isOutOfTime);

            Cursor.lockState = CursorLockMode.None;

            _playerTable.SetParent(_winMenu.transform);
            _playerTable.anchoredPosition = new Vector2(0.5f, 0.5f);
            _playerTable.pivot = new Vector2(0.5f, 0.5f);
            _playerTable.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        protected void Awake()
        {
            singleton = Singleton.Create<GameMenu>(this, singleton);
        }

        protected void Start()
        {
            Debug.Log("GameMenu:Start");
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}