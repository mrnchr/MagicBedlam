using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Calculation random position in a random area    
    /// </summary>
    public class Spawner : NetworkBehaviour
    {
        static public Spawner singleton { get; protected set; }

        [Tooltip("The cooldown after the spawn in seconds")]
        [SerializeField]
        [Range(0, 10)]
        protected float _delay;
        [Tooltip("Box objects within which players spawn. The size of the box is defined by its scale")]
        [SerializeField]
        protected Transform[] _spawners;

        protected bool[] _busy;

        protected void Awake()
        {
            singleton = Singleton.Create<Spawner>(this, singleton);
        }

        /// <summary>
        ///     Calculate the spawn position
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateSpawnPosition()
        {
            int spawnIndex = GetRandomSpawnIndex();
            Vector3 endPosition = GetRandomPositionBySquare(_spawners[spawnIndex].position, _spawners[spawnIndex].localScale);

            // _spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.black;
            Debug.Log($"Spawn position: {endPosition}, from object {_spawners[spawnIndex].name}");
            if (_busy[spawnIndex] == true)
            {
                Debug.LogError("Spawn position was calculated from object which has the cooldown");
            }

            _busy[spawnIndex] = true;
            StartCoroutine(WaitForDelay(spawnIndex));

            return endPosition;
        }

        public override void OnStartServer()
        {
            Debug.Log("Spawner:OnStartServer");
            _busy = new bool[_spawners.Length];
        }

        protected Vector3 GetRandomPositionBySquare(Vector3 pos, Vector3 scale)
        {
            return new Vector3(GetRandomPointByLength(pos.x, scale.x / 2), pos.y, GetRandomPointByLength(pos.z, scale.z / 2));
        }

        protected float GetRandomPointByLength(float start, float length)
        {
            return Random.Range(start - length, start + length);
        }

        protected int GetRandomSpawnIndex()
        {
            Dictionary<int, bool> tBusy = new Dictionary<int, bool>();

            for (int i = 0; i < _busy.Length; i++)
            {
                tBusy[i] = _busy[i];
            }

            int spawnIndex = Random.Range(0, tBusy.Count);
            int dictionaryIndex;

            while (tBusy[spawnIndex])
            {
                tBusy.Remove(spawnIndex);

                // NOTE: finds index of _busy (spawnIndex) lain in dictionaryIndex element of tBusy.
                // SpawnIndex and dictionaryIndex are different because we remove an element from tBusy before it
                dictionaryIndex = Random.Range(0, tBusy.Count);
                foreach (var busy in tBusy)
                {
                    spawnIndex = busy.Key;
                    --dictionaryIndex;
                    if (dictionaryIndex < 0)
                        break;
                }
            }

            return spawnIndex;
        }

        protected IEnumerator WaitForDelay(int spawnIndex)
        {
            yield return new WaitForSeconds(_delay);

            _busy[spawnIndex] = false;
            // _spawners[spawnIndex].GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}