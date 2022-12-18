using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WinTracker : NetworkBehaviour {

    #region Singleton
    static private WinTracker _instance;

    static public WinTracker Instance {
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

        Debug.Log("WinTracker:Awake");
    }
    #endregion

    // NOTE: used that win function by time wouldn't be called several times 
    [SyncVar] private bool _endOfGame;

    public bool EndOfGame {
        get {
            return _endOfGame;
        }
    }

    // NOTE: It's needed to process the order of operations and keep track of win event 
    [Server]
    public void PlayerDeath(Player killed, Player killer) {
        if(!killed) {
            Debug.LogError("Don't try to call player death without the killed player");
        }

        killed.Dead();

        if(killer == null) {
            Debug.Log($"The {GameData.Instance.GetColorName(killed.OwnColor).ToUpper()} player killed himself");
        }
        if(killer != null) {
            Debug.Log($"The {GameData.Instance.GetColorName(killed.OwnColor).ToUpper()} player was killed by the {GameData.Instance.GetColorName(killer.OwnColor).ToUpper()} player");

            int newScores = killer.IncrementScores();

            Debug.Log($"Now the {GameData.Instance.GetColorName(killer.OwnColor).ToUpper()} player has {newScores} scores");
            
            if(newScores >= GameData.Instance.WinScores) {
                Win(false);
            }
        }
    }

    [Server]
    public void Win(bool outOfTime) {
        Debug.Log(outOfTime ? "Time is up" : "Win!");

        _endOfGame = true;
        RpcSetWin(outOfTime);
        StartCoroutine(WaitForEndOfGame());
    }

    [ClientRpc] 
    private void RpcSetWin(bool outOfTime) {
        GameMenu.Instance.SetWinMenu(outOfTime);
    }

    [Server] 
    private IEnumerator WaitForEndOfGame() {
        yield return new WaitForSeconds(5);
        NetworkInteraction.Instance.Disconnect();
    }

}
