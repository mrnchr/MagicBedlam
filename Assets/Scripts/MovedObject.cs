using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovedObject : MonoBehaviour
{
    
    void Start()
    {
        tag = "MovedObject";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Glow(bool highlighted)
    {
        if(highlighted)
            this.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        else 
            this.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }
}
