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
    }
    #endregion

    [SyncVar] private bool _endOfGame;

    public bool EndOfGame {
        get {
            return _endOfGame;
        }
    }

    [Server]
    public void PlayerDeath(Player killed, Player killer) {
        killed.Dead();
        Debug.Log($"The player {killed.netIdentity.connectionToClient} was killed by the player {killer.netIdentity.connectionToClient}");

        int newScores = killer.IncrementScores();

        Debug.Log($"Now the player {killer.netIdentity.connectionToClient} has {newScores} scores");
        
        if(newScores >= GameData.Instance.WinScores) {
            Win(false);
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
        Spawner.Instance.Disconnect();
    }

}
