using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace OldScript {
    [RequireComponent(typeof(Rigidbody))]
    public class MovedObject : MonoBehaviour
    {
        private Material _material;
        
        private void Start()
        {
            tag = "MovedObject";
            _material = GetComponent<MeshRenderer>().material;

        }

        public void Glow(bool highlighted)
        {
            if(highlighted)
                _material.EnableKeyword("_EMISSION");
            else 
                _material.DisableKeyword("_EMISSION");
        }
    }
}
