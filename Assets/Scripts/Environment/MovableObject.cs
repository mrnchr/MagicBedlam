using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// The object which a player can control by telekinesis
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MovableObject : NetworkBehaviour
    {
        /// <summary>
        /// The object is throwing by the player
        /// </summary>
        [HideInInspector] public bool isThrowing;
        /// <summary>
        /// The player who threw the object
        /// </summary>
        [HideInInspector] public GameObject owner;

        [Tooltip("Assigned emission component")]
        [SerializeField]
        protected EmissionObject _ownEmission;

        protected MeshRenderer[] _meshes;

        protected void Awake()
        {
            _meshes = GetComponentsInChildren<MeshRenderer>();
        }
        
        /// <summary>
        /// Change the object layer and all its children
        /// </summary>
        /// <param name="layer"></param>
        public void ChangeLayer(int layer) {
            foreach(var child in GetComponentsInChildren<Transform>()) {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        ///     Switch the emission to the player color and back
        /// </summary>
        /// <param name="isHighlighted"></param>
        public void Glow(bool isHighlighted)
        {
            foreach (var mesh in _meshes)
            {
                foreach (var mat in mesh.materials)
                {
                    if (mat)
                    {
                        if (isHighlighted)
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                            mat.SetColor("_EmissionColor", Player.localPlayer.OwnColor);
                        }
                        else
                        {
                            Color oldEmission;
                            if (_ownEmission && _ownEmission.TryFindColor(mesh, out oldEmission))
                            {
                                mat.SetColor("_EmissionColor", oldEmission);
                            }
                            else
                            {
                                mat.DisableKeyword("_EMISSION");
                            }
                        }
                    }
                }
            }
        }

        [Server]
        protected void Kill(Player killed, Player killer)
        {
            killed.Die();
            Debug.Log($"The {GameData.singleton.GetColorName(killed.OwnColor).ToUpper()} player was killed");
        }

        [ServerCallback]
        protected void OnCollisionEnter(Collision col)
        {
            if (isThrowing && !col.collider.isTrigger && col.gameObject != owner)
            {
                isThrowing = false;

                if (col.gameObject.CompareTag("Player"))
                {
                    Debug.Log($"I catch {col.gameObject.name}");
                    Kill(col.gameObject.GetComponent<Player>(), owner.GetComponent<Player>());
                    WinTracker.singleton?.GiveScores(owner.GetComponent<Player>());
                }

                owner = null;
            }
        }
        
        protected void Start()
        {
            // NetworkServer.Destroy(gameObject);
            foreach(var child in GetComponentsInChildren<Transform>()) 
            {
                if(child.name == "Trigger") 
                {
                    Destroy(child.gameObject);
                }
            }
        }

        protected void Reset()
        {
            TryGetComponent<EmissionObject>(out _ownEmission);
            foreach (var child in GetComponentsInChildren<Transform>())
                child.gameObject.layer = LayerMask.NameToLayer("MovableObject");
        }
    }
}
