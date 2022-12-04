using UnityEngine;
using Mirror;
using TMPro;

public class TimeCounter : NetworkBehaviour 
{   
    [SerializeField] private TMP_Text _ownText;

    [SyncVar(hook = nameof(WhenChangeCurrentTime))] private float _currentTime;

    // NOTE: Will it be faster to call this function every second?
    public void WhenChangeCurrentTime(float oldTime, float newTime) {
        ChangeTime(Mathf.RoundToInt(newTime));
    }

    public override void OnStartServer()
    {
        _currentTime = GameData.Instance.GameTime;
    }

    [ServerCallback]
    private void FixedUpdate() {
        _currentTime -= Time.fixedDeltaTime;

        if(_currentTime <= 0 && !WinTracker.Instance.EndOfGame) {
            WinTracker.Instance.Win(true);
        }
    }

    public void ChangeTime(int time) {
        _ownText.text = $"{time / 60}:{(time % 60).ToString("00")}";
    }
}