using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Area which players die in
    /// </summary>
    public class DeathZone : MonoBehaviour
    {
        [Server]
        protected void Kill(Player killed)
        {
            killed.Die();
            Debug.Log($"The {GameData.singleton.GetColorName(killed.OwnColor).ToUpper()} player killed himself");
        }
        
        [ServerCallback]
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Kill(other.GetComponent<Player>());
            }
        }
    }
}
