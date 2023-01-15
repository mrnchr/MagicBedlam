using System.Collections.Generic;
using UnityEngine;

namespace MagicBedlam
{
    /// <summary>
    ///     Store meshes which emission and them emission color
    ///     and restore colors after them changes
    /// </summary>
    public class EmissionObject : MonoBehaviour
    {
        [Tooltip("Emission meshes")]
        [SerializeField]
        protected MeshRenderer[] _meshes;

        protected Color[] _colors;

        protected void Start()
        {
            _colors = new Color[_meshes.Length];

            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = _meshes[i].material.GetColor("_EmissionColor");
            }
        }

        /// <summary>
        ///     Find the starting color of the given object if it is stored          
        /// </summary>
        /// <param name="match"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool TryFindColor(MeshRenderer match, out Color origin)
        {
            origin = Color.black;

            for (int i = 0; i < _colors.Length; i++)
            {
                if (match == _meshes[i])
                {
                    origin = _colors[i];
                    return true;
                }
            }

            return false;
        }
    }
}