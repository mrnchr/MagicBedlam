using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float limitTelekinesis;

    private GameObject _camera;
    private MovedObject _movedObject;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _movedObject = null;
    }

    void Update()
    {
        RaycastHit hit;


        // TODO: Duplicate code. I would correct
        if(Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit))
        {
            if(hit.transform.tag == "MovedObject" && Vector3.Distance(hit.transform.position, transform.position) <= limitTelekinesis)
            {
                _movedObject = hit.transform.GetComponent<MovedObject>();
                _movedObject.Glow(true);
            }
            else if (_movedObject)
            {
                _movedObject.Glow(false);
                _movedObject = null;
            }
        }
        else if (_movedObject)
        {
            _movedObject.Glow(false);
            _movedObject = null;
        }
    }
}
