using UnityEngine;
using Mirror;

public class GameMenu : MonoBehaviour 
{
    #region Singleton
    static private GameMenu _instance;

    static public GameMenu Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    [SerializeField] private GameObject _playMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private RectTransform _playerTable;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetPause(bool isPause) {
        _playMenu.SetActive(!isPause);
        _pauseMenu.SetActive(isPause);
        
        Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetWinMenu(bool outOfTime) {
        _playMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _winMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;

        _playerTable.SetParent(_winMenu.transform);
        _playerTable.anchoredPosition = new Vector2(0.5f, 0.5f);
        _playerTable.pivot = new Vector2(0.5f, 0.5f);
        _playerTable.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    public void Exit() {
        NetworkInteraction.Instance.Disconnect();
    }

    private void OnDestroy() {
        Cursor.lockState = CursorLockMode.None;
    }
}