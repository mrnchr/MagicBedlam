using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour 
{
    #region Singleton
    
    static private Spawner _instance;

    static public Spawner Instance {
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

    [SerializeField] private Transform[] _spawners;
    [SerializeField] private float _delay;

    private bool[] _busy;

    public override void OnStartServer() {
        Debug.Log("Spawner:OnStartServer");
        _busy = new bool[_spawners.Length];
    }

    public Vector3 CalculateSpawnPosition() {

        Dictionary<int, bool> tBusy = new Dictionary<int, bool>();
        
        for (int i = 0; i < _busy.Length; i++) {
            tBusy.Add(i, _busy[i]);
        }

        int spawnIndex = Random.Range(0, tBusy.Count); 
        int dictionaryIndex;

        while(tBusy[spawnIndex]) {
            tBusy.Remove(spawnIndex);

            dictionaryIndex = Random.Range(0, tBusy.Count);
            foreach(var busy in tBusy) {
                spawnIndex = busy.Key;
                --dictionaryIndex;
                if(dictionaryIndex < 0) 
                    break;
            }
        }

        Vector3 endPosition = new Vector3 (
            Random.Range(_spawners[spawnIndex].position.x - _spawners[spawnIndex].localScale.x,
                _spawners[spawnIndex].position.x - _spawners[spawnIndex].localScale.x),
            _spawners[spawnIndex].position.y,
            Random.Range(_spawners[spawnIndex].position.x - _spawners[spawnIndex].localScale.x,
                _spawners[spawnIndex].position.z - _spawners[spawnIndex].localScale.z)
        );

        //_spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.black;
        Debug.Log($"Spawn position: {endPosition}, from object {_spawners[spawnIndex].name}");
        if(_busy[spawnIndex] == true) {
            Debug.LogError("Spawn position was calculated from object which is relaxing");
        }

        _busy[spawnIndex] = true;
        StartCoroutine(WaitForDelay(spawnIndex));

        return endPosition;
    }

    private IEnumerator WaitForDelay(int spawnIndex) {
        yield return new WaitForSeconds(_delay);

        _busy[spawnIndex] = false;
        //_spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.white;
    }
}