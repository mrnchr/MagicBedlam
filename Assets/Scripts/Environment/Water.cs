using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Water : MonoBehaviour
{
    [SerializeField] private float _extraDrag;
    [SerializeField] private Transform _levelOfWater;
    [SerializeField] private GameObject _waterSpray;

    [ServerCallback]
    private void OnTriggerEnter(Collider other) {
        if(other.attachedRigidbody && !other.isTrigger) {
            other.attachedRigidbody.drag += _extraDrag;
            other.attachedRigidbody.angularDrag += _extraDrag;

            Vector3 sprayPos = other.transform.position;
            sprayPos.y = _levelOfWater.position.y;
            NetworkServer.Spawn(Instantiate(_waterSpray, sprayPos, Quaternion.identity));
        }

        //Debug.Log($"{other.gameObject} is gone in");
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other) {
        if(other.attachedRigidbody && !other.isTrigger) {
            other.attachedRigidbody.drag -= _extraDrag;
            other.attachedRigidbody.angularDrag -= _extraDrag;
        }

        //Debug.Log($"{other.gameObject} is gone out");
    }
}
