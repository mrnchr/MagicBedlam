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
        int spawnIndex = RandomSpawnIndex();
        Vector3 endPosition = RandomPositionBySquare(_spawners[spawnIndex].position, _spawners[spawnIndex].localScale);

        // _spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.black;
        Debug.Log($"Spawn position: {endPosition}, from object {_spawners[spawnIndex].name}");
        if(_busy[spawnIndex] == true) {
            Debug.LogError("Spawn position was calculated from object which is sleeping");
        }

        _busy[spawnIndex] = true;
        StartCoroutine(WaitForDelay(spawnIndex));

        return endPosition;
    }

    private Vector3 RandomPositionBySquare(Vector3 pos, Vector3 scale) {
        return new Vector3(RandomPointByLength(pos.x, scale.x / 2), pos.y, RandomPointByLength(pos.z, scale.z / 2));
    }
    
    private float RandomPointByLength(float start, float length) {
        return Random.Range(start - length, start + length);
    }

    private int RandomSpawnIndex() {
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

        return spawnIndex;
    }

    private IEnumerator WaitForDelay(int spawnIndex) {
        yield return new WaitForSeconds(_delay);

        _busy[spawnIndex] = false;
        // _spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.white;
    }
}